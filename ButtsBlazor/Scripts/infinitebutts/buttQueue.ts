import * as butts from './types.js';

export default class ButtQueue {
    preloadWorker: Worker;
    preloadCount: 0;
    targetSize: number;
    randomButtsQueue: butts.IButtImage[];
    latestButt: butts.IButtImage;
    latestButtShown: boolean = false;
    apiUrl: string;
    constructor(options:butts.IButtsOptions) {
        this.targetSize = options.preloadCount;
        this.randomButtsQueue = [];
        this.apiUrl = options.urlBase;
        const url = new URL("worker/loadButtsWorker.js", window.location.origin);
        this.preloadWorker = new Worker(url, { type: "module" });
        this.preloadWorker.onmessage = e => this._preloadWorkerMessage(e);
        this._requestButts();
    }

    async nextAsync(): Promise<butts.IButtImage> {
        return new Promise((resolve, reject) => this._nextLoop(resolve,reject))
    }

    
    loopTimeout: NodeJS.Timeout;
    async _nextLoop(resolve: (arg: butts.IButtImage) => void, reject: (reason: string) => void) {
        const nextButt = this.next();
        if (nextButt) {
            resolve(nextButt);
            return;
        }
        if(this.loopTimeout)
            clearTimeout(this.loopTimeout);
        this.loopTimeout = setTimeout(() => this._nextLoop(resolve, reject), 100);
    }

    latest(): butts.IButtImage {
        if (this.latestButt && !this.latestButtShown) {
            this.latestButtShown = true;
            return this.latestButt;
        }
        return null;
    }

    next(): butts.IButtImage {
        try {
            if (this.latestButt && !this.latestButtShown) {
                this.latestButtShown = true;
                return this.latestButt;
            }
            if (this.randomButtsQueue.length > 0) {
                return this.randomButtsQueue.shift();
            }
            return null;
        }
        finally {
            this._requestButts();
        }
    }

    _requestButts() {
        let request = this.targetSize;
        if (this.latestButt)
            request--;
        request -= this.randomButtsQueue.length;
        if (request > 0)
            this.preloadWorker.postMessage(<butts.RequestButtsMessage>{
                data: request,
                url: this.apiUrl,
                request: "request-butts"
            });
    }
    

    _preloadWorkerMessage(e: MessageEvent<butts.IButtImage>) {
        if (e.data.isLatest) { 
            this.latestButt = e.data;
            this.latestButtShown = false;
        }
        else
            this.randomButtsQueue.push(e.data);
    }


}