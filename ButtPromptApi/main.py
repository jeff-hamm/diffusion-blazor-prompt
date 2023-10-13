from diffusers import ControlNetModel, DiffusionPipeline, StableDiffusionXLControlNetPipeline, AutoencoderKL
from diffusers.utils import load_image
from PIL import Image
from img_utils import resize
from config import Config
import torch
import numpy as np
import cv2
import getopt
import sys
class Args:
    prompt = "a rear view of a butt, holding a futuristic cyborg tool, on the moon and with asteroids in the sky, Nikon Z9, Canon 5d, masterpiece"
    negative ='low quality, bad quality, sketches'
    controlpath = './butts.png'
    controlsize = Config.controlnet_img_size
    controlscale = 0.5
    cannylow = 80
    cannyhigh = 100
    output = "./output.png"

def generatebutt(argv):
    args = Args()
    arg_help = "{0} -p <prompt> -n <negative> -i <controlpath> -s <controlsize> -c <controlscale> -l <cannylow> -u <cannyhigh> -o <output>".format(argv[0])
    try:
        opts, arg = getopt.getopt(argv[1:], "hp:n:i:s:c:l:u:o:", ["help", "prompt",
                                                 "negative", "controlpath",
                                                 "controlsize","controlscale",
                                                 "cannylow","cannyhigh","output"]
                                  )
    except:
        print(arg_help)
        sys.exit(2)
    for opt, arg in opts:
        if opt in ("-h", "--help"):
            print(arg_help)  # print the help message
            sys.exit(2)
        elif opt in ("-p", "--prompt"):
            args.prompt = arg
        elif opt in ("-n", "--negative"):
            args.negative = arg
        elif opt in ("-i", "--controlpath"):
            args.controlpath = arg
        elif opt in ("-s", "--controlsize"):
            args.controlsize = int(arg)
        elif opt in ("-c", "--controlscale"):
            args.controlscale = float(arg)
        elif opt in ("-l", "--cannylow"):
            args.cannylow = int(arg)
        elif opt in ("-h", "--cannyhigh"):
            args.cannyhigh = int(arg)
        elif opt in ("-o", "--output"):
            args.output = arg

    controlnet = ControlNetModel.from_pretrained(
        "diffusers/controlnet-canny-sdxl-1.0",
        torch_dtype=torch.float16,
        use_safetensors = True
    )
    vae = AutoencoderKL.from_pretrained("madebyollin/sdxl-vae-fp16-fix", torch_dtype=torch.float16, use_safetensors=True)

    control_image = resize(load_image(args.controlpath), args.controlsize)
    #control_image = load_image("https://huggingface.co/datasets/hf-internal-testing/diffusers-images/resolve/main/sd_controlnet/hf-logo.png")

    control_image = np.array(control_image)
    control_image = cv2.Canny(control_image, args.cannylow, args.cannyhigh)
    control_image = control_image[:, :, None]
    control_image = np.concatenate([control_image, control_image, control_image], axis=2)
    control_image = Image.fromarray(control_image)
    control_image.save('./control.png')

    # load both base & refiner
    base = StableDiffusionXLControlNetPipeline.from_pretrained(
        "stabilityai/stable-diffusion-xl-base-1.0",
        controlnet=controlnet,
        torch_dtype=torch.float16,
    #    variant="fp16",
        vae=vae,
        use_safetensors=True
    )
    base.unet = torch.compile(base.unet, mode="reduce-overhead", fullgraph=True)
    base.to("cuda")
    # refiner = DiffusionPipeline.from_pretrained(
    #     "stabilityai/stable-diffusion-xl-refiner-1.0",
    #     text_encoder_2=base.text_encoder_2,
    #     vae=vae,
    #     torch_dtype=torch.float16,
    #     use_safetensors=True,
    # #    variant="fp16",
    # )
    # refiner.to("cuda")
    #n_steps = 40

    # Define how many steps and what % of steps to be run on each experts (80/20) here
    #high_noise_frac = 0.8

    #args.prompt = "    "

    print("start")
    # run both experts
    images = base(
        prompt=args.prompt,
        negative_prompt=args.negative,
        image=control_image,
        controlnet_conditioning_scale=args.controlscale,
        height=args.controlsize,
        width=args.controlsize
    #    num_inference_steps=n_steps,
    #    denoising_end=high_noise_frac,
    #   output_type="latent",
    ).images
    #images = refiner(
    #    prompt=prompt,
    #    num_inference_steps=n_steps,
    #    denoising_start=high_noise_frac,
    #    image=images,
    #).images
    image = images[0]
    image.save(args.output)
    print("output:" + args.output)

if __name__ == "__main__":
    generatebutt(sys.argv)