export default class LatestMode {
    constructor(queue) {
        this.name = "latest";
        this.queue = queue;
    }
    isPageMatch(page) {
        return page == "latest";
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
        return this.next();
    }
}
//# sourceMappingURL=latestMode.js.map