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

function collapseAllToggle(sender) {
    if ($(sender).hasClass('open')) {
        $('div.collapse').collapse('hide');
        $(sender).removeClass('open');
        $(sender).text('Expand all');
        setArrowDown($('[data-toggle="collapse"]').find('.toggler-icon'));
    } else {
        $('div.collapse').collapse('show');
        $(sender).addClass('open');
        $(sender).text('Collapse all');
        setArrowUp($('[data-toggle="collapse"]').find('.toggler-icon'));
    }
}
$(document).ready(function () {
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
            } else {
                //setCookie('openNavItems', itemText);
            }
        }
    });

    var openItems = getCookie('openNavItems');
    if (openItems) {
        var split = openItems.split('_');
        $('.nav-dropdown-toggle').each(function () {
            if (split.includes($(this).text().trim())) {
                $(this).parent().addClass('open');
            }
        });
    }

    function isItemInArray(input, separator, itemToFind) {
        if (input) {
            var split = input.split(separator);
            var exists = split.includes(itemToFind);
            return exists;
        }
        return false;
    }
});

$(document).ready(function () {

    $.get("/Home/NavHeaderStats", function (data) {
        $("#NavHeaderStatsContainer").replaceWith(function () {
            return $(data).hide().fadeIn(500);
        });
    });

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


function getOrganizationFromKey(key) {
    return key.split(":")[0];
}
function getRepositoryFromKey(key) {
    return key.split(":")[1];
}


$(document).ready(function () {

    $.get("/Home/NavHeaderStats", function (data) {
        $("#NavHeaderStatsContainer").replaceWith(function () {
            return $(data).hide().fadeIn(500);
        });
    });

    

    $('.sidebar-minimizer').on('click', function () {
        if ($('body').hasClass('brand-minimized sidebar-minimized')) {
            setCookie('sidebarOpen', 'true');
        } else {
            setCookie('sidebarOpen', 'false');
        }
    });
});


function getOrganizationFromKey(key) {
    return key.split(":")[0];
}
function getRepositoryFromKey(key) {
    return key.split(":")[1];
}

function trimStringMidsection(text) {
    if (text.length > 45) {
        var middlePoint = text.length / 2;
        var firstPart = text.substring(0, middlePoint);
        var secondPart = text.substring(middlePoint);
        return firstPart.substring(0, 20) + '(...)' + secondPart.substring(secondPart.length - 20);
    }
}
