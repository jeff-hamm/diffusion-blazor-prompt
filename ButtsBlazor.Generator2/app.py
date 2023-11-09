import gradio as gr
from prompt import PromptArgs, PromptPipeline,ScaleDimension, ControlArgs
from enum import Enum
import numpy as np


config = PromptArgs.read_config_file()
pipeline = PromptPipeline()

def load_pipeline(initButton):
    pipeline.load_pipeline();


def copyTo(args:ControlArgs,controlNetImgSize,scale,scaleUp,crop,scaleDimension,cannyLow, cannyHigh):
    if(controlNetImgSize is not None):
        args.img_size = controlNetImgSize
    if(scale is not None):
        args.scale = scale
    if(scaleUp is not None):
        args.scaleUp = scaleUp
    if(crop is not None):
        args.crop = crop
    if(scaleDimension is not None):
        args.scaleDimension = ScaleDimension[scaleDimension].value
    if(cannyLow is not None):
        args.low = cannyLow
    if(cannyHigh is not None):
        args.high = cannyHigh
def copyToPrompt(args:PromptArgs,numOutputs,imgSize,numSteps,controlScale):
    if(numOutputs is not None):
        args.numOutputs = numOutputs
    if(imgSize is not None):
        args.img_size = imgSize
    if(numSteps is not None):
        args.numSteps = numSteps
    if(controlScale is not None):
        args.controlScale = controlScale

def resetConfig():
    config = PromptArgs();
    config.save_config_file();

def saveConfig(imgSize,numOutputs,numSteps,controlScale,controlNetImgSize,scale,scaleUp,crop,scaleDimension,cannyLow, cannyHigh):
    copyTo(config.controlnet,controlNetImgSize,scale,scaleUp,crop,scaleDimension,cannyLow, cannyHigh)
    copyToPrompt(config,numOutputs,imgSize,numSteps,controlScale)
    config.save_config_file()

def canny(input_img,userImgSize,scale,scaleUp,crop,scaleDimension,cannyLow, cannyHigh):
    userConfig = ControlArgs(vars(config.controlnet).items())
    copyTo(userConfig,userImgSize,scale,scaleUp,crop,scaleDimension,cannyLow, cannyHigh)
    return pipeline.generate_canny(input_img,userConfig)

def generate_prompt(controlOutput,prompt,negative,numOutputs,promptImgSize,numSteps,controlScale,progress=gr.Progress()):
    if(not pipeline.is_loaded()):
        pipeline.load_pipeline()
    if(controlOutput is None):
        return None
    userConfig = PromptArgs(vars(config).items())
    copyToPrompt(userConfig,numOutputs,promptImgSize,numSteps,controlScale)
    images= pipeline.generatebutts(controlOutput,prompt,negative,userConfig,progress)
    for n in range(len(images), 6):
        images.append(None)
    return images[0],images[1],images[2],images[3],images[4],images[5],
    


with gr.Blocks() as demo:
    gr.Markdown("Enter Prompt")
    with gr.Tab("Prompt"):
        with gr.Group():
            with gr.Row():
                with gr.Group():
                    controlInput = gr.Image(label="Source",elem_id="controlnet_input")
                    with gr.Row() as cannyConfig:
                        userCannyLow = gr.Number(value=lambda: config.controlnet.low, label="Canny Low", precision=0, elem_id="userCannyLow")
                        userCannyHigh = gr.Number(value=lambda:config.controlnet.high, label="Canny High", precision=0, elem_id="userCannyHigh")
                    with gr.Accordion("Scaling", open=False) as scaleConfig:
                        with gr.Row():
                            userScale = gr.Checkbox(value=lambda: config.controlnet.scale, label="Scale?", elem_id="userScale")
                            userScaleUp = gr.Checkbox(value=lambda: config.controlnet.scaleUp, label="Scale Up?", elem_id="userScaleUp")
                            userCrop = gr.Checkbox(value=lambda: config.controlnet.crop, label="Crop?", elem_id="userCrop")
                        with gr.Row():
                            controlnetImgSize = gr.Number(value=lambda: config.controlnet.img_size, label="Target Size", precision=0, elem_id="controlnetImgSize")
                            userScaleDimension = gr.Dropdown(choices=[ScaleDimension.SHORT.name,ScaleDimension.LONG.name], value=lambda: ScaleDimension(config.controlnet.scaleDimension).name,label="Scale Dimension",elem_id="userScaleDimension")
                controlOutput = gr.Image(label="ControlNet Output", elem_id="controlnet_output")
            generateCanny = gr.Button(value="Run Canny",elem_id="gen_canny")
        with gr.Group():
            prompt = gr.Textbox(value=config.prompt, label="Prompt",elem_id="prompt")
            negative = gr.Textbox(value=config.negative, label="Negative Prompt",elem_id="negative")
            with gr.Row() as promptConfig:
                numOutputs = gr.Number(value= lambda: config.numOutputs, label="Num. Outputs",   precision=0, elem_id="num_outputs")
                promptImgSize = gr.Number(value= lambda: config.img_size, label="Image Size", precision=0, elem_id="prompt_img_size")
                numSteps = gr.Number(value= lambda: config.numSteps, label="Num. Steps",   precision=0, elem_id="num_steps")
                controlScale = gr.Number(value=lambda: config.controlScale, label="Control Scale", elem_id="controlnet_scale")
            generatePrompt = gr.Button(label="Generate",elem_id="gen_prompt")
        with gr.Group():
            promptOutputs = [gr.Image(label="Prompt Output "+str(i),elem_id="prompt_outputs" + str(i)) for i in range(6)]
            # promptOutput = gr.Gallery(6
            # label="Prompt Output", show_label=False, elem_id="prompt_outputs", 
            #     columns=[3], rows=[3], object_fit="contain", height="auto")
    with gr.Tab("Config") as configTab:
        configTab.add(cannyConfig)
        configTab.add(scaleConfig)
        configTab.add(promptConfig)
        with gr.Row():
            saveConfigButton = gr.Button(value="Save Config",elem_id="save_config")
            saveConfigButton.click(saveConfig, inputs=[promptImgSize,numOutputs,numSteps,controlScale,controlnetImgSize,userScale,userScaleUp,userCrop,userScaleDimension,userCannyLow,userCannyHigh] ,api_name="save_config")
            loadDefaultConfigButton = gr.Button(value="Reload Defaults",elem_id="reload_config")
            loadDefaultConfigButton.click(resetConfig,api_name="reset_config")
    cannyInputs=[controlInput,controlnetImgSize,userScale,userScaleUp,userCrop,userScaleDimension,userCannyLow,userCannyHigh]
    generateCanny.click(canny, inputs=cannyInputs, outputs=[controlOutput] ,api_name="generate_canny")
    generatePrompt.click(canny, inputs=cannyInputs, outputs=[controlOutput]).then(
            generate_prompt, 
            inputs=[controlOutput,prompt,negative,numOutputs,promptImgSize,numSteps,controlScale], 
            outputs=promptOutputs, api_name="generate_prompt")

    demo.load(lambda: load_pipeline(generatePrompt))
    
demo.queue(concurrency_count=2, max_size=2).launch(server_name="0.0.0.0", server_port=7860, debug=True)
