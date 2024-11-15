﻿import * as butts from '../types';
import * as utils from '../utils.js';
import DefaultMode from './defaultMode.js';
import ButtQueue from '../buttQueue';
import { pageTitle } from '../types';

export default class IndexMode extends DefaultMode {
    constructor(queue: ButtQueue) {
        super(queue);
        this.name = IndexMode.ModeName;
    }
    static ModeName = "index";
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
            title: pageTitle(butt, this.queue.options, "") 
        }
    }
}
