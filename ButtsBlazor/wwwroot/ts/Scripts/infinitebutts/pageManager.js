import * as utils from './utils.js';
import DefaultMode from './modes/defaultMode.js';
import IndexMode from './modes/indexMode.js';
export default class PageManager {
    constructor(owner) {
        this.owner = owner;
        this.pageModes = pageModes.map(c => new c(owner.buttQueue));
        if (!this.pageModes.length)
            console.error('No pagemodes found');
        this.initialPage = utils.getPageFromUrl(window.location.pathname);
        window.onpopstate = event => this._onPopState(event);
    }
    async _onPopState(event) {
        if (event.state && event.state.pageName) {
            const state = this.setPage(event.state.pageName);
            this.owner.pageMode = state.mode;
            if (event.state.page)
                await this.owner.setPage(event.state.page);
            else
                await this.owner.setPage(await state.mode.next());
            return;
        }
        //        this.owner.loadPage(this.initialPage);
    }
    setButtPage(buttPage) {
        const pageName = buttPage?.page || '';
        let mode = this.pageModes.find(m => m.isPageMatch(pageName));
        if (!mode)
            mode = this.pageModes[0];
        this.current = { pageName, mode, page: buttPage };
        const urlPage = utils.getPageFromUrl(window.location.pathname);
        if (pageName !== urlPage) {
            history.pushState({ pageName, page: buttPage }, "", "/" + pageName);
        }
        return this.current;
    }
    setPage(page, addToHistory = true) {
        page || (page = '');
        let mode = this.pageModes.find(m => m.isPageMatch(page));
        if (!mode)
            mode = this.pageModes[0];
        this.current = { pageName: page, mode };
        const urlPage = utils.getPageFromUrl(window.location.pathname);
        if (addToHistory && page !== urlPage) {
            history.pushState({ pageName: this.current.pageName }, "", "/" + page);
        }
        return this.current;
    }
}
const pageModes = [DefaultMode, IndexMode];
//# sourceMappingURL=pageManager.js.map