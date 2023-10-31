import * as butts from '../types';
import DefaultMode from './defaultMode.js';
import ButtQueue from '../buttQueue';
export default class IndexMode extends DefaultMode {
    constructor(queue: ButtQueue);
    isPageMatch(page?: string): boolean;
    first(): Promise<butts.ButtPage>;
}
