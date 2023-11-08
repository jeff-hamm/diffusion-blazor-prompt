import * as utils from './utils.js';
import PageManager from './pageManagerFs.js';
import defaultOptions from './options.js';
import ButtQueue from "./buttQueue.js";
export default class InfiniteButts {
    constructor($container, options) {
        this.options = $.extend({}, defaultOptions);
        this.option(options);
        this.$container = $container;
        this.buttQueue = new ButtQueue(this.options);
        this.pageModes = new PageManager(this);
        this.refreshTimer = this.options.refreshTimer;
        $(() => {
            $(document).keydown(e => this._onKeyDown(e));
            this.$loader = $('#spinner');
            this.$shareLink = $('#shareLink');
            this.$caption = $('#caption');
            //            Share(<HTMLElement>this.$shareLink[0])
            this.$refreshBox = $('#refresh-box').hide();
            this.$timer = $('#timer');
            this.$photo = $('#photo');
            //this.$imageBox = $('#imageBox')
            //if (this.$imageBox.attr('href')) { 
            //    this.$imageBox.trigger('click');
            //}
            this.$metaTag = $('meta[property=og\\:image]');
            $('#content').on('click', (e) => this._onClicked(e));
            this.loadPage(utils.getPageFromUrl(window.location.pathname));
        });
    }
    async loadPage(page) {
        const changed = page != this.pageModes.current?.pageName;
        const mode = this.pageModes.setPage(page).mode;
        if (changed || this.pageMode != mode) {
            this.pageMode = mode;
            await this.setPage(await this.pageMode.first());
        }
        else
            await this.loop();
    }
    async loop() {
        clearTimeout(this.timer);
        try {
            this.showLoader = true;
            //this.$loader.delay(250).show(250, () => {
            //    if (!this.showLoader)
            //        this.$loader.hide();
            //});
            const next = await this.pageMode.next();
            await this.setPage(next);
            await this.setPage(next);
        }
        finally {
            this.showLoader = false;
            this.$loader.hide();
        }
    }
    async setPage(next) {
        clearTimeout(this.timer);
        let nextRefresh = this.refreshTimer;
        if (next.page && !this.pageMode.isPageMatch(next.page)) {
            this.loadPage(next.page);
            return;
        }
        if (next.nextRefresh)
            nextRefresh = next.nextRefresh;
        await this.setButt(next);
        this.countdown(nextRefresh);
        this.isRunning = true;
        this.timer = setTimeout(async () => {
            await this.loop();
        }, nextRefresh);
    }
    countdown(timer) {
        this.countDownDate = new Date(new Date().getTime() + timer);
        if (!this.countDownInterval)
            this.startCountdown();
    }
    startCountdown() {
        if (!this.countDownInterval) {
            this.$refreshBox.show();
            // Update the count down every 1 second
            this.countDownInterval = setInterval(() => {
                // Get today's date and time
                const now = new Date().getTime();
                // Find the distance between now and the count down date
                const distance = this.countDownDate.getTime() - now;
                // Time calculations for days, hours, minutes and seconds
                let seconds = Math.floor(distance / 1000);
                if (seconds < 0)
                    seconds = 0;
                // Display the result in the element with id="demo"
                this.$timer.text(seconds + 's');
                // If the count down is finished, write some text
                this.$refreshBox.toggle(this.refreshTimer > 2000);
            }, 1000);
        }
    }
    stopCountdown() {
        if (this.countDownInterval) {
            clearInterval(this.countDownInterval);
            this.$refreshBox.hide();
            this.countDownInterval = null;
        }
    }
    async setButt(butt) {
        if (!butt.page)
            butt.page = butt.data.index.toString();
        this.pageModes.setButtPage(butt);
        this.$photo.attr("data-title", butt.title);
        this.$caption.text(butt.title);
        //        this.$imageBox.attr("data-title", butt.title);
        if (!this.image || butt.data.path !== this.image.path) {
            this.image = butt.data;
            this.$photo.attr('src', this.image.path);
            //            this.$imageBox.attr('href', this.image.path);
            this.$metaTag.attr('content', this.image.path);
            //            this.$imageBox.trigger('click');
        }
        return this.image;
    }
    async _onKeyDown(event) {
        if (event.key == "ArrowRight" || event.key == " " || event.key == "Spacebar") {
            await this.next(event);
        }
        else if (event.key == "ArrowLeft") {
            this.prev();
        }
    }
    async _onClicked(event) {
        await this.next(event);
    }
    prev() {
        if (this.lastClick && (new Date().getTime() - this.lastClick.getTime()) < this.options.maxNavSpeed)
            return;
        this.lastClick = new Date();
        history.back();
    }
    async next(event) {
        if (this.lastClick && (new Date().getTime() - this.lastClick.getTime()) < this.options.maxNavSpeed)
            return;
        this.lastClick = new Date();
        if (this.pageModes.current?.mode) {
            const page = await this.pageModes.current.mode.clicked(event);
            if (page) {
                await this.setPage(page);
            }
        }
    }
    option(options) {
        $.extend(this.options, options);
    }
}
//# sourceMappingURL=infiniteButtsFs.js.map