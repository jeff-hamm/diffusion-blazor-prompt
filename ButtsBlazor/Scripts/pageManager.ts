import InfiniteButts from './infiniteButts.js';
import * as butts from './types.js';
import * as utils from './utils.js';
import pageModes from './pageModes.js';


export default class PageManager {
    owner: InfiniteButts;
    pageModes: butts.IPageMode[];
    current: butts.PageState;
    initialPage: string;
    constructor(owner: InfiniteButts) {
        this.owner = owner;
        this.pageModes = pageModes.map(c => new c(owner.buttQueue));
        if (!this.pageModes.length)
            console.error('No pagemodes found');
        this.initialPage = utils.getPageFromUrl(window.location.pathname);
        window.onpopstate = event => this._onPopState(event);
    }
    _onPopState(event:PopStateEvent) {
        this.owner.loadPage((event.state && event.state.page) || this.initialPage);
    }

    setPage(page?: string): butts.PageState {
        page ||= '';
        let mode = this.pageModes.find(m => m.isPageMatch(page))
        if (!mode)
            mode = this.pageModes[0];
        this.current = { page, mode }
        const urlPage = utils.getPageFromUrl(window.location.pathname);
        if (page !== urlPage) {
            history.pushState(this.current.page,"","/" + page);
        }
        return this.current;
    }
}