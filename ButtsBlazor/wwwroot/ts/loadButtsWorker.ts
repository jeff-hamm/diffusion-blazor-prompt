// @ts-check
/// <reference no-default-lib="true"/>
/// <reference lib="ES2015" />
/// <reference lib="webworker" />

import * as butts from './types';
import * as utils from './utils';

// Using IIFE to provide closure to redefine `self`
(() => {
    // This is a little messy, but necessary to force type assertion
    // Same issue as in TS -> https://github.com/microsoft/TypeScript/issues/14877
    // prettier-ignore
    const self = /** @type {ServiceWorkerGlobalScope} */ (/** @type {unknown} */ (globalThis.self));


interface ButtsWorkerState {
    requestedButts: number;
    suppliedButts: number;
    url: string;
    lastIndex: number;
    lastDate: string;
}
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
                    known: state.lastDate,
                    except: state.lastIndex?.toString()
                }).toString();
                const img: butts.IButtImage = await utils.getImageFromUrl(url);
                if (img) {
                    state.lastIndex = img.index;
                    if(img.isLatest)
                        state.lastDate = img.created;
                    await utils.makeRequest("GET", img.path, "blob");
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