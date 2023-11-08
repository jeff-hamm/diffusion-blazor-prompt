import { client, SpaceStatus } from "@gradio/client";
import { Status } from "@gradio/client/src/types";

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



export async function generateCanny(srcImage: string | Blob, data: CannyConfig): Promise<Blob> {
	const srcImageBlob = await toBlob(srcImage)
	const app = await getClient();
	const args = [
		srcImageBlob, 	// blob in 'Source' Image component		
	].concat(toArgs(data));
	const result = <PredictReturn<Blob>>await app.predict("/generate_canny", args);

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

	const result = await app.predict("/save_config", [
		data?.numOutputs,
	     data?.imgSize, // number  in 'Image Size' Number component
	     data?.numSteps, // number  in 'Num. Steps' Number component
		 data?.controlScale, // number  in 'Control Scale' Number component
		 ].concat(toArgs(data.cannyConfig))
	); 
	console.log(result);
	return result;
}

export interface PredictReturn<T> {
	data: T[],
	endpoint: string,
	fn_index: number,
	time: object,
	type: string,
	event_data: unknown
}
export async function resetConfig(uriBase: string) {
	const app = await getClient();
	const result = await app.predict("/reset_config", []);

	console.log(result);
	return result;
}
export async function generate(srcImage: string, prompt: string, negative: string, data: PromptConfig) {
	const canny = await generateCanny(srcImage, data?.cannyConfig);
	return await generatePromptImage(<Blob><any>canny, prompt, negative, data);
}
export async function generatePromptImage(cannyImage: string |Blob,prompt:string,negative:string,data:PromptConfig): Promise<string[]> {
	const cannyBlob = await toBlob(cannyImage)
	const app = await getClient();
	try {
		const result = <PredictReturn<string>>await app.predict('/generate_prompt', [
			cannyBlob, 	// blob in 'ControlNet Output' Image component		
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
	catch(e) {
		console.error(e);
	}
}

async function run<T>(
	endpoint: string,
	data: unknown[],
	event_data?: unknown,
	status_callback?: (d: Status) => void
): Promise<PredictReturn<T>> {
	const client = await getClient();
	let data_returned = false;
	let status_complete = false;
	return new Promise((res, rej) => {
		const app = client.submit(endpoint, data, event_data);
		let result: PredictReturn<T>;

		app
			.on("data", (d) => {
				// if complete message comes before data, resolve here
				const r: PredictReturn<T> = {
					data: <T[]>d.data,
					endpoint: d.endpoint,
					fn_index: d.fn_index,
					event_data: d.event_data,
					time: d.time,
					type: d.type,

				}
				if (status_complete) {
					app.destroy();
					res(r);
				}
				data_returned = true;
				result = r;
			})
			.on("status", (status) => {
				if (status.stage === "error") rej(status);
				if (status.stage === "complete") {
					status_complete = true;
					// if complete message comes after data, resolve here
					if (data_returned) {
						app.destroy();
						res(result);
					}
				}
				if (status_callback) {
					status_callback(status);
				}
			});
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