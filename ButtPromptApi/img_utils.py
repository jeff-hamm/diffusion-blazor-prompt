from PIL import Image
from diffusers.utils import load_image
import numpy as np
from transformers import pipeline
import cv2
def resize(img, img_size):
    w = img.size[0]
    h = img.size[1]
    if(w>h):
        pct = (img_size / float(w))
        h = int((float(h) * float(pct)))
        w = img_size
    else:
        pct = (img_size / float(h))
        w = int((float(w) * float(pct)))
        h = img_size
    img = img.resize((w, h), Image.Resampling.LANCZOS)
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
