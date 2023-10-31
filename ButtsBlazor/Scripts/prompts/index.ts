import '../'
import SSG from 'story-show-gallery/src/ssg.esm'
import 'story-show-gallery/dist/GridOverflow3D.min.css'
import 'story-show-gallery/dist/ssg.min.css';
//SSG.run({
//    noExit: false,
//    fs: false,
//    cfg: {
//        theme: 'black',
//        socialShare: false,
//        hideImgCaptions: true,
//        observeDOM: true,
//        showLandscapeHint: false,
//        rightClickProtection: false
//    }
//});
$('#SSG_menu').remove();
$(document).ready(() => {

    $('#SSG_menu').remove();
})

export * from './gradio';
export * from './documentListener';