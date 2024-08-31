


export type PageState = {
    pageName: string;
    mode: IPageMode;
    page?: ButtPage;
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
    type?: string;
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
    maxNavSpeed: number;

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
    first(): Promise<ButtPage>;
    next(): Promise<ButtPage>;
    clicked(event: any): Promise<ButtPage>;
}

export type ButtPage = {
    pageType: "butt";
    data: IButtImage;
    page?: string;
    title?: string;
    nextRefresh?: number;
}
