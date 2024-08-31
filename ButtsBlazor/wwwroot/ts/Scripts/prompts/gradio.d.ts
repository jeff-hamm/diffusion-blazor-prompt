import { Status, PayloadMessage, Config } from "@gradio/client/src/types";
import { DotNet } from "@microsoft/dotnet-js-interop";
declare global {
    interface Window {
        __gradio_mode__: "app" | "website";
        gradio_config: Config;
        __is_colab__: boolean;
        __gradio_space__: string | null;
        gradio_api_info: any;
    }
}
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
export declare function saveConfig(data: PromptConfig): Promise<PayloadMessage>;
export declare function resetConfig(uriBase: string): Promise<PayloadMessage>;
export declare function generate(srcImage: string, prompt: string, negative: string, data: PromptConfig, callbackObj?: DotNet.DotNetObject | StatusCallback): Promise<string[]>;
export declare function generatePromptImage(cannyImage: string | Blob, prompt: string, negative: string, data: PromptConfig, callbackObj?: DotNet.DotNetObject | StatusCallback): Promise<string[]>;
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
