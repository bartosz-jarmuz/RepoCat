var filtersParamName = 'filters';
function addFilterToUrl(property, value) {
    var currentUrl = document.location.href;
    var newUrl = updateUrl(currentUrl, property, value);
    window.history.pushState(null, null, newUrl);
}
function updateUrl(currentUrl, filterKey, activeFilterValues) {
    var firstChar = '&';
    if (currentUrl.indexOf('?') < 0) {
        firstChar = '?';
    }
    var newUrl = currentUrl;
    if (currentUrl.indexOf(filtersParamName) === -1) {
        newUrl = currentUrl += firstChar + getNewFiltersString(filterKey, activeFilterValues);
    }
    else {
        newUrl = updateExistingFiltersDefinition(currentUrl, filterKey, activeFilterValues);
    }
    return ensureFirstCharCorrect(newUrl);
}
function updateExistingFiltersDefinition(currentUrl, filterKey, activeFilterValues) {
    var newUrl = currentUrl;
    var allParameters = currentUrl.split(/(?=[\?&]+)/);
    //rewrite the URL string with new parameters
    for (var i = 0; i < allParameters.length; i++) {
        var param = allParameters[i];
        if (!param.startsWith('?' + filtersParamName) && !param.startsWith('&' + filtersParamName)) {
            continue; //ignore non-filter params
        }
        var key = param.replace(filtersParamName + '[', '').substring(1);
        key = key.substring(0, key.indexOf(']='));
        if (key == encodeURIComponent(filterKey)) {
            //the current key will be added 'from scratch', so remove all values for now
            newUrl = newUrl.replace(param, "");
        }
        //and the other filter values as well as other params should just stay untouched
    }
    //now add filters for the current key - if there are fewer values because a filter is removed, the new URL will end up shorter than current URL
    var firstChar = '&';
    if (newUrl.endsWith('&') || newUrl.endsWith('?')) {
        firstChar = '';
    }
    var newFilters = getNewFiltersString(filterKey, activeFilterValues);
    if (newFilters.length > 0) {
        newUrl += firstChar + newFilters;
    }
    return newUrl;
}
function ensureFirstCharCorrect(url) {
    if (url.indexOf('?') === -1 && url.indexOf('&') >= 0) {
        url = url.replace('&', '?');
    }
    return url;
}
function getNewFiltersString(filterKey, activeFilterValues) {
    var newFiltersString = '';
    for (var i = 0; i < activeFilterValues.length; i++) {
        if (activeFilterValues[i].length > 0) {
            newFiltersString += filtersParamName + '[' + encodeURIComponent(filterKey) + ']=' + encodeURIComponent(activeFilterValues[i]) + '&';
        }
    }
    if (newFiltersString.endsWith('&')) {
        newFiltersString = newFiltersString.slice(0, -1);
    }
    return newFiltersString;
}
//# sourceMappingURL=urlParams.js.map