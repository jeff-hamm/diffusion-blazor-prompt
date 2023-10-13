from accelerate.utils.modeling import safe_load_file
from diffusers import ControlNetModel, DiffusionPipeline, StableDiffusionXLControlNetPipeline, AutoencoderKL
import torch
from diffusers.utils import load_image
from img_utils import resize
from PIL import Image
import numpy as np
import cv2
import os
import base64
from io import BytesIO
from .PromptState import PromptState
from .PromptArgs import PromptArgs
from .ListenerStream import ListenerStream


class PromptPipeline:
    state: PromptState
    sdxlBase: DiffusionPipeline
    pipeline: DiffusionPipeline
    listener: ListenerStream
    def __init__(self, listener:ListenerStream):
        self.listener = listener
        self.state = PromptState()
        
    def load(self): 
        self.controlnet = ControlNetModel.from_pretrained(
            "diffusers/controlnet-canny-sdxl-1.0",
            torch_dtype=torch.float16,
            use_safetensors = True
        )
        self.vae = AutoencoderKL.from_pretrained("madebyollin/sdxl-vae-fp16-fix", torch_dtype=torch.float16, use_safetensors=True)
        self.sdxlBase = StableDiffusionXLControlNetPipeline.from_pretrained(
            "stabilityai/stable-diffusion-xl-base-1.0",
            controlnet=self.controlnet,
            torch_dtype=torch.float16,
        #    variant="fp16",
            vae=self.vae,
            use_safetensors=True
        )
        if(os.name != "nt"):
            self.sdxlBase.unet = torch.compile(self.sdxlBase.unet, mode="reduce-overhead", fullgraph=True)
        self.sdxlBase.to("cuda")
        # load both base & refiner
        # refiner = DiffusionPipeline.from_pretrained(
        #     "stabilityai/stable-diffusion-xl-refiner-1.0",
        #     text_encoder_2=base.text_encoder_2,
        #     vae=vae,
        #     torch_dtype=torch.float16,
        #     use_safetensors=True,
        # #    variant="fp16",
        # )
        # refiner.to("cuda")
        self.pipeline = self.sdxlBase

    def generatebutt(self, args:PromptArgs):
        try:
            self.state.running = True
            self.state.file = args
            if(args.controlfile is not None):
                imgdata = base64.b64decode(args.controlfile)
                control_image = Image.open(io.BytesIO(imgdata))
            else:
                control_image = load_image(args.controlpath)
            control_image = resize(control_image, args.controlsize)
            control_image = np.array(control_image)
            control_image = cv2.Canny(control_image, args.cannylow, args.cannyhigh)
            control_image = control_image[:, :, None]
            control_image = np.concatenate([control_image, control_image, control_image], axis=2)
            control_image = Image.fromarray(control_image)
            control_image.save('./control.png')

            print("start")
            # run both experts
            images = self.pipeline(
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
            if(args.outputfile != None):
                image.save(args.outputfile)
#            print("output:" + args.outputfile)
            buffered = BytesIO()
            image.save(buffered, format="PNG")
            return base64.b64encode(buffered.getvalue()).decode('ascii')
        finally:
           self.state.running = False