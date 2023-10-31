/// <reference types="jquery" />
import ButtQueue from '../buttQueue';
import * as butts from '../types';
export default class LatestMode implements butts.IPageMode {
    queue: ButtQueue;
    name: string;
    constructor(queue: ButtQueue);
    isPageMatch(page?: string): boolean;
    first(): Promise<butts.ButtPage>;
    next(): Promise<butts.ButtPage>;
    clicked(event: JQuery.ClickEvent): Promise<butts.ButtPage>;
}
