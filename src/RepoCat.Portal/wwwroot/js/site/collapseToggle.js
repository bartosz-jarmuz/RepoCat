$(document).ready(function () {
   

    $(function () {
        $('[data-toggle="collapse"]').click(function () {
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

function collapseAllToggle(sender) {
    if ($(sender).hasClass('open')) {
        $(sender).removeClass('open')
        $('div.collapse').removeClass('show');
        $(sender).text('Expand all')
        setArrowDown($('[data-toggle="collapse"]').find('.toggler-icon'));
    } else {
        $(sender).addClass('open')
        $('div.collapse').addClass('show');
        $(sender).text('Collapse all')
        setArrowUp($('[data-toggle="collapse"]').find('.toggler-icon'));
    }
}