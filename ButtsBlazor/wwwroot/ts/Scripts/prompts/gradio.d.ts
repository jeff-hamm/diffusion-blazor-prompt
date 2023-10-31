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
export declare function generateCanny(srcImage: string | Blob, data: CannyConfig): Promise<Blob>;
export declare function saveConfig(data: PromptConfig): Promise<unknown>;
export interface PredictReturn<T> {
    data: T[];
    endpoint: string;
    fn_index: number;
    time: object;
    type: string;
}
export declare function resetConfig(uriBase: string): Promise<unknown>;
export declare function generate(srcImage: string, prompt: string, negative: string, data: PromptConfig): Promise<string[]>;
export declare function generatePromptImage(cannyImage: string | Blob, prompt: string, negative: string, data: PromptConfig): Promise<string[]>;
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
