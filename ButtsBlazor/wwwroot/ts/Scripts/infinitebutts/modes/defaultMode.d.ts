/// <reference types="jquery" />
import * as butts from '../types';
import ButtQueue from '../buttQueue';
export default class DefaultMode implements butts.IPageMode {
    queue: ButtQueue;
    name: string;
    constructor(queue: ButtQueue);
    isPageMatch(page?: string): boolean;
    first(): Promise<butts.ButtPage>;
    next(): Promise<butts.ButtPage>;
    clicked(event?: JQuery.TriggeredEvent): Promise<butts.ButtPage>;
}
