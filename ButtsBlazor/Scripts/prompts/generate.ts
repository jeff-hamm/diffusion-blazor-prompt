import 'story-show-gallery/dist/ssg.min.css';
import SSG from 'story-show-gallery/src/ssg.esm'
export * from './gradio';

export function gallery() {
    (<any>SSG).jQueryImgSelector = "a[href$='.jpg'],a[href$='.jpeg'],a[href$='.JPG'],a[href$='.JPEG'],a[href$='.png'],a[href$='.PNG'],a[href$='.gif'],a[href$='.GIF'],a[href$='.webp'],a[href^='data']"
    SSG.run({
        noExit: true,
        fs: true,
        cfg: {
            theme: 'black',
            socialShare: false,
            hideImgCaptions: false,
            observeDOM: true,
            preferedCaptionLocation: 300,
            showLandscapeHint: false,
            rightClickProtection: false
        }
    });
    setTimeout(() => $('#SSG_menu').remove(), 100);
}

export function closeGallery() {
//    SSG.closeFullscreen();
    SSG.destroyGallery(null);
}