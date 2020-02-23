$(document).ready(function () {


    $(function () {
        $('[data-toggle="collapse"]').on('click', function () {
            var icon = $(this).find('.toggler-icon');
            if (icon.hasClass('icon-arrow-up')) {
                setArrowDown(icon); 
            } else { 
                setArrowUp(icon);
            }
        }); 
    });



}); 

function setArrowDown(icon) { 
    icon.removeClass('icon-arrow-up').addClass('icon-arrow-down');
}

function setArrowUp(icon) {
    icon.removeClass('icon-arrow-down').addClass('icon-arrow-up');
}
