$(document).ready(function () {
    $.get("/Home/NavHeaderStats", function (data) {
        $("#NavHeaderStatsContainer").replaceWith(function () {
            return $(data).hide().fadeIn(500);
        });
    });
    $('.sidebar-minimizer').on('click', function () {
        if ($('body').hasClass('brand-minimized sidebar-minimized')) {
            setCookie('sidebarOpen', 'false');
        }
        else {
            setCookie('sidebarOpen', 'true');
        }
    });
});
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
//# sourceMappingURL=site.js.map