
export type PageState = {
    page: string;
    mode: IPageMode;
}
export type MapType = {
    [id: string]: any;
}

export type DefaultArgs = {
    known?: Date;
    except?: number;
}

export type RequestButtsMessage = {
    request: "request-butts";
    url?: string;
    data: number;
}
export type ConsumedButtsMessage = {
    request: "consume-butts";
    data: number;
}
export type WorkerRequestMessage = RequestButtsMessage | ConsumedButtsMessage;

export interface IButtsOptions {
    urlBase: string;
    refreshTimer: number;
    latestRefreshTimer: number;
    indexRefreshTimer: number;
    preloadCount: number;

}
export interface IButtImage {
    path: string;
    created: string;
    index: number;
    isLatest: boolean;
    createdDate: Date;
}

export class ButtImage implements IButtImage {
    path: string;
    created: string;
    index: number;
    isLatest: boolean;
    createdDate: Date;
    init() {
        if (this.created)
            this.createdDate = new Date(this.created);
    }
}


export interface IPageMode {
    name: string;
    isPageMatch(page?: string): boolean;
    next(): Promise<ButtPage>;
    clicked(event:JQuery.ClickEvent): Promise<void>;
}

export type ButtPage = {
    pageType: "butt";
    data: IButtImage;
    page?: string;
    title?: string;
    nextRefresh?: number;
}
