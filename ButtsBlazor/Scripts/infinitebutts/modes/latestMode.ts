import ButtQueue from '../buttQueue';
import * as butts from '../types';

export default class LatestMode implements butts.IPageMode {
    queue: ButtQueue;
    name = "latest";
    constructor(queue: ButtQueue) {
        this.queue = queue;
    }
    isPageMatch(page?: string) {
        return page == "latest";
    }
    first(): Promise<butts.ButtPage> {
        return this.next();
    }
    async next(): Promise<butts.ButtPage> {
        const butt = await this.queue.nextAsync();
        return {
            data: butt,
             pageType: "butt",
            title: butts.pageTitle(butt, this.queue.options)

        }
    }
    clicked(event: JQuery.ClickEvent): Promise<butts.ButtPage>  {
        return this.next();
    }
}
