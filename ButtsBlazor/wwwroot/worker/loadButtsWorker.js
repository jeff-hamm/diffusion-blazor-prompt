import * as utils from './utils.js';
const state = {
    requestedButts: 0,
    suppliedButts: 0,
    url: '',
    lastIndex: null,
    lastDate: null,
};
let timeout;
let running = false;
self.onmessage = async (e) => {
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
                const img = await utils.getImageFromUrl(url);
                if (img) {
                    state.lastIndex = img.index;
                    if (img.isLatest)
                        state.lastDate = img.created;
                    await utils.makeRequest("GET","../"+ img.path, "blob");
                    state.suppliedButts++;
                    self.postMessage(img);
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
//# sourceMappingURL=loadButtsWorker.js.map