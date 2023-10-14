//@ts-check
/// <reference no-default-lib="true"/>
/// <reference lib="ES2020" />
/// <reference lib="webworker" />
import * as butts from './types';
function makeRequest(method:string, url:string, responseType:XMLHttpRequestResponseType):Promise<any> {
    return new Promise(function (resolve, reject) {
        const xhr = new XMLHttpRequest();
        xhr.responseType = responseType;
        xhr.open(method, url);
        xhr.onload = function () {
            if (this.status >= 200 && this.status < 300) {
                resolve(xhr.response);
            } else {
                reject({
                    status: this.status,
                    statusText: xhr.statusText
                });
            }
        };
        xhr.onerror = function () {
            reject({
                status: this.status,
                statusText: xhr.statusText
            });
        };
        xhr.send();
    });
}

interface ButtsWorkerState {
    requestedButts: number;
    suppliedButts: number;
    url: string;
    lastIndex: number;
    lastDate: string;
}
// Using IIFE to provide closure to redefine `self`
(() => {
    // This is a little messy, but necessary to force type assertion
    // Same issue as in TS -> https://github.com/microsoft/TypeScript/issues/14877
    // prettier-ignore
    const self = /** @type {ServiceWorkerGlobalScope} */ (/** @type {unknown} */ (globalThis.self));
    const state: ButtsWorkerState = {
        requestedButts: 0,
        suppliedButts: 0,
        url: '',
        lastIndex: null,
        lastDate: null,
    };
    let timeout: number;
    self.onmessage = async (e: MessageEvent<butts.WorkerRequestMessage>) => {
        switch (e.data.request) {
            case "request-butts": 
                state.requestedButts = e.data.data;
                state.suppliedButts = 0;
                state.url = e.data.url || state.url;
                break;
        //    case "consume-butts":
        //        state.suppliedButts -= e.data.data; 
        //        if (state.suppliedButts < 0)
        //            state.suppliedButts = 0;
        //        break;
        }
        if (state.requestedButts < state.suppliedButts) {
            if (timeout)
                clearTimeout(timeout);
            timeout = setTimeout(runLoop, 0);
        }
    };

    async function runLoop() {
        while (state.suppliedButts < state.requestedButts) {
            try {
                const url = state.url + "?" + new URLSearchParams({
                    known: state.lastDate?.toString(),
                    except: state.lastIndex?.toString()
                }).toString();
                const img: butts.IButtImage = await makeRequest("GET", url, "json");
                if (img) {
                    if (img.created)
                        img.createdDate = new Date(img.created);
                    state.lastIndex = img.index;
                    if(img.isLatest)
                        state.lastDate = img.created;
                    await makeRequest("GET", img.path, "blob");
                    state.suppliedButts++;
                    self.postMessage(<butts.IButtImage>img);

                } 
            }
            catch (e) {
                console.error("Error processing request", e);
                break;
            }
        }
        timeout = setTimeout(runLoop, 500);
    }
})();