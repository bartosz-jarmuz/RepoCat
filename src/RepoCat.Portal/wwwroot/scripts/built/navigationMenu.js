$(document).ready(function () {
    $('.nav-dropdown-toggle').click(function () {
        var itemText = $(this).text().trim();
        var cookie = getCookie('openNavItems');
        var dropdown = $(this).parent();
        if (!dropdown.hasClass('open')) {
            if (cookie) {
                if (!isItemInArray(cookie, '_+_', itemText)) {
                    cookie += "_+_" + itemText;
                    setCookie('openNavItems', cookie);
                }
            }
            else {
                setCookie('openNavItems', itemText);
            }
        }
        else {
            if (cookie) {
                var split = cookie.split('_+_');
                for (var i = 0; i < split.length; i++) {
                    if (split[i] === itemText) {
                        split.splice(i, 1);
                        i--;
                    }
                }
                var joint = split.join('_+_');
                setCookie('openNavItems', joint);
            }
        }
    });
    var openItems = getCookie('openNavItems');
    if (openItems) {
        var split = openItems.split('_+_');
        $('.nav-dropdown-toggle').each(function () {
            if (split.includes($(this).text().trim())) {
                $(this).parent().addClass('open');
            }
        });
    }
});
//# sourceMappingURL=navigationMenu.js.map