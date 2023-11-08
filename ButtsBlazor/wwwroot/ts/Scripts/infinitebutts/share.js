export default function Share(element) {
    const canShare = 'share' in window.navigator;
    if (!element || !element.dataset)
        return;
    const options = element.dataset.share.split(' ');
    const shareIndex = options.findIndex(option => { return option === 'device'; });
    const shareData = {
        'facebook': 'https://www.facebook.com/share.php?u=',
        'linkedin': 'https://www.linkedin.com/shareArticle?mini=true&url',
        'twitter': 'https://www.twitter.com/share?url='
    };
    if (shareIndex > -1 && !canShare) {
        options.splice(shareIndex, 1);
    }
    if (shareIndex > -1 && canShare) {
        const shareButton = h('button', {
            'aria-label': `${element.dataset.shareDevice}`,
            'data-share-item': ''
        }, [h('img', {
                'src': '/share.svg',
                'class': 'share-icon'
            })]);
        shareButton.addEventListener('click', () => {
            navigator.share({
                title: document.title,
                url: location.href
            }).catch(() => { return; });
        });
        element.appendChild(shareButton);
    }
    else {
        options.forEach(option => {
            const shareLink = h('a', {
                'aria-label': `${element.dataset["shareLabel"]} ${option}`,
                'data-share-item': option,
                'href': shareData[option] + encodeURIComponent(location.href),
                'rel': 'noopener noreferrer',
                'target': '_blank'
            }, [h('i')]);
            element.appendChild(shareLink);
        });
    }
}
function h(type, attributes, children = []) {
    const element = document.createElement(type);
    if (attributes) {
        for (const key in attributes) {
            element.setAttribute(key, attributes[key]);
        }
    }
    if (children.length) {
        children.forEach(child => {
            if (typeof child === 'string') {
                element.appendChild(document.createTextNode(child));
            }
            else {
                element.appendChild(child);
            }
        });
    }
    return element;
}
//# sourceMappingURL=share.js.map