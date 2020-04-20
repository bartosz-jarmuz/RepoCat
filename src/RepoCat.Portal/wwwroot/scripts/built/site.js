$(document).ready(function () {
    $.get("/Home/NavHeaderStats", function (data) {
        $("#NavHeaderStatsContainer").replaceWith(function () {
            return $(data).hide().fadeIn(500);
        });
    });
    $('.sidebar-minimizer').on('change', function () {
        setTimeout(function () {
            if ($('body').hasClass('brand-minimized sidebar-minimized')) {
                setCookie('sidebarOpen', 'true');
            }
            else {
                setCookie('sidebarOpen', 'false');
            }
        }, 200);
    });
    $(document).on('click', '.share-link', function () {
        var btn = $(this);
        // @ts-ignore
        btn.popover();
        navigator.clipboard.writeText(document.location.href).then(function () {
            btn.attr('data-content', 'Link copied to clipboard!');
            // @ts-ignore
            btn.popover('show');
        }, function () {
            btn.attr('data-content', 'Failed to copy link to clipboard. Copy it manually :(');
            // @ts-ignore
            btn.popover('show');
        });
    });
    attachShowMoreTagsHandlers();
});
function attachShowMoreTagsHandlers() {
    var selector = '.show-more-link.show-tags';
    $(selector).off('click.rc.links');
    $(selector).on('click.rc.links', function () {
        $(this).next('.tags-list').show();
        $(this).hide();
    });
}
function getOrganizationFromKey(key) {
    return key.split(":")[0];
}
function getRepositoryFromKey(key) {
    return key.split(":")[1];
}
function isItemInArray(input, separator, itemToFind) {
    if (input) {
        var split = input.split(separator);
        var exists = split.includes(itemToFind);
        return exists;
    }
    return false;
}
function trimStringMidsection(text) {
    return text;
    //if (text.length > 45) {
    //    var middlePoint = text.length / 2;
    //    var firstPart = text.substring(0, middlePoint);
    //    var secondPart = text.substring(middlePoint);
    //    return firstPart.substring(0, 20) + '(...)' + secondPart.substring(secondPart.length - 20);
    //}
    //return undefined;
}
function addToCollectionDictionaryCookie(cookieName, dictionaryKey, valueToAddToCollection) {
    var cookie = getCookie(cookieName);
    if (cookie) {
        var dictionary = void 0;
        try {
            dictionary = JSON.parse(cookie);
        }
        catch (error) {
            //ok, cookie corrupted, create new
            dictionary = {};
        }
        var collection = dictionary[dictionaryKey];
        if (!collection) {
            var collection_1 = [];
            collection_1.push(valueToAddToCollection);
            dictionary[dictionaryKey] = collection_1;
        }
        else {
            if (!collection.some(function (c) { return c === valueToAddToCollection; })) {
                collection.push(valueToAddToCollection);
            }
        }
        setCookie(cookieName, JSON.stringify(dictionary));
    }
    else {
        var dictionary = {};
        var collection_2 = [];
        collection_2.push(valueToAddToCollection);
        dictionary[dictionaryKey] = collection_2;
        setCookie(cookieName, JSON.stringify(dictionary));
    }
}
function getFromCollectionDictionaryCookie(cookieName, dictionaryKey) {
    var cookie = getCookie(cookieName);
    if (cookie) {
        var dictionary;
        try {
            dictionary = JSON.parse(cookie);
        }
        catch (error) {
            //ok, cookie corrupted, ignore it
            return;
        }
        return dictionary[dictionaryKey];
    }
}
function removeFromCollectionDictionaryCookie(cookieName, dictionaryKey, valueToRemoveFromCollection) {
    var cookie = getCookie(cookieName);
    if (cookie) {
        var dictionary;
        try {
            dictionary = JSON.parse(cookie);
        }
        catch (error) {
            //ok, cookie corrupted, ignore it
            return;
        }
        var collection = dictionary[dictionaryKey];
        if (collection) {
            for (var i = 0; i < collection.length; i++) {
                if (collection[i] === valueToRemoveFromCollection) {
                    collection.splice(i, 1);
                    i--;
                }
            }
            setCookie(cookieName, JSON.stringify(dictionary));
        }
    }
}
function rgbToHex(rgba) {
    function trim(str) {
        return str.replace(/^\s+|\s+$/gm, '');
    }
    var parts = rgba.substring(rgba.indexOf("(")).split(","), r = parseInt(trim(parts[0].substring(1)), 10), g = parseInt(trim(parts[1]), 10), b = parseInt(trim(parts[2]), 10), a = parseFloat(trim(parts[3].substring(0, parts[3].length - 1))).toFixed(2);
    // @ts-ignore
    return ('#' + r.toString(16) + g.toString(16) + b.toString(16) + (a * 255).toString(16).substring(0, 2));
}
//# sourceMappingURL=site.js.map