import { client } from "@gradio/client";
const clientConfig = {
    uriBase: "http://butts.infinitebutts.com",
};
export function configure(uriBase, hfToken) {
    clientConfig.uriBase = uriBase;
    clientConfig.hfToken = hfToken;
}
async function getClient(statusCallback) {
    return await client(clientConfig.uriBase, {
        hf_token: `hf_${clientConfig.hfToken}`,
        status_callback: statusCallback
    });
}
export async function toBlob(imageUrl) {
    if (typeof imageUrl != "string")
        return imageUrl;
    const response_0 = await fetch(imageUrl);
    return await response_0.blob();
}
export async function generateCanny(srcImage, data) {
    const srcImageBlob = await toBlob(srcImage);
    const app = await getClient();
    const args = [
        srcImageBlob, // blob in 'Source' Image component		
    ].concat(toArgs(data));
    const result = await app.predict("/generate_canny", args);
    console.log(result);
    return result.data[0];
}
function toArgs(data) {
    return [
        data?.controlImgSize ?? 768, // number  in 'Target Size' Number component		
        data?.scale ?? true, // boolean  in 'Scale?' Checkbox component		
        data?.scaleUp ?? true, // boolean  in 'Scale Up?' Checkbox component		
        data?.crop ?? true, // boolean  in 'Crop?' Checkbox component		
        data?.scaleDimension, // string (Option from: [('SHORT', 'SHORT'), ('LONG', 'LONG')]) in 'Scale Dimension' Dropdown component		
        data?.cannyLow ?? 100, // number  in 'Canny Low' Number component		
        data?.cannyHigh ?? 200, // number  in 'Canny High' Number component
    ];
}
export async function saveConfig(data) {
    const app = await getClient();
    const result = await app.predict("/save_config", [
        data?.numOutputs,
        data?.imgSize, // number  in 'Image Size' Number component
        data?.numSteps, // number  in 'Num. Steps' Number component
        data?.controlScale, // number  in 'Control Scale' Number component
    ].concat(toArgs(data.cannyConfig)));
    console.log(result);
    return result;
}
export async function resetConfig(uriBase) {
    const app = await getClient();
    const result = await app.predict("/reset_config", []);
    console.log(result);
    return result;
}
export async function generate(srcImage, prompt, negative, data) {
    const canny = await generateCanny(srcImage, data?.cannyConfig);
    return await generatePromptImage(canny, prompt, negative, data);
}
export async function generatePromptImage(cannyImage, prompt, negative, data) {
    const cannyBlob = await toBlob(cannyImage);
    const app = await getClient();
    try {
        const result = await app.predict('/generate_prompt', [
            cannyBlob, // blob in 'ControlNet Output' Image component		
            prompt, // string  in 'Prompt' Textbox component		
            negative, // string  in 'Negative Prompt' Textbox component		
            data?.numOutputs ?? 2,
            data?.imgSize ?? 1024, // number  in 'Image Size' Number component		
            data?.numSteps ?? 40, // number  in 'Num. Steps' Number component		
            data?.controlScale ?? 0.45, // number  in 'Control Scale' Number component
        ]);
        console.log(result);
        return result.data;
    }
    catch (e) {
        console.error(e);
    }
}
export default {
    configure,
    generateCanny,
    generatePromptImage,
    generate,
    resetConfig,
    saveConfig,
    toBlob
};
//# sourceMappingURL=gradio.js.map