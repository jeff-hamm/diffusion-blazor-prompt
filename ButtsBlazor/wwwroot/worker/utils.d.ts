import * as butts from './types';
export declare function getPageFromUrl(pathName?: string): string;
export declare function getImageFromUrl(url: string): Promise<butts.IButtImage>;
export declare function makeRequest(method: string, url: string, responseType: XMLHttpRequestResponseType): Promise<any>;
