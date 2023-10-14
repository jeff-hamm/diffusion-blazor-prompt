/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable @typescript-eslint/no-empty-function */

import ButtQueue from './buttQueue';
import * as butts from './types';
import * as utils from './utils';

export class DefaultMode implements butts.IPageMode {
    queue: ButtQueue;
    name = "default";
    constructor(queue: ButtQueue) {
        this.queue = queue;
    }
    isPageMatch(page?: string) {
        return page == null || page == "";
    }
    async next(): Promise<butts.ButtPage> {
        const butt = await this.queue.nextAsync();
        return {
            data: butt,
             pageType: "butt",
             title: butt.isLatest ? `Latest Butt! [#${butt.index}]` : "Random Butt #" + butt.index.toString()

        }
    }

    async clicked(event: JQuery.ClickEvent): Promise<void>  {
        return null;
    }
}

const pageModes: (new (queue: ButtQueue) => butts.IPageMode)[] = [DefaultMode];
export default pageModes;



//    async showLatest(timer ?: number) {
//    this.refreshTimer = timer ? timer : this.options.latestRefreshTimer;
//    const url = this.options.urlBase + "/latest";
//    const data = () => {
//        if (this.lastCreated)
//            return {
//                "known": this.lastCreated
//            };
//        return null;
//    }
//    await this.loadButt(url, "Latest Butt!", data);
//}

//    async showDefault() {
//    this.refreshTimer = this.options.refreshTimer;
//    const url = this.options.urlBase;
//    const data = () => {
//        const d: butts.DefaultArgs = {};
//        if (this.image && this.lastCreated)
//            d.known = this.lastCreated;
//        if (this.image && this.image.index)
//            d.except = this.image.index;
//        return d;
//    }
//    const title = (image: butts.ButtImage) => {
//        if (image.isLatest)
//            return `Latest Butt! [#${image.index}]`;
//        else
//            return "Random Butt #" + image.index.toString();
//    }

//    await this.loadButt(url, title, data);

//    if (!this.firstPage)
//        this.firstPage = this.image.index;
//    else if (!history.state || this.image.index !== history.state.page) {
//        history.pushState({
//            'page': this.image.index,
//        },
//            "",
//            "/");
//    }

//}

//    async showNumber(number: number) {
//    if (number == null || (this.image && this.image.index === number)) {
//        history.pushState({
//            'page': null,
//        },
//            "",
//            "/");
//        await this.loadPage();
//        return;
//    }
//    this.refreshTimer = this.options.indexRefreshTimer;
//    const url = this.options.urlBase + "/" + number.toString();
//    const title = "Butt #" + number.toString();
//    await this.loadButt(url, title, null);
//}
