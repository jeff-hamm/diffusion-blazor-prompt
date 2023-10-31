import * as butts from '../types';
import ButtQueue from '../buttQueue';

export default class DefaultMode implements butts.IPageMode {
    queue: ButtQueue;
    name = "default";
    constructor(queue: ButtQueue) {
        this.queue = queue;
        this.name = "default";
    }
    isPageMatch(page?: string) {
        return page == null || page == "";
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

    clicked(event?: JQuery.TriggeredEvent): Promise<butts.ButtPage> {
        if ((<any>window)?.navigation?.canGoForward) {
            history.forward();
        }
        else
            return this.next();
    }
}
