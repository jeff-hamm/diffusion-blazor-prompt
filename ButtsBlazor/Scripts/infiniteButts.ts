/// <reference types="jquery" />
import * as butts from './types';
import * as utils from './utils';
import PageManager from './pageManager.js';
import defaultOptions from './options.js'
import ButtQueue  from "./buttQueue.js";



export default class InfiniteButts {
    $container: JQuery;
    options: butts.IButtsOptions;

    pageModes: PageManager;
    buttQueue: ButtQueue;
    refreshTimer: number;
    timer: number;
    isRunning: boolean;
    pageMode: butts.IPageMode;
    $loader: JQuery;
    $refreshBox: JQuery;
    $timer: JQuery;
    image: butts.IButtImage;
    $imageBox: JQuery;


    constructor($container: JQuery, options: butts.IButtsOptions) {
        this.options = $.extend({}, defaultOptions);
        this.option(options);
        this.$container = $container;
        this.buttQueue = new ButtQueue(this.options);
        this.pageModes = new PageManager(this);
        this.refreshTimer = this.options.refreshTimer;
        $(() => {
            this.$loader = $('<div id="spinner" class="spinner-border" role="status"></div>').prependTo($container);
            this.$refreshBox =
                $('<span class="lb-refresh"><span>Refresh: </span></div>').appendTo($('.lb-details')).hide();
            this.$timer = $('<span id="timer"></span>').appendTo(this.$refreshBox);
            this.$imageBox = $('#imageBox');
            $('.lb-nav,#lightbox,#lightboxOverlay')
                .off('click')
                .on('click', (e) => this._onClicked(e));
            this.loadPage();
        });
    }


    async loadPage(page?: string) {
        const mode = this.pageModes.setPage(page).mode;
        this.pageMode = mode;
        this.isRunning = true;
        await this.loop();
    }


    async loop() {
        clearTimeout(this.timer);
        let nextRefresh = this.refreshTimer;
        try {
            const next = await this.pageMode.next()
            if (next.page) {
                this.loadPage(next.page);
                return;
            }
            if (next.nextRefresh)
                nextRefresh = next.nextRefresh;
            await this.setButt(next)
        } finally {
            this.$loader.hide();
        }
        this.countdown(nextRefresh);
        this.timer = setTimeout(async () => {
            await this.loop();
        },
            nextRefresh);
    }

    countDownDate: Date;
    countDownInterval: number;
    countdown(timer: number) {
        this.countDownDate = new Date(new Date().getTime() + timer);

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


    async setButt(butt: butts.ButtPage) {
        if (!butt.page)
            butt.page = butt.data.index.toString();
        this.pageModes.setPage(butt.page);
        this.$imageBox.attr("data-title", butt.title);
        if (!this.image || butt.data.path !== this.image.path) {
            this.image = butt.data;
            this.$imageBox.attr('href', this.image.path);
            this.$imageBox.trigger('click');
        }
        return this.image;
    }



    async _onClicked(event: JQuery.ClickEvent) {
        if (this.pageModes.current?.mode) {
            this.pageModes.current.mode.clicked(event);
        }
    }
    
    option(options: butts.IButtsOptions) {
        $.extend(this.options, options);
    }

}

