$(document).ready(function () {

$(function () {
  $('[data-toggle="popover"]').popover()
})


    $('.nav-dropdown-toggle').click(function () {
        var itemText = $(this).text().trim();
        var cookie = getCookie('openNavItems');

        if (!$(this).parent().hasClass('open')) {
            if (cookie) {
                if (!isItemInArray(cookie, '_', itemText)) {
                    cookie += "_" + itemText
                    setCookie('openNavItems', cookie);
                }
            } else {
                setCookie('openNavItems', itemText);
            }
        } else {
            if (cookie) {
                var split = cookie.split('_');
                for (var i = 0; i < split.length; i++) {
                    if (split[i] === itemText) {
                        split.splice(i, 1);
                        i--;
                    }
                }
                var joint = split.join('_');
                setCookie('openNavItems', joint);
            }
        }
    });

    $.get("/Home/NavHeaderStats", function (data) { $("#NavHeaderStatsContainer").replaceWith(function() {
    return $(data).hide().fadeIn(500);
}); });
    




    var openItems = getCookie('openNavItems');
    if (openItems) {
        var split = openItems.split('_');
        $('.nav-dropdown-toggle').each(function () {
            if (split.includes($(this).text().trim())) {
                $(this).parent().addClass('open');
            }
        });
    }




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

});


function collapseAllToggle(sender) {
    if ($(sender).hasClass('open')) {
        $(sender).removeClass('open')
        $('div.collapse').removeClass('show');
        $(sender).text('Expand all')
    } else {
        $(sender).addClass('open')
        $('div.collapse').addClass('show');
        $(sender).text('Collapse all')

    }
}

function isItemInArray(input, separator, itemToFind) {
    if (input) {
        var split = input.split(separator);
        var exists = split.includes(itemToFind);
        return exists;
    }
    return false;
}

function getOrganizationFromKey(key) {
    return key.split(":")[0];
}
function getRepositoryFromKey(key) {
    return key.split(":")[1];
}



function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}
function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}
function eraseCookie(name) {
    document.cookie = name + '=; Max-Age=-99999999;';
}