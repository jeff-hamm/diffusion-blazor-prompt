class SlotWheel {
    constructor(//component: DotNet.DotNetObject,
    element) {
        this.itemHeight = 210;
        this.itemTime = 600;
        this.slowItemTime = 800;
        this.isSpinning = false;
        //        this.component = component;
        this.element = element;
        this.index = 0;
    }
    //dispose() {
    //    this.component = null;
    animation(ix, duration) {
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
    async cancel() {
        this.isSpinning = false;
        //        this.element.classList.add("cancelling");
        if (this.currentAnimation) {
            const cancelledAnimation = this.currentAnimation;
            const pct = cancelledAnimation.currentTime / this.itemTime;
            cancelledAnimation.updatePlaybackRate(.25);
            await cancelledAnimation.finished;
            cancelledAnimation.updatePlaybackRate(1);
        }
        return this.index;
    }
    initItems(items) {
        this.items = items.length;
        this.maxHeight = this.itemHeight * this.items;
        this.animations = [];
        this.itemTime = 500 + Math.random() * 400;
        for (var i = 0; i < this.items; i++) {
            this.animations.push(this.animation(i, this.itemTime));
            this.animations[0].persist();
        }
    }
    async spin(items) {
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
            animation.play();
            await animation.finished;
            this.index = (this.index + 1) % this.items;
        }
        //        this.element.classList.remove("spinning");
        //       this.element.classList.remove("cancelling");
        return (this.items - this.index) - 1;
    }
}
export function spin(element) {
    try {
        if (!element.wheel) {
            element.wheel = new SlotWheel(element);
        }
        return element.wheel;
    }
    catch (e) {
        console.error("Error creating wheel", element, e);
        return {
            spin: function () {
                console.info("Dummy spin func", element, e);
            },
            cancel: function () {
                console.info("Dummy cancel func", element, e);
            }
        };
    }
}
//# sourceMappingURL=prompt.js.map