import * as $ from "jquery";
import Cropper from 'cropperjs/dist/cropper.min.js';
import 'cropperjs/dist/cropper.min.css'

function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            if (typeof e.target.result == "string") {
                var img = $('#photo');
                img.closest('form').addClass('has-image');
                img.attr('src', e.target.result);
                onImageSelected(img);
            }
            else
                console.error("Error", e);
        }

        reader.readAsDataURL(input.files[0]);
    }
} 
function clear() {
    var img = $('#photo');
    img.removeClass('cropper-hidden')
        .attr('src', null);

    img.closest('form')
        .removeClass('has-image')
    if(cropper)
        cropper.destroy();
    cropper = null;
}
const minCroppedWidth = 200;
const minCroppedHeight = 200;
function onCrop(event: Cropper.CropEvent<HTMLImageElement>) {
    var width = Math.round(event.detail.width);
    var height = Math.round(event.detail.height);
    if (
        width < minCroppedWidth
        || height < minCroppedHeight
    ) {
        event.currentTarget.cropper.setData({
            width: Math.max(minCroppedWidth, width),
            height: Math.max(minCroppedHeight, height),
        });
    }
}
function onFormSubmit(e: JQuery.SubmitEvent) {
    e.preventDefault();
    var formData = new FormData(this);
    $.ajax({
        type: "POST",
        url: "/ipad",
        data: formData,
        cache: false,
        contentType: false,
        processData: false
    }).then(function() {
        clear();
        $('#cameraInput').click();    
    }).fail(function () {
        clear();
        alert("Upload error. Try again");
    });

}
let isRotated = false;
let cropper;
let options: Cropper.Options<HTMLImageElement> = {
    aspectRatio: 2/3,
    viewMode: 2,
    crop: function(e) { onCrop(e)},
    dragMode: 'move',
    autoCropArea: 1,
    restore: false,
    modal: false,
    guides: true,
    highlight: true,
    cropBoxMovable: true,
    cropBoxResizable: true,
    toggleDragModeOnDblclick: true,
};
function rotateRatio() {
    if (isRotated) {
        isRotated = false;
        options.aspectRatio = 2 / 3;
    } else {
        isRotated = true;
        options.aspectRatio = 3 / 2;
    }
}
function onImageSelected(image:JQuery) {
    const imageEl = <HTMLImageElement>image.addClass('cropper-hidden')[0];
    options.aspectRatio = 2 / 3;
    isRotated = false;
    if (imageEl.naturalWidth < imageEl.naturalHeight) {
        rotateRatio();
    }
    cropper = new Cropper(imageEl, options);
}
function onRotateClicked() {
    rotateRatio();
    if(cropper)
        cropper.destroy();
    cropper = new Cropper(<HTMLImageElement>$('#photo')[0], options);
}

$(document).ready(() => {
    $("#cameraInput").change(function () {
        readURL(this);
    });
    $('#rotate').click(onRotateClicked);
    $('#photo-form').submit(onFormSubmit);
})
