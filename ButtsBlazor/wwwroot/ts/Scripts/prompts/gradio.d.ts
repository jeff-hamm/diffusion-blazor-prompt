import { Status } from "@gradio/client/src/types";
import { DotNet } from "@microsoft/dotnet-js-interop";
export interface CannyConfig {
    controlImgSize?: number;
    scale?: boolean;
    scaleUp?: boolean;
    crop?: boolean;
    scaleDimension?: string;
    cannyLow?: number;
    cannyHigh?: number;
}
export interface PromptConfig {
    numOutputs: number;
    imgSize?: number;
    numSteps?: number;
    controlScale?: number;
    cannyConfig: CannyConfig;
}
export interface ClientConfig {
    uriBase: string;
    hfToken?: string;
}
export declare function configure(uriBase: string, hfToken?: string): void;
export declare function toBlob(imageUrl: string | Blob): Promise<Blob>;
export declare function generateCanny(srcImage: string | Blob, data: CannyConfig, callbackObj?: DotNet.DotNetObject | StatusCallback): Promise<Blob>;
export declare function saveConfig(data: PromptConfig): Promise<PayloadResponse<any>>;
export declare function resetConfig(uriBase: string): Promise<PayloadResponse<any>>;
export declare function generate(srcImage: string, prompt: string, negative: string, data: PromptConfig, callbackObj?: DotNet.DotNetObject | StatusCallback): Promise<string[]>;
export declare function generatePromptImage(cannyImage: string | Blob, prompt: string, negative: string, data: PromptConfig, callbackObj?: DotNet.DotNetObject | StatusCallback): Promise<string[]>;
export interface PayloadResponse<T> {
    data: T[];
    fn_index?: number;
    event_data?: unknown;
    time?: Date;
}
type StatusCallback = (d: Status) => void;
declare const _default: {
    configure: typeof configure;
    generateCanny: typeof generateCanny;
    generatePromptImage: typeof generatePromptImage;
    generate: typeof generate;
    resetConfig: typeof resetConfig;
    saveConfig: typeof saveConfig;
    toBlob: typeof toBlob;
};
export default _default;
