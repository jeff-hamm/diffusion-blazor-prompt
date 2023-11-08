import InfiniteButts from './infiniteButtsFs';
import * as butts from './types';
export default class PageManager {
    owner: InfiniteButts;
    pageModes: butts.IPageMode[];
    current: butts.PageState;
    initialPage: string;
    constructor(owner: InfiniteButts);
    _onPopState(event: PopStateEvent): Promise<void>;
    setButtPage(buttPage?: butts.ButtPage): butts.PageState;
    setPage(page?: string, addToHistory?: boolean): butts.PageState;
}
