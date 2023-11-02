import { DotNet } from "@microsoft/dotnet-js-interop";
export declare class ComponentEventListener implements EventListenerObject {
    constructor(componentRef: DotNet.DotNetObject, eventName: string, methodName: string);
    componentRef: DotNet.DotNetObject;
    eventName: string;
    methodName: string;
    isDisposed: boolean;
    id(): any;
    attach(): void;
    detach(): void;
    handleEvent(e: any): Promise<void>;
}
export declare function detachAll(): void;
export declare function addDocumentListener(componentRef: DotNet.DotNetObject, eventName: string, methodName: string): ComponentEventListener;
