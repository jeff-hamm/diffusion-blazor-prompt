import { DotNet } from "@microsoft/dotnet-js-interop";

class SlotWheel {
//    component?: DotNet.DotNetObject;
    element: HTMLElement;
    itemHeight: number = 210;
    itemTime: number = 600;
    slowItemTime: number = 800;
    items: number;
    index: number;
    maxHeight: number;
    animations: Animation[];
    constructor(//component: DotNet.DotNetObject,
        element: HTMLElement) {

        this.isSpinning = false;
//        this.component = component;
        this.element = element;
        this.index = 0;
    }

    //dispose() {
    //    this.component = null;
    
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
    async dispose() {
        this.isSpinning = false;
        if (this.currentAnimation)
            this.currentAnimation.cancel();
        for (var i = 0; i < this.animations.length; i++) 
            this.animations[i].cancel();
        (<any>this.element).wheel = null;            
    }
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
    initItems(items: string[]) {
        this.items = items.length;
        this.maxHeight = this.itemHeight * this.items;
        this.animations = [];
        this.itemTime = 750;
//        this.itemTime = 600 + Math.random() * 300
        for (var i = 0; i < this.items; i++) {
            this.animations.push(this.animation(i, this.itemTime));
            this.animations[0].persist();
        }

    }
    async spin(items: string[]) {
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
        this.initItems(items);
//        this.element.classList.add("spinning");
        console.log("spin");
        while (this.isSpinning) {
            var animation = this.animation(this.index, this.itemTime);
            await animation.ready;
            if(this.isSpinning)
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
function dummyWheel(element) {
    return {
        spin: function () {
            console.info("Dummy spin func", element);
        },
        dispose: function () {
            console.info("Dummy dispose func", element);
        },
        cancel: function () {
            console.info("Dummy cancel func", element);
        }
    }
}

export function spin(element: HTMLElement) {
    try {
        if (!element)
            return dummyWheel(element);
        if (!(<any>element).wheel) {
            (<any>element).wheel = new SlotWheel(element);
        }
        return (<any>element).wheel;
    }
    catch (e) {
        console.error("Error creating wheel", element);
        return dummyWheel(element);
    }
}