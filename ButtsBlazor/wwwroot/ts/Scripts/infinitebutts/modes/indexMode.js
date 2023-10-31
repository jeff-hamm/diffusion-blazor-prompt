import * as utils from '../utils.js';
import DefaultMode from './defaultMode.js';
export default class IndexMode extends DefaultMode {
    constructor(queue) {
        super(queue);
        this.name = "index";
    }
    isPageMatch(page) {
        return !isNaN(parseInt(page));
    }
    async first() {
        const index = parseInt(utils.getPageFromUrl(window.location.pathname));
        const url = this.queue.apiUrl + "/" + index.toString();
        const butt = await utils.getImageFromUrl(url);
        return {
            data: butt,
            pageType: "butt",
            title: "Butt #" + butt.index.toString()
        };
    }
}
//# sourceMappingURL=indexMode.js.map