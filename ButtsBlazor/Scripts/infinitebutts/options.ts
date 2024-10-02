import * as butts from './types';

const defaultOptions: butts.IButtsOptions = {
    urlBase: "/api/butts",
    refreshTimer: 15 * 1000,
    latestRefreshTimer: 45 * 1000,
    indexRefreshTimer: 45 * 1000,
    preloadCount: 5,
    maxNavSpeed: 100
};

export default defaultOptions;
