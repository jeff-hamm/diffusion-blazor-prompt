import * as butts from '../types';
import * as utils from '../utils.js';
import DefaultMode from './defaultMode.js';
import ButtQueue from '../buttQueue';

export default class IndexMode extends DefaultMode {
    constructor(queue: ButtQueue) {
        super(queue);
        this.name = "index";
    }
    isPageMatch(page?: string) {
        return !isNaN(parseInt(page));
    }
    async first(): Promise<butts.ButtPage> {
        const index = parseInt(utils.getPageFromUrl(window.location.pathname));
        const url = this.queue.apiUrl + "/" + index.toString();
        const butt: butts.IButtImage = await utils.getImageFromUrl(url);
        return {
            data: butt,
            pageType: "butt",
            title: "Butt #" + butt.index.toString()
        }
    }
}
