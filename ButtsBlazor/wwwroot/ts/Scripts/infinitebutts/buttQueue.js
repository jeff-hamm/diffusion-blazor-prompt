export default class ButtQueue {
    constructor(options) {
        this.latestButtShown = false;
        this.targetSize = options.preloadCount;
        this.randomButtsQueue = [];
        this.apiUrl = options.urlBase;
        const url = new URL("worker/loadButtsWorker.js", window.location.origin);
        this.preloadWorker = new Worker(url, { type: "module" });
        this.preloadWorker.onmessage = e => this._preloadWorkerMessage(e);
        this._requestButts();
    }
    async nextAsync() {
        return new Promise((resolve, reject) => this._nextLoop(resolve, reject));
    }
    async _nextLoop(resolve, reject) {
        const nextButt = this.next();
        if (nextButt) {
            resolve(nextButt);
            return;
        }
        if (this.loopTimeout)
            clearTimeout(this.loopTimeout);
        this.loopTimeout = setTimeout(() => this._nextLoop(resolve, reject), 100);
    }
    latest() {
        if (this.latestButt && !this.latestButtShown) {
            this.latestButtShown = true;
            return this.latestButt;
        }
        return null;
    }
    next() {
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
            this.preloadWorker.postMessage({
                data: request,
                url: this.apiUrl,
                request: "request-butts"
            });
    }
    _preloadWorkerMessage(e) {
        if (e.data.isLatest) {
            this.latestButt = e.data;
            this.latestButtShown = false;
        }
        else
            this.randomButtsQueue.push(e.data);
    }
}
//# sourceMappingURL=buttQueue.js.map