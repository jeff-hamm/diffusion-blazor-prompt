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
             title: butt.isLatest ? `Latest Butt! [#${butt.index}]` : "Random Butt #" + butt.index.toString()

        }
    }
    clicked(event: JQuery.ClickEvent): Promise<butts.ButtPage>  {
        return this.next();
    }
}
