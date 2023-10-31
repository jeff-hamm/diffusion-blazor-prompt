/// <reference types="node" />
import * as butts from './types.js';
export default class ButtQueue {
    preloadWorker: Worker;
    preloadCount: 0;
    targetSize: number;
    randomButtsQueue: butts.IButtImage[];
    latestButt: butts.IButtImage;
    latestButtShown: boolean;
    apiUrl: string;
    constructor(options: butts.IButtsOptions);
    nextAsync(): Promise<butts.IButtImage>;
    loopTimeout: NodeJS.Timeout;
    _nextLoop(resolve: (arg: butts.IButtImage) => void, reject: (reason: string) => void): Promise<void>;
    latest(): butts.IButtImage;
    next(): butts.IButtImage;
    _requestButts(): void;
    _preloadWorkerMessage(e: MessageEvent<butts.IButtImage>): void;
}
