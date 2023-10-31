export * from './documentListener';
import { DotNet } from "@microsoft/dotnet-js-interop";

class SlotWheel {
//    component?: DotNet.DotNetObject;
    element: HTMLElement;
    itemHeight: number = 210;
    itemTime: number = 300;
    slowItemTime: number = 800;
    items: number;
    index: number;
    maxHeight: number;
    animations: Animation[];
    constructor(//component: DotNet.DotNetObject,
        element: HTMLElement, items: string[]) {
        this.isSpinning = false;
//        this.component = component;
        this.element = element;
        this.items = items.length;
        this.index = 0;
        this.maxHeight = this.itemHeight * this.items;
        this.animations = [];
        for (var i = 0; i < this.items; i++) {
            this.animations.push(this.animation(i, this.itemTime + Math.random()*300));
            this.animations[0].persist();
        }
    }

    //dispose() {
    //    this.component = null;
    //}
    animation(ix: number, duration: number): Animation {
        const from = (ix * this.itemHeight) - this.maxHeight;
        const effect = new KeyframeEffect(this.element, [
            { transform: `translateY(${from}px)` },
            { transform: `translateY(${from + this.itemHeight}px)` }
        ], {
            duration: duration,
            iterations: 1,
            fill: "forwards",
            composite: 'replace'
        });
        return new Animation(effect);
    }
    isSpinning: boolean;
    currentAnimation: Animation;
    async cancel() {
        this.isSpinning = false;
//        this.element.classList.add("cancelling");
        if (this.currentAnimation) {
            const cancelledAnimation = this.currentAnimation;
            const pct = <number>cancelledAnimation.currentTime / this.itemTime;
            cancelledAnimation.updatePlaybackRate(.25);
            await cancelledAnimation.finished;
            cancelledAnimation.updatePlaybackRate(1);
        }
        return this.index;
    }
    async spin() {
        //    this.element.addEventListener("animationstart", e => this.onstart(e), false);
        //    this.element.addEventListener("animationend", e => this.onend(e), false);
        //    this.element.addEventListener("animationcancel", e => this.oncancel(e), false);
        //    this.element.addEventListener("animationiteration", e => this.oniteration(e), false);
        //    this.loopStart = 0;

        if (this.isSpinning) {
            console.log("Already spinning");
            return -1;
        }
        this.isSpinning = true;
//        this.element.classList.add("spinning");
        console.log("spin");
        while (this.isSpinning) {
            var animation = this.animation(this.index, this.itemTime);
            await animation.ready;
            animation.play();
            await animation.finished;
            this.index = (this.index + 1) % this.items;
        }

//        this.element.classList.remove("spinning");
 //       this.element.classList.remove("cancelling");
        return (this.items - this.index) - 1;
    }
    //}
    //loopStart: number;
    //private onstart(e: AnimationEvent) {
    //    this.loopStart = e.elapsedTime;
    //}
    //private oniteration(e: AnimationEvent) {
    //    this.loopStart = e.elapsedTime;
    //}
    //private onend(e: AnimationEvent) {
        
    //}
    //private oncancel(e: AnimationEvent) {

    //}

}

export function spin(element: HTMLElement, items: string[]) {
    try {
        return new SlotWheel(element, items);
    }
    catch (e) {
        console.error("Error creating wheel", element, e, items);
        return {
            spin: function () {
                console.info("Dummy spin func", element,e);
            },
            cancel: function () {
                console.info("Dummy cancel func", element, e);
            }
        }
    }
}