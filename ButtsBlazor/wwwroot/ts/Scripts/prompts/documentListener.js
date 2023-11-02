import { GamepadListener } from 'gamepad.js';
//interface Listener {
//    Component: DotNet.DotNetObject,
//    Method: string
//}
//const listeners: Map<string, Listener[]> = new Map<string, Listener[]>();
function serializeEvent(e) {
    if (e) {
        const clone = {};
        for (const key in e) {
            const val = e[key];
            if (typeof (val) !== "function" && typeof (val) !== "object" && typeof (val) !== "symbol")
                clone[key] = val;
        }
        return clone;
    }
}
;
//function eventHandler(eventName: string, e: any) {
//    const listenersList = listeners.get(eventName);
//    for (const listener of listenersList) {
//        listener.Component.invokeMethodAsync(listener.Method, serializeEvent(e));
//    }
//}
const listeners = [];
export class ComponentEventListener {
    constructor(componentRef, eventName, methodName) {
        this.componentRef = componentRef;
        this.eventName = eventName;
        this.methodName = methodName;
        this.isDisposed = false;
    }
    id() {
        if (!this.componentRef)
            return "-1";
        return this.componentRef._id;
    }
    attach() {
        console.debug("Attaching ", this.id());
        if (this.isDisposed) {
            console.info("Attempted to attach while disposed");
            return;
        }
        listeners.push(this);
        window.document.addEventListener(this.eventName, this);
    }
    detach() {
        console.debug("Detatching ", this.id(), this.isDisposed);
        if (this.isDisposed)
            return;
        this.isDisposed = true;
        window.document.removeEventListener(this.eventName, this);
        const ix = listeners.indexOf(this);
        if (ix > -1) {
            listeners.splice(ix, 1);
        }
        if (this.componentRef) {
            let ref = this.componentRef;
            this.componentRef = null;
            try {
                ref.dispose();
            }
            catch (e) {
                console.info("Error disposing component", ref);
            }
        }
    }
    async handleEvent(e) {
        try {
            console.debug("handleEvent", this.id(), this.isDisposed);
            if (this.isDisposed) {
                console.warn("A disposed instance recieved an event", e);
                return;
            }
            if (this.componentRef) {
                this.componentRef.invokeMethodAsync(this.methodName, serializeEvent(e));
            }
        }
        catch (e) {
            console.error(e);
            this.detach();
        }
    }
}
export function detachAll() {
    console.debug("detachAll", listeners, listeners.length);
    const listenersCopy = [...listeners];
    for (const listener of listenersCopy) {
        listener.detach();
    }
    console.debug("after detachAll", listeners, listeners.length);
}
export function addDocumentListener(componentRef, eventName, methodName) {
    const r = new ComponentEventListener(componentRef, eventName, methodName);
    r.attach();
    return r;
}
async function gamepadCode(code) {
    console.debug("gamepadCode", code);
    for (const l of listeners) {
        await l.handleEvent({
            code,
            key: code
        });
    }
}
const listener = new GamepadListener();
listener.on('gamepad:button', async (event) => {
    const { index, // Gamepad index: Number [0-3].
    button, // Button index: Number [0-N].
    value, // Current value: Number between 0 and 1. Float in analog mode, integer otherwise.
    pressed, // Native GamepadButton pressed value: Boolean.
    gamepad, // Native Gamepad object
     } = event.detail;
    if (!pressed)
        return;
    let code = "";
    switch (button) {
        // B
        case 1:
        // Y
        case 4:
            code = "Primary";
            break;
        // A
        case 0:
        // X
        case 3:
            code = "Secondary";
            break;
        // left shoulder
        case 6:
            code = "Left";
            break;
        // Select
        case 10:
            code = "Select";
            break;
        // right shoulder
        case 7:
            code = "Right";
        // Start
        case 11:
            code = "Start";
            break;
    }
    if (code)
        await gamepadCode(code);
});
listener.on('gamepad:axis', async (event) => {
    const { index, // Gamepad index: Number [0-3].
    axis, // Axis index: Number [0-N].
    value, // Current value: Number between -1 and 1. Float in analog mode, integer otherwise.
    gamepad, // Native Gamepad object
     } = event.detail;
    let code = "";
    if (value == 0)
        return;
    if (value == -1) {
        if (axis == 0) {
            code = "Left";
        }
        else if (axis == 1) {
            code = "Up";
        }
    }
    else if (value == 1) {
        if (axis == 0) {
            code = "Right";
        }
        else if (axis == 1) {
            code = "Down";
        }
    }
    if (code)
        await gamepadCode(code);
});
listener.start();
//# sourceMappingURL=documentListener.js.map