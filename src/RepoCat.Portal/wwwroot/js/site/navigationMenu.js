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
