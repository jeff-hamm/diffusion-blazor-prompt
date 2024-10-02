var $ = window.$;
Cropper = window.Cropper;
const FORCE_RATIO = true;
const RATIO = 2.5/3.5;
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
        };
        reader.readAsDataURL(input.files[0]);
    }
}
function clear() {
    var img = $('#photo');
    img.removeClass('cropper-hidden')
        .attr('src', null);
    img.closest('form')
        .removeClass('has-image');
    if (cropper)
        cropper.destroy();
    cropper = null;
}
const minCroppedWidth = 200;
const minCroppedHeight = 200;
function onCrop(event) {
    var width = Math.round(event.detail.width);
    var height = Math.round(event.detail.height);
    if (width < minCroppedWidth
        || height < minCroppedHeight) {
        event.currentTarget.cropper.setData({
            width: Math.max(minCroppedWidth, width),
            height: Math.max(minCroppedHeight, height),
        });
    }
}
function onFormSubmit(e) {
    e.preventDefault();
    var formData = new FormData(this);
    $photoForm.removeClass('loaded');
    $.ajax({    
        type: "POST",
        url: "/camera",
        data: formData,
        cache: false,
        contentType: false,
        processData: false
    }).then(function (data) {
        if (data?.location) {
            window.location.href = data.location;
            return;
        }
        clear();
        $cameraInput.click();
    }).fail(function () {
        clear();
        alert("Upload error. Try again");
    }).always(function () {
        $photoForm.addClass("loaded");
    });
}
let isRotated = false;
let cropper;
let options = {
    viewMode: 3,
    crop: function (e) { onCrop(e); },
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
if (FORCE_RATIO) {
    options.aspectRatio = RATIO
}
function rotateRatio() {
    if (isRotated) {
        isRotated = false;
        if(FORCE_RATIO)
            options.aspectRatio = RATIO;
    }
    else {
        isRotated = true;
        if (FORCE_RATIO)
            options.aspectRatio = 1 / RATIO;
    }
}
function onImageSelected(image) {
    const imageEl = image.addClass('cropper-hidden')[0];
    if (FORCE_RATIO)
        options.aspectRatio = RATIO;
    isRotated = false;
    if (imageEl.naturalWidth < imageEl.naturalHeight) {
        rotateRatio();
    }
    cropper = new Cropper(imageEl, options);
}
function onRotateClicked() {
    rotateRatio();
    if (cropper)
        cropper.destroy();
    cropper = new Cropper($('#photo')[0], options);
}
let $photoForm;
let $cameraInput;
$(document).ready(() => {
    $photoForm = $('#photo-form');
    $cameraInput = $("#cameraInput").change(function () {
        readURL(this);
    });
    $('#rotate').click(onRotateClicked);
    $('#photo-form').submit(onFormSubmit);
});
//# sourceMappingURL=ipad.js.map