import * as butts from './types.js';
import * as utils from './utils.js';
import PageManager from './pageManagerFs.js';
import defaultOptions from './options.js'
import ButtQueue from "./buttQueue.js";
import Share from './share.js';
import QRCode from 'qrcode'
import IndexMode from './modes/indexMode.js';

export class PhotoSetter {
    constructor() {
        this.$photoContainer = $('#photoContainer');
        this.template = this.$photoContainer.find('img')[0].outerHTML;
    }
    title: string;
    template: string;
    $photoContainer: JQuery;
    $photoContainerB: JQuery;
    fadeTime: number = 1000;

    setPhotoTitle(title: string) {
        this.title = title;
    }
    setPhoto(path:string) {
        var $photo = $(this.template).attr('src', path).attr('data-title',this.title).data('title', this.title);
        var $children = this.$photoContainer.children('img')
        if($children.length > 1)
            $children.last().remove();
        this.$photoContainer.removeClass('fading');
        this.$photoContainer.prepend($photo);
        this.$photoContainer.addClass('fading');

    }
}

export default class InfiniteButts {
    $container: JQuery;
    options: butts.IButtsOptions;

    pageModes: PageManager;
    buttQueue: ButtQueue;
    refreshTimer: number;
    timer: NodeJS.Timeout;
    isRunning: boolean;
    pageMode: butts.IPageMode;
    $loader: JQuery;
    $refreshBox: JQuery;
    $timer: JQuery;
    $caption: JQuery;
    image: butts.IButtImage;
//    $imageBox: JQuery;
    $metaTag: JQuery;
    $shareLink: JQuery;
    photoSetter: PhotoSetter;
    $qrCode: JQuery;


    constructor($container: JQuery, options: butts.IButtsOptions) {
        this.options = $.extend({}, defaultOptions);
        this.option(options);
        this.$container = $container;
        this.buttQueue = new ButtQueue(this.options);
        this.pageModes = new PageManager(this);
        this.refreshTimer = this.options.refreshTimer;
        $(() => {
            $(document).keydown(e => this._onKeyDown(e))
            this.$loader = $('#spinner');
            this.$shareLink = $('#shareLink');
            this.$qrCode = $('#qrCode');
            this.$caption = $('#caption');
//            Share(<HTMLElement>this.$shareLink[0])
            this.$refreshBox =$('#refresh-box').hide();
            this.$timer = $('#timer');
            this.photoSetter = new PhotoSetter();
            //this.$imageBox = $('#imageBox')
            //if (this.$imageBox.attr('href')) { 
            //    this.$imageBox.trigger('click');
            //}
            this.$metaTag = $('meta[property=og\\:image]');
            $('#content').on('click', (e) => this._onClicked(e));
            this.loadPage(utils.getPageFromUrl(window.location.pathname));
        });
    }


    async loadPage(page?: string) {
        const changed = page != this.pageModes.current?.pageName;
        const mode = this.pageModes.setPage(page).mode;
        if (changed || this.pageMode != mode) {
            this.pageMode = mode;
            await this.setPage(await this.pageMode.first());
        }
        else await this.loop();
    }


    showLoader: boolean;
    async loop() {
        clearTimeout(this.timer);
        try {
            this.showLoader = true;
            //this.$loader.delay(250).show(250, () => {
            //    if (!this.showLoader)
            //        this.$loader.hide();
            //});
            const next = await this.pageMode.next()
            await this.setPage(next);
            await this.setPage(next);
        } finally {
            this.showLoader = false;
            this.$loader.hide();
        }
    }

    async setPage(next: butts.ButtPage) {
        clearTimeout(this.timer);
        let nextRefresh;
        if (next.data?.isLatest)
            nextRefresh = this.options.latestRefreshTimer;
        else if (this.pageMode?.name == IndexMode.ModeName) {
            nextRefresh = this.options.indexRefreshTimer;
        } else {
            nextRefresh = this.refreshTimer;
        }
        
        if (next.page && !this.pageMode.isPageMatch(next.page)) {
            this.loadPage(next.page);
            return;
        }
        if (next.nextRefresh)
            nextRefresh = next.nextRefresh;
        await this.setButt(next)
        if (nextRefresh > 0) {
            this.countdown(nextRefresh);
            this.isRunning = true;
            this.timer = setTimeout(async () => {
                await this.loop();
            }, nextRefresh);
        }

    }

    countDownDate: Date;
    countDownInterval: NodeJS.Timeout;
    countdown(timer: number) {
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

    getUrlForPath(butt: butts.IButtImage) : string {
        if(!butt.isLatest)
            return document.location.href;
        return new URL("/" + butt.index, document.location.href).href; 
        
    }

    async setButt(butt: butts.ButtPage) {
        if (!butt.page)
            butt.page = butt.data.index.toString();
        this.pageModes.setButtPage(butt);
        this.photoSetter.setPhotoTitle(butt.title);
        console.log('Generating QR code for ' +this.options.urlBase + butt.data.path);

        const data = await QRCode.toDataURL(
            this.getUrlForPath(butt.data),
            {
                margin: 1,
                scale: 4,
                color: {
                    dark:"#FF00FF",
                    light:"#D0D0D0"
                  }
            });
        this.$qrCode.attr('src', data);
        this.$caption.text(butt.title);
//        this.$imageBox.attr("data-title", butt.title);
        if (!this.image || butt.data.path !== this.image.path) {
            this.image = butt.data;
            this.photoSetter.setPhoto(this.image.path);
//            this.$imageBox.attr('href', this.image.path);
            this.$metaTag.attr('content', this.image.path);
            $('#content').addClass('loaded');
//            this.$imageBox.trigger('click');
        }
        return this.image;
    }


    async _onKeyDown(event: JQuery.KeyDownEvent) {
        if (event.key == "ArrowRight" || event.key == " " || event.key == "Spacebar") {
            await this.next(event);
        }
        else if (event.key == "ArrowLeft") {
            this.prev();
        }
    }

    async _onClicked(event: JQuery.ClickEvent) {
        await this.next(event);
    }
    lastClick: Date;
    prev() {
        if (this.lastClick && (new Date().getTime() - this.lastClick.getTime()) < this.options.maxNavSpeed)
            return;
        this.lastClick = new Date();
        history.back();
    }
    async next(event?: JQuery.TriggeredEvent) {
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
    
    option(options: butts.IButtsOptions) {
        $.extend(this.options, options);
    }

}

