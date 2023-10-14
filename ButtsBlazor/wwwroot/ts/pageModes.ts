/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable @typescript-eslint/no-empty-function */


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
