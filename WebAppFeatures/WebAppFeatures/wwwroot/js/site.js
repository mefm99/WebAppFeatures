$(document).ready(() => {
    $('#Uploadbtn').hide();
    $('#fileUpload').change(function () {
        const file = this.files[0];
        console.log(file);
        if (file) {
            let reader = new FileReader();
            reader.onload = function (event) {
                console.log(event.target.result);
                $('#imgPreview').attr('src', event.target.result);
            }
            reader.readAsDataURL(file);
            $('#Uploadbtn').show();
        }
    });
    $('.navUser li').on("click",function () {
        $(this).addClass('active').siblings().removeClass('active');
    });
});
