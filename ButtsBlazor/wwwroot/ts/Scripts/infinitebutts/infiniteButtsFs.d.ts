/// <reference types="jquery" />
/// <reference types="jquery" />
/// <reference types="node" />
import * as butts from './types.js';
import PageManager from './pageManagerFs.js';
import ButtQueue from "./buttQueue.js";
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
    $metaTag: JQuery;
    $shareLink: JQuery;
    $photo: JQuery;
    constructor($container: JQuery, options: butts.IButtsOptions);
    loadPage(page?: string): Promise<void>;
    showLoader: boolean;
    loop(): Promise<void>;
    setPage(next: butts.ButtPage): Promise<void>;
    countDownDate: Date;
    countDownInterval: NodeJS.Timeout;
    countdown(timer: number): void;
    startCountdown(): void;
    stopCountdown(): void;
    setButt(butt: butts.ButtPage): Promise<butts.IButtImage>;
    _onKeyDown(event: JQuery.KeyDownEvent): Promise<void>;
    _onClicked(event: JQuery.ClickEvent): Promise<void>;
    lastClick: Date;
    prev(): void;
    next(event?: JQuery.TriggeredEvent): Promise<void>;
    option(options: butts.IButtsOptions): void;
}
