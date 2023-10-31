export * from './documentListener';
declare class SlotWheel {
    element: HTMLElement;
    itemHeight: number;
    itemTime: number;
    slowItemTime: number;
    items: number;
    index: number;
    maxHeight: number;
    animations: Animation[];
    constructor(//component: DotNet.DotNetObject,
    element: HTMLElement, items: string[]);
    animation(ix: number, duration: number): Animation;
    isSpinning: boolean;
    currentAnimation: Animation;
    cancel(): Promise<number>;
    spin(): Promise<number>;
}
export declare function spin(element: HTMLElement, items: string[]): SlotWheel | {
    spin: () => void;
    cancel: () => void;
};
