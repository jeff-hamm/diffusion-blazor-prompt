import 'story-show-gallery/dist/ssg.min.css';
import InfiniteButts from "./infiniteButts.js";
import './infiniteButts.css';
import lightbox from 'lightbox2/dist/js/lightbox.js';
import 'lightbox2/dist/css/lightbox.min.css';
const infiniteButts = new InfiniteButts($(''), null);
lightbox.end = function () {
};
lightbox.option({
    'disableScrolling': true,
    'fadeDuration': 100,
    'imageFadeDuration': 100,
    'resizeDuration': 100,
    'wrapAround': false,
});
//SSG.run({
//    noExit: true,
//    fs: true,
//    cfg: {
//        theme: 'black',
//        socialShare: false,
//        hideImgCaptions: true,
//        observeDOM: true,
//        showLandscapeHint: false,
//        rightClickProtection: false
//    }
//});
//$('#SSG_menu').remove();
//$(document).ready(() => {
//    $('#SSG_menu').remove();
//})
export { infiniteButts, InfiniteButts };
//# sourceMappingURL=index.js.map