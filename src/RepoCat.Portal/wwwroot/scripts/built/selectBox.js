$(document).ready(function () {
    $.get("/Home/NavHeaderStats", function (data) {
        $("#NavHeaderStatsContainer").replaceWith(function () {
            return $(data).hide().fadeIn(500);
        });
    });
    intializeSelect2();
    $('.select2-inline').parent().find('.select2-container').addClass('inline-filter');
});
function initializeDeferredSelect2() {
    var time = 200;
    setTimeout(function () {
        var t0 = performance.now();
        $('.select2-deferred').each(function () {
            // @ts-ignore
            var width = ($(this).width() + 26).toString();
            var css = {
                display: 'inline-block',
                'font-size': 'small',
            };
            $(this).select2({
                theme: 'bootstrap4',
                placeholder: $(this).attr('placeholder'),
                width: width,
                allowClear: false,
                containerCss: css,
            });
        });
        var t1 = performance.now();
        console.log("Drawing select boxes: " + (t1 - t0) + " milliseconds.");
    }, time);
}
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
        $(this).select2({
            theme: 'bootstrap4',
            placeholder: $(this).attr('placeholder'),
            width: width,
            allowClear: Boolean($(this).data('allow-clear')),
            containerCss: css,
            // @ts-ignore
            templateResult: function (data) {
                if (data.id && data.id.toString().endsWith(':*')) {
                    return $('<span style="font-style: italic">' + data.text + '</span>');
                }
                else {
                    return data.text;
                }
            },
            // @ts-ignore
            templateSelection: function (data) {
                if (data.id && data.id.toString().endsWith(':*')) {
                    return $('<span style="font-style: italic">' + data.text + '</span>');
                }
                else {
                    return data.text;
                }
            }
        });
    });
}
//# sourceMappingURL=selectBox.js.map