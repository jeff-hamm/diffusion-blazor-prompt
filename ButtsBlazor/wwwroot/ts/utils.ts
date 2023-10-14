import * as butts from './types'

export function getPageFromUrl(pathName?:string): string  {
    pathName = pathName || '';
    if (pathName.startsWith("/"))
        pathName = pathName.substring(1);
    return pathName;
}
export async function getImageFromUrl(url: string): Promise<butts.IButtImage> {
    const img: butts.IButtImage = await makeRequest("GET", url, "json");
    if (img) {
        if (img.created)
            img.createdDate = new Date(img.created);
    }
    return img;
}
export function makeRequest(method: string, url: string, responseType: XMLHttpRequestResponseType): Promise<any> {
    return new Promise(function (resolve, reject) {
        const xhr = new XMLHttpRequest();
        xhr.responseType = responseType;
        xhr.open(method, url);
        xhr.onload = function () {
            if (this.status >= 200 && this.status < 300) {
                resolve(xhr.response);
            } else {
                reject({
                    status: this.status,
                    statusText: xhr.statusText
                });
            }
        };
        xhr.onerror = function () {
            reject({
                status: this.status,
                statusText: xhr.statusText
            });
        };
        xhr.send();
    });
}
