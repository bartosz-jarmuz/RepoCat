$(document).ready(function () {

    $.get("/Home/NavHeaderStats", function (data) {
        $("#NavHeaderStatsContainer").replaceWith(function () {
            return $(data).hide().fadeIn(500);
        });
    });

    intializeSelect2();

    $('.select2-inline').parent().find('.select2-container').addClass('inline-filter');
});


function intializeSelect2() {
    let t0 = performance.now();
    $('.select2').each(function () {
        let css = {};
        let width = 'style';
        if ($(this).hasClass('form-control-lg')) {
            css = {
                color: '#4d555d',
                'font-size': 'large'
            }
        }
        if ($(this).hasClass('condensed')) {
            width = 'auto';
            css = {
                display: 'inline-block',
                'font-size': 'small',
            }
        }

        $(this).select2({
            theme: 'bootstrap4',
            placeholder: $(this).attr('placeholder'),
            width: width ,
            allowClear: Boolean($(this).data('allow-clear')),
            containerCss: css,
        });
    });
    let t1 = performance.now();

    console.log("Initializing select boxes: " + (t1 - t0) + " milliseconds.");

}

