import { DotNet } from "@microsoft/dotnet-js-interop";

//interface Listener {
//    Component: DotNet.DotNetObject,
//    Method: string
//}
//const listeners: Map<string, Listener[]> = new Map<string, Listener[]>();

function serializeEvent(e: any) {
    if (e) {
        const clone: any = {}
        for (const key in e) {
            const val = e[key];
            if (typeof (val) !== "function" && typeof (val) !== "object" && typeof (val) !== "symbol")
                clone[key] = val;
        }
        return clone;
    }
};

//function eventHandler(eventName: string, e: any) {
//    const listenersList = listeners.get(eventName);
//    for (const listener of listenersList) {
//        listener.Component.invokeMethodAsync(listener.Method, serializeEvent(e));
//    }
//}
const listeners:ComponentEventListener[] = [];

export class ComponentEventListener implements EventListenerObject {
    constructor(componentRef: DotNet.DotNetObject, eventName: string, methodName: string) {
        this.componentRef = componentRef;
        this.eventName = eventName;
        this.methodName = methodName;
        this.isDisposed = false;
    }
    componentRef: DotNet.DotNetObject;
    eventName: string;
    methodName: string;
    isDisposed: boolean;
    id() {
        if (!this.componentRef) return "-1";
        return (<any>this.componentRef)._id;
    }
    attach() {
        console.debug("Attaching ", this.id())
        if (this.isDisposed) {
            console.info("Attempted to attach while disposed")
            return;
        }
        
        listeners.push(this);
        window.document.addEventListener(this.eventName, this);
    }
    detach() {
        console.debug("Detatching ", this.id(), this.isDisposed)
        if (this.isDisposed) return;
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
    handleEvent(e:any) {
        try {
            console.debug("handleEvent", this.id(), this.isDisposed)
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
export function addDocumentListener(componentRef: DotNet.DotNetObject, eventName: string, methodName: string) {
    const r = new ComponentEventListener(componentRef, eventName, methodName);
    r.attach();

    return r;
}

//export function addDocumentListener2(componentRef: DotNet.DotNetObject, eventName: string, methodName: string) {
//    const listener: Listener = {
//        Component: componentRef,
//        Method: methodName
//    };

//    let l = listeners.get(eventName);
//    if (typeof (l) == "undefined" || l === null) {
//        l = [listener];
//        listeners.set(eventName, l);
//        window.document.addEventListener(eventName, e => eventHandler(eventName,e));
//    } else {
//        l.push(listener);
//    }
//}

//export function removeDocumentListener(componentRef: DotNet.DotNetObject, eventName: string) {
//    let l = listeners.get(eventName);
//    if (typeof (l) != "undefined" && l !== null) {
//        const ix = l.findIndex((v) => (<any>v.Component)._id == (<any>componentRef)._id);
//        if (ix > -1) {
//            l.splice(ix, 1);
//            listeners.set(eventName, l);
//        }
//    }
//}