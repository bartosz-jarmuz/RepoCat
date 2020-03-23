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
    $('.select2').each(function () {
        var css = {};
        var width = 'style';
        if ($(this).hasClass('form-control-lg')) {
            css = {
                color: '#4d555d',
                'font-size': 'large'
            };
        }
        if ($(this).hasClass('condensed')) {
            width = 'resolve';
            css = {
                display: 'inline-block',
                'font-size': 'small',
            };
        }
        $(this).select2({
            theme: 'bootstrap4',
            placeholder: $(this).attr('placeholder'),
            width: width,
            allowClear: Boolean($(this).data('allow-clear')),
            containerCss: css,
        });
    });
}
//# sourceMappingURL=selectBox.js.map