from accelerate.utils.modeling import safe_load_file
from diffusers import ControlNetModel, DiffusionPipeline, StableDiffusionXLControlNetPipeline, AutoencoderKL
import torch
from diffusers.utils import load_image
from .img_utils import resize,crop
from PIL import Image
import numpy as np
import cv2
import os
import base64
from io import BytesIO
from .PromptArgs import PromptArgs, ControlArgs
from gradio import Progress

class PromptPipeline:
    controlnet: DiffusionPipeline
    sdxlBase: DiffusionPipeline
    vae: DiffusionPipeline
    pipeline: DiffusionPipeline
    
    running: bool
    def __init__(self):
        self.controlnet = None
        self.sdxlBase = None
        self.vae = None
        self.pipeline = None

    def load_controlnet(self):
        if(self.controlnet is None):
            self.controlnet = ControlNetModel.from_pretrained(
                "diffusers/controlnet-canny-sdxl-1.0",
                torch_dtype=torch.float16,
                use_safetensors = True
            )
        return self.controlnet
    
    def load_vae(self):
        if(self.vae is None):
            self.vae = AutoencoderKL.from_pretrained("madebyollin/sdxl-vae-fp16-fix", torch_dtype=torch.float16, use_safetensors=True)
        return self.vae
        
    def load_sdxl(self):
        if(self.sdxlBase is None):
            self.sdxlBase = StableDiffusionXLControlNetPipeline.from_pretrained(
                "stabilityai/stable-diffusion-xl-base-1.0",
                controlnet=self.load_controlnet(),
                torch_dtype=torch.float16,
            #    variant="fp16",
                vae=self.load_vae(),
                use_safetensors=True,
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
        return self.sdxlBase
            
    def is_loaded(self):
        return self.pipeline is not None
    
    def load_pipeline(self):
        if(self.pipeline is None):
            self.pipeline = self.load_sdxl()
        return self.pipeline

    def generate_canny(self, control_image, args:ControlArgs):
        if(args.scale or args.crop):
            control_image = Image.fromarray(control_image)
            if(args.scale):
                control_image = resize(control_image, args.img_size, args.scaleDimension, args.scaleUp)
            if(args.crop):
                control_image = crop(control_image, args.img_size);
            control_image = np.array(control_image)
        control_image = cv2.Canny(control_image, args.low, args.high)
        control_image = control_image[:, :, None]
        return np.concatenate([control_image, control_image, control_image], axis=2)
#        return Image.fromarray(control_image)

    def generatebutts(self,control_image,prompt,negative,args:PromptArgs,progress:Progress):
        try:
            self.running = True
            control_image =Image.fromarray(control_image)
            width,height = control_image.size
            ratio = width/float(height)
            if(ratio < 1.0):
                width = args.img_size*ratio
                height = args.img_size
            elif (ratio > 1.0):
                width = args.img_size
                height = args.img_size*(1/ratio)
            # run both experts
            progress.track_tqdm
            images = self.load_pipeline()(
                prompt=prompt,
                negative_prompt=negative,
                num_images_per_prompt=args.numOutputs,
                image=control_image,
                controlnet_conditioning_scale=args.controlScale,
                height=int(height),
                width=int(width),
                num_inference_steps=args.numSteps,
            #    denoising_end=high_noise_frac,
            #   output_type="latent",
            ).images
            #images = refiner(
            #    prompt=prompt,
            #    num_inference_steps=n_steps,
            #    denoising_start=high_noise_frac,
            #    image=images,
            #).images
            return images
        finally:
           self.running = False