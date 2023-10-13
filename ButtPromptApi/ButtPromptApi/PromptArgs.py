from numpy import double

class PromptArgs:
    def __init__(self, my_dict=None):
        if(my_dict is not None):
            for key in my_dict:
                setattr(self, key, my_dict[key])
    id: str
    prompt: str | None = "a rear view of a butt, holding a futuristic cyborg tool, on the moon and with asteroids in the sky, Nikon Z9, Canon 5d, masterpiece"
    negative: str | None = 'low quality, bad quality, sketches'
    controlfile: str = None
    cannylow: int = 80
    cannyhigh: int = 100
    controlsize: int = 768
    controlscale: double = 0.5

    controlpath: str = './butts.png'
    outputfile: str = None
