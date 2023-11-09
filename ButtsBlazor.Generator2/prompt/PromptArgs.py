from tkinter import SE
from numpy import double
from .img_utils import ScaleDimension
import json

class ControlArgs(object):
    def __init__(self, my_dict=None):
        self.low = 100
        self.high = 150
        self.scale = True
        self.scaleUp = False
        self.img_size = 768
        self.scaleDimension = ScaleDimension.LONG
        self.crop = True

        if(my_dict is not None):
            for key,val in my_dict:
                if(val is not None):
                    setattr(self, key, val)
    low: int
    high: int
    scale: bool
    scaleUp: bool
    img_size: int
    scaleDimension: int
    crop: bool



        
class PromptArgs:
    def __init__(self, my_dict=None):
        self.prompt = "a rear view of a butt, holding a futuristic cyborg tool, on the moon and with asteroids in the sky, Nikon Z9, Canon 5d, masterpiece"
        self.negative = 'low quality, bad quality, sketches'
        self.img_size = 768
        self.numSteps = 40
        self.controlScale = 0.5
        self.numOutputs = 1
        if(my_dict is not None):
            for key,val in my_dict:
                if(val is not None): 
                    if(key == "controlnet" and hasattr(val, "items") ):
                        self.controlnet = ControlArgs(val.items())
                    else:
                        setattr(self, key, val)
        if(self.controlnet is None):
           self.controlnet = ControlArgs();
               
    id: str
    prompt: str | None
    negative: str | None
    controlnet: ControlArgs = None
    img_size: int
    numSteps: int
    numOutputs: int
    outputFile: str = None
    controlScale: double
    
    def toJSON(self):
        return json.dumps(self, default=lambda o: o.__dict__, 
            sort_keys=True, indent=4)
    @staticmethod
    def read_config_file(config_file_name: str = "config.json"):
        try:
            with open(config_file_name) as conf_file:
                return PromptArgs(json.loads(conf_file.read()).items())
        except Exception as e:
            print(f"Error was detected while reading {config_file_name}: {str(e)}. Hard coded values will be applied")
            return PromptArgs()
            
    def save_config_file(self, config_file_name: str = "config.json"):
        try:
    #            conf_items = {k: v for k, v in vars(self).items() if isinstance(v, (int, float, str, list, dict,object))}
            with open(config_file_name, "w") as conf_file:
                conf_file.write(self.toJSON())
    #                json.dump(conf_items, conf_file, sort_keys=False, indent=2)
        except Exception as e:
            print(f"Error was detected while saving {config_file_name}: {str(e)}")



