import * as butts from '../infinitebutts/types';
import * as utils from '../infinitebutts/utils.js';

interface ButtsWorkerState {
    requestedButts: number;
    suppliedButts: number;
    url: string;
    lastIndex: number;
    lastDate: string;
}

declare var self: DedicatedWorkerGlobalScope;

const state: ButtsWorkerState = {
    requestedButts: 0,
    suppliedButts: 0,
    url: '',
    lastIndex: null,
    lastDate: null,
};
let timeout: number;
let running = false;
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
    if (!running && state.suppliedButts < state.requestedButts) {
        if (timeout) { 
            clearTimeout(timeout);
        }
        timeout = setTimeout(runLoop, 0);
    }
};

async function runLoop() {
    running = true;
    try {
        while (state.suppliedButts < state.requestedButts) {
            try {
                const url = state.url + "?" + new URLSearchParams({
                    known: state.lastDate !== null ? state.lastDate : '',
                    except: state.lastIndex !== null ? state.lastIndex.toString() : ''
                }).toString();
                const img: butts.IButtImage = await utils.getImageFromUrl(url);
                if (img) {
                    state.lastIndex = img.index;
                    if (img.isLatest)
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
    }
    finally {
        running = false;
    }
    timeout = setTimeout(runLoop, 500);
}