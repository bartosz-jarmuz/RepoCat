$('.select2').each(function () {
    var css = {};
    if ($(this).hasClass('form-control-lg')) {
        css = {
            color: '#4d555d',
            'font-size': 'large'
        }
    }

    $(this).select2({
        theme: 'bootstrap4',
        placeholder: $(this).attr('placeholder'),
        width: 'style',
        allowClear: Boolean($(this).data('allow-clear')),
        containerCss: css
    });
});
