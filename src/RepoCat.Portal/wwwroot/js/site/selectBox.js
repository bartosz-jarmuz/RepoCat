﻿$(document).ready(function () {

    $.get("/Home/NavHeaderStats", function (data) {
        $("#NavHeaderStatsContainer").replaceWith(function () {
            return $(data).hide().fadeIn(500);
        });
    });

    intializeSelect2();

    $('.select2-inline').parent().find('.select2-container').addClass('inline-filter');
});

function initializeDeferredSelect2() {
    let time = 200;

    //setTimeout(function () {
    let t0 = performance.now();
    $('.card').on('mouseover','.select2-deferred', function (event) {
        // @ts-ignore
        let width = ($(this).width() + 26).toString();

        let css = {
            display: 'inline-block',
            'font-size': 'small',
        }

        $(this).select2({
            theme: 'bootstrap4',
            placeholder: $(this).attr('placeholder'),
            width: width,
            allowClear: false,
            containerCss: css,
        });
    });
    let t1 = performance.now();
    console.log("Drawing select boxes: " + (t1 - t0) + " milliseconds.");
    //}, time);
}

function intializeSelect2() {
    $('.select2').each(function () {
        let css = {};
        let width = 'style';
        if ($(this).hasClass('form-control-lg')) {
            css = {
                color: '#4d555d',
                'font-size': 'large'
            }
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
                } else {
                    return data.text;
                }
            },
            // @ts-ignore
            templateSelection: function (data) {
                if (data.id && data.id.toString().endsWith(':*')) {
                    return $('<span style="font-style: italic">' + data.text + '</span>');
                } else {
                    return data.text;
                }
            }
        });
    });

}

