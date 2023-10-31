export default class DefaultMode {
    constructor(queue) {
        this.name = "default";
        this.queue = queue;
        this.name = "default";
    }
    isPageMatch(page) {
        return page == null || page == "";
    }
    first() {
        return this.next();
    }
    async next() {
        const butt = await this.queue.nextAsync();
        return {
            data: butt,
            pageType: "butt",
            title: butt.isLatest ? `Latest Butt! [#${butt.index}]` : "Random Butt #" + butt.index.toString()
        };
    }
    clicked(event) {
        if (window?.navigation?.canGoForward) {
            history.forward();
        }
        else
            return this.next();
    }
}
//# sourceMappingURL=defaultMode.js.map