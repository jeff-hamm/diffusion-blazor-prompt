import { client, SpaceStatus } from "@gradio/client";
import { Status, Payload, LogMessage } from "@gradio/client/src/types";
import { DotNet } from "@microsoft/dotnet-js-interop";

export interface CannyConfig {
	controlImgSize?: number,
	scale?: boolean,
	scaleUp?: boolean,
	crop?: boolean,
	scaleDimension?: string,
	cannyLow?: number,
	cannyHigh?: number
}

export interface PromptConfig {
	numOutputs: number,
	imgSize?: number,
	numSteps?: number,
	controlScale?: number,
	cannyConfig: CannyConfig
}

type SpaceStatusCallback = (status: SpaceStatus) => void;
export interface ClientConfig {
	uriBase: string,
	hfToken?: string
}

const clientConfig:ClientConfig = {
	uriBase: "http://butts.infinitebutts.com",
} 
export function configure(uriBase: string, hfToken?:string) {
	clientConfig.uriBase = uriBase;
	clientConfig.hfToken = hfToken;
}

async function getClient(statusCallback?:SpaceStatusCallback) {
	return await client(clientConfig.uriBase, {
		hf_token: `hf_${clientConfig.hfToken}`,
		status_callback: statusCallback
	});
}

export async function toBlob(imageUrl: string | Blob): Promise<Blob> {
	if (typeof imageUrl != "string")
		return imageUrl;
	const response_0 = await fetch(imageUrl);
	return await response_0.blob();
}



export async function generateCanny(srcImage: string | Blob, data: CannyConfig, callbackObj?: DotNet.DotNetObject | StatusCallback): Promise<Blob> {
	const srcImageBlob = await toBlob(srcImage)
	const args = [
		srcImageBlob, 	// blob in 'Source' Image component		
	].concat(toArgs(data));
	const result = await run<Blob>("/generate_canny", args,callbackObj);

	console.log(result);
	return result.data[0];
}

function toArgs(data: CannyConfig) : any[] {
	return [
		data?.controlImgSize ?? 768, // number  in 'Target Size' Number component		
		data?.scale ?? true, // boolean  in 'Scale?' Checkbox component		
		data?.scaleUp ?? true, // boolean  in 'Scale Up?' Checkbox component		
		data?.crop ?? true, // boolean  in 'Crop?' Checkbox component		
		data?.scaleDimension, // string (Option from: [('SHORT', 'SHORT'), ('LONG', 'LONG')]) in 'Scale Dimension' Dropdown component		
		data?.cannyLow ?? 100, // number  in 'Canny Low' Number component		
		data?.cannyHigh ?? 200, // number  in 'Canny High' Number component
	]
}

export async function saveConfig(data: PromptConfig) {
	const app = await getClient();

	const result = await run<any>("/save_config", [
		data?.numOutputs,
	     data?.imgSize, // number  in 'Image Size' Number component
	     data?.numSteps, // number  in 'Num. Steps' Number component
		 data?.controlScale, // number  in 'Control Scale' Number component
		 ].concat(toArgs(data.cannyConfig))
	); 
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
export async function resetConfig(uriBase: string) {
	const app = await getClient();
	const result = await run<any>("/reset_config", []);

	console.log(result);
	return result;
}
function toStatusCallback(callbackObj?: DotNet.DotNetObject | StatusCallback): StatusCallback {
	if (!callbackObj) return null;
	if (typeof callbackObj == "function")
		return callbackObj;
	return status => callbackObj.invokeMethodAsync("Callback", status);
}
function disposeStatusCallback(callbackObj?: DotNet.DotNetObject | StatusCallback) {
	if (!callbackObj)
		return;
	if (typeof callbackObj != "function")
		callbackObj.dispose();
}
export async function generate(srcImage: string, prompt: string, negative: string, data: PromptConfig, callbackObj?: DotNet.DotNetObject | StatusCallback) {
	try {
		const canny = await generateCanny(srcImage, data?.cannyConfig, callbackObj);
		return await generatePromptImage(<Blob><any>canny, prompt, negative, data, callbackObj);
	}
	finally {
		disposeStatusCallback(callbackObj);
	}
}
export async function generatePromptImage(cannyImage: string | Blob, prompt: string, negative: string, data: PromptConfig, callbackObj?: DotNet.DotNetObject | StatusCallback): Promise<string[]> {
	const cannyBlob = await toBlob(cannyImage)
	try {
		const result = await run<string>('/generate_prompt', [
			cannyBlob, 	// blob in 'ControlNet Output' Image component		
			prompt, // string  in 'Prompt' Textbox component		
			negative, // string  in 'Negative Prompt' Textbox component		
			data?.numOutputs ?? 2,
			data?.imgSize ?? 1024, // number  in 'Image Size' Number component		
			data?.numSteps ?? 40, // number  in 'Num. Steps' Number component		
			data?.controlScale ?? 0.45, // number  in 'Control Scale' Number component
		],callbackObj);
		console.log(result);
		return result.data;
	}
	catch(e) {
		console.error(e);
	}
}
type SubmitEvent = { type: string; endpoint: string; fn_index: number };
export interface PayloadResponse<T> {
	data: T[];
	fn_index?: number;
	event_data?: unknown;
	time?: Date;
}

type StatusEvent = Status & SubmitEvent;
type DataEvent<T> = PayloadResponse<T> & SubmitEvent;
type LogEvent = LogMessage & SubmitEvent;
type StatusCallback = (d: Status) => void;
async function run<T>(
	endpoint: string,
	data: unknown[],
	callbackObj?: DotNet.DotNetObject | StatusCallback,
	event_data?: unknown): Promise<PayloadResponse<T>> {
	const client = await getClient();
	let data_returned = false;
	let status_complete = false;
	let error_sent = false;
	return new Promise((res, rej) => {
		let app;
		let status_callback = toStatusCallback(callbackObj);
		try {
			app = client.submit(endpoint, data, event_data);
			let result: PayloadResponse<T>;
			app.on("data", (d: DataEvent<T>) => {
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
				.on("status", (status: Status) => {
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
}