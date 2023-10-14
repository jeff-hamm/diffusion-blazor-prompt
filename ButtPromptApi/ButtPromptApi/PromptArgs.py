from numpy import double

class PromptArgs:
    def __init__(self, my_dict=None):
        if(my_dict is not None):
            for key in my_dict:
                if(my_dict[key] is not None):
                    setattr(self, key, my_dict[key])
    id: str
    prompt: str | None = "a rear view of a butt, holding a futuristic cyborg tool, on the moon and with asteroids in the sky, Nikon Z9, Canon 5d, masterpiece"
    negative: str | None = 'low quality, bad quality, sketches'
    controlFile: str = None
    cannyLow: int = 80
    cannyHigh: int = 100
    controlSize: int = 768
    controlScale: double = 0.5
    numSteps: int = 40
    controlPath: str = './butts.png'
    outputFile: str = None
