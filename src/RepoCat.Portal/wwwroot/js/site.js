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

function getOrganizationFromKey(key) {
    return key.split(":")[0];
}
function getRepositoryFromKey(key) {
    return key.split(":")[1];
}

$('.collapseAllToggle').click(function () {
    if ($(this).hasClass('open')) {
        $(this).removeClass('open')
        $('div.collapse').removeClass('show');
        $(this).text('Expand all')
    } else {
        $(this).addClass('open')
        $('div.collapse').addClass('show');
        $(this).text('Collapse all')

    }
});