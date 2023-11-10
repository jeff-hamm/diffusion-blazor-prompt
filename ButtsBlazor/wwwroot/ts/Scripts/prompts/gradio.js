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
export async function generateCanny(srcImage, data, callbackObj) {
    const srcImageBlob = await toBlob(srcImage);
    const args = [
        srcImageBlob, // blob in 'Source' Image component		
    ].concat(toArgs(data));
    const result = await run("/generate_canny", args, callbackObj);
    console.log(result);
    return result.data[0];
}
function toArgs(data) {
    return [
        data?.controlImgSize ?? 768,
        data?.scale ?? true,
        data?.scaleUp ?? true,
        data?.crop ?? true,
        data?.scaleDimension,
        data?.cannyLow ?? 100,
        data?.cannyHigh ?? 200, // number  in 'Canny High' Number component
    ];
}
export async function saveConfig(data) {
    const app = await getClient();
    const result = await run("/save_config", [
        data?.numOutputs,
        data?.imgSize,
        data?.numSteps,
        data?.controlScale, // number  in 'Control Scale' Number component
    ].concat(toArgs(data.cannyConfig)));
    console.log(result);
    return result;
}
//export interface PredictReturn<T> {
//	data: T[],
//	endpoint: string,
//	fn_index: number,
//	time: object,
//	type: string,
//	event_data: unknown
//}
export async function resetConfig(uriBase) {
    const app = await getClient();
    const result = await run("/reset_config", []);
    console.log(result);
    return result;
}
function toStatusCallback(callbackObj) {
    if (!callbackObj)
        return null;
    if (typeof callbackObj == "function")
        return callbackObj;
    return status => callbackObj.invokeMethodAsync("Callback", status);
}
function disposeStatusCallback(callbackObj) {
    if (!callbackObj)
        return;
    if (typeof callbackObj != "function")
        callbackObj.dispose();
}
export async function generate(srcImage, prompt, negative, data, callbackObj) {
    try {
        const canny = await generateCanny(srcImage, data?.cannyConfig, callbackObj);
        return await generatePromptImage(canny, prompt, negative, data, callbackObj);
    }
    finally {
        disposeStatusCallback(callbackObj);
    }
}
export async function generatePromptImage(cannyImage, prompt, negative, data, callbackObj) {
    const cannyBlob = await toBlob(cannyImage);
    try {
        const result = await run('/generate_prompt', [
            cannyBlob,
            prompt,
            negative,
            data?.numOutputs ?? 2,
            data?.imgSize ?? 1024,
            data?.numSteps ?? 40,
            data?.controlScale ?? 0.45, // number  in 'Control Scale' Number component
        ], callbackObj);
        console.log(result);
        return result.data;
    }
    catch (e) {
        console.error(e);
    }
}
async function run(endpoint, data, callbackObj, event_data) {
    const client = await getClient();
    let data_returned = false;
    let status_complete = false;
    let error_sent = false;
    return new Promise((res, rej) => {
        let app;
        let status_callback = toStatusCallback(callbackObj);
        try {
            app = client.submit(endpoint, data, event_data);
            let result;
            app.on("data", (d) => {
                // if complete message comes before data, resolve here
                if (status_complete) {
                    app.destroy();
                    disposeStatusCallback(status_callback);
                    status_callback = null;
                    res(d);
                }
                data_returned = true;
                result = d;
            })
                .on("status", (status) => {
                if (status_callback) {
                    status_callback(status);
                }
                if (status.stage === "error") {
                    error_sent = true;
                    disposeStatusCallback(status_callback);
                    status_callback = null;
                    rej(status);
                }
                if (status.stage === "complete") {
                    status_complete = true;
                    // if complete message comes after data, resolve here
                    if (data_returned) {
                        app.destroy();
                        disposeStatusCallback(status_callback);
                        status_callback = null;
                        res(result);
                    }
                }
            });
        }
        catch (e) {
            if (app && data_returned) {
                app.destroy();
                disposeStatusCallback(status_callback);
                status_callback = null;
            }
            if (!data_returned && !status_complete && !error_sent)
                rej(e);
        }
    });
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