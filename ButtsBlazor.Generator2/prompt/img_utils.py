from PIL import Image
from diffusers.utils import load_image
import numpy as np
from transformers import pipeline
import cv2
from enum import Enum

# class ScaleDimension(Enum):
#     LONG=0,
#     SHORT=1
ScaleDimension = Enum('ScaleDimension', ['LONG', 'SHORT'])

def crop(img:Image, img_size:int):
    width, height = img.size
    if(width <= img_size and height <= img_size):
        return img
    left=(width-img_size)/2
    top=(height-img_size)/2
    if(left < 0):
        left=0
    if(top < 0):
        top=0
    
    return img.crop((left, top, width-left, height-top))

def resize(img:Image, img_size:int, scale_dimension=ScaleDimension.LONG,scaleUp:bool=False):
    w = img.size[0]
    h = img.size[1]
    if((w>h and scale_dimension == ScaleDimension.LONG) or
       (h>w and scale_dimension == ScaleDimension.SHORT)):
        pct = (img_size / float(w))
        h = int((float(h) * float(pct)))
        w = img_size
    else:
        pct = (img_size / float(h))
        w = int((float(w) * float(pct)))
        h = img_size
    if(not scaleUp and pct > 1.0):
        return img
       
    img = img.resize((w, h), Image.Resampling.BICUBIC)
    new_im = Image.new('RGBA', (img_size, img_size), (0, 0, 0, 255))
    new_im.paste(img, (int((img_size - w) / 2), int((img_size - h) / 2)))
    return new_im

def load_conltrolnet_image(url: str, isCanny: bool):
    img:  Image.Image = load_image(url)
    img = np.array(img)
    if(isCanny):
        img = cv2.Canny(img, True, Config.low_threshold, Config.high_threshold)
    else:
        img = pipeline('depth-estimation')(img)['depth']
    img = img[:, :, None]
    img = np.concatenate([img, img, img], axis=2)
    canny = Image.fromarray(img)
    canny.save("./images/c2.png")
    return resize(canny)
