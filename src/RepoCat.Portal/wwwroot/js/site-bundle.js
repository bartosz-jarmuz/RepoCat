$(document).ready(function () {
    $(function () {
        $('[data-toggle="collapse"]').on('click', function () {
            var icon = $(this).find('.toggler-icon');
            if (icon.hasClass('icon-arrow-up')) {
                setArrowDown(icon);
            }
            else {
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
//# sourceMappingURL=collapseToggle.js.map
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
        while (c.charAt(0) == ' ')
            c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0)
            return c.substring(nameEQ.length, c.length);
    }
    return null;
}
function eraseCookie(name) {
    document.cookie = name + '=; Max-Age=-99999999;';
}
//# sourceMappingURL=cookies.js.map
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
function setupAddingColumns(table) {
    $('.add-column').off('click.rc.columns');
    $('.add-column').on('click.rc.columns', function () {
        var data = $(this).data('property');
        addColumn(this, data, table);
    });
}
function addColumn(columnToggle, propertyName, table) {
    var column = table.column('[data-column-id="' + propertyName + '"]');
    if (column.length === 0) {
        showOverlay();
        setTimeout(function () {
            var numberOfExtraColumns = $('#ResultsTableData').attr('data-numberofextracolumns');
            $('#ResultsTableData').attr('data-numberofextracolumns', parseInt(numberOfExtraColumns) + 1);
            table.destroy();
            $('#ResultsTable > thead > tr').append('<th data-column-id="' + propertyName + '">' + propertyName + '</th>');
            table.rows().nodes().to$().each(function (index, row) {
                insertCell(propertyName, row);
            });
            table = getProjectsTable();
            setupTableFeatures(table);
            $(columnToggle).hide();
            hideOverlay();
            addToCollectionDictionaryCookie('activeColumns', getRepositoriesKey(), propertyName);
        }, 10);
    }
    else {
        $(columnToggle).hide();
        column.visible(true);
        addToCollectionDictionaryCookie('activeColumns', getRepositoriesKey(), propertyName);
    }
}
function insertCell(propertyName, row) {
    var propertyCell = $(row).find('.properties');
    var propertyDiv = $(propertyCell).find('.property-name').filter(function () { return this.textContent == propertyName; }).parent().parent();
    if (propertyDiv[0] !== undefined) {
        $($.parseHTML('<td class="break-word">' + propertyDiv.find('.description').text() + '</td>')).appendTo(row);
    }
    else {
        $($.parseHTML('<td></td>')).appendTo(row);
    }
}
function addToActiveColumns(columnKey) {
    addToCollectionDictionaryCookie('activeColumns', getRepositoriesKey(), columnKey);
}
function removeFromActiveColumns(columnKey) {
    removeFromCollectionDictionaryCookie('activeColumns', getRepositoriesKey(), columnKey);
}
function addHideButtonToHeader(headerCell) {
    if (!$(headerCell).find('.column-hide').length && $(headerCell).data('column-id')) {
        !$(headerCell).append('&nbsp;<i class="column-hide icons icon-minus" data-toggle="tooltip" title="Click to hide the column"></i>');
    }
}
function setupHideButtons(table) {
    $('#ResultsTable th').each(function () {
        addHideButtonToHeader($(this));
    });
    $('.column-hide').off('click.rc.column-hide');
    $('.column-hide').on('click.rc.column-hide', function (e) {
        e.preventDefault();
        var columnId = $(this).parent().data('column-id');
        var column = table.column('[data-column-id="' + columnId + '"]');
        column.visible(false);
        if (columnId.startsWith("rc_")) {
            var columnName = $(this).parent().text();
            addHideDefaultColumnButton(column, columnId, columnName);
        }
        else {
            $('.add-column[data-property="' + columnId + '"').show();
            removeFromCollectionDictionaryCookie('activeColumns', getRepositoriesKey(), columnId);
        }
    });
}
function showColumnsFromCookies(activeColumnsCookie) {
    var numberOfExtraColumns = 0;
    var repos = getRepositoriesKey();
    var columnsPerRepo = JSON.parse(activeColumnsCookie);
    var columns = columnsPerRepo[repos];
    if (columns) {
        numberOfExtraColumns = columns.length;
        for (var i = 0; i < columns.length; i++) {
            var propertyName = columns[i];
            $('#ResultsTable > thead > tr').append('<th data-column-id="' + propertyName + '">' + propertyName + '</th>');
            $('#ResultsTable > tbody > tr').each(function (index, row) {
                insertCell(propertyName, row);
            });
            var button = $('.add-column[data-property="' + propertyName + '"');
            button.hide();
        }
        $('#ResultsTableData').attr('data-numberofextracolumns', numberOfExtraColumns);
    }
    return numberOfExtraColumns;
}
function hideDefaultColumnsFromCookies(table) {
    var columns = getFromCollectionDictionaryCookie('hiddenDefaultColumns', getRepositoriesKey());
    if (columns) {
        for (var i = 0; i < columns.length; i++) {
            var column = table.column('[data-column-id="' + columns[i] + '"]');
            addHideDefaultColumnButton(column, columns[i], columns[i].replace("rc_", ""));
            column.visible(false);
        }
    }
}
function getColumns(numberOfExtraColumns) {
    var columns = [
        {
            "className": 'expand-table-row',
            "orderable": false,
            "data": null,
            "defaultContent": ''
        },
        null,
        null,
        null,
        null,
        null,
        null
    ];
    if (numberOfExtraColumns !== undefined) {
        for (var i = 0; i < numberOfExtraColumns; i++) {
            columns.push(null);
        }
    }
    return columns;
}
function addHideDefaultColumnButton(column, columnId, columnName) {
    var $cloned = $('.hide-default-column').first().clone();
    $cloned.attr('style', 'margin: 0px 2px; cursor: pointer;');
    $cloned.attr('data-property', columnId);
    $cloned.attr('title', 'Click to restore column: ' + columnName);
    var span = $cloned.children('span').first();
    span.text(columnName);
    $('.restore-columns-row').append($cloned.show());
    addToCollectionDictionaryCookie('hiddenDefaultColumns', getRepositoriesKey(), columnId);
    $cloned.click(function () {
        column.visible(true);
        $cloned.remove();
        removeFromCollectionDictionaryCookie('hiddenDefaultColumns', getRepositoriesKey(), columnId);
    });
}
//# sourceMappingURL=projectsTable-columns.js.map
function getProjectsTable(activeColumnsCookie) {
    var t0 = performance.now();
    var showRepositoryColumn = $('#ResultsTableData').data('showrepositorycolumn');
    var numberOfExtraColumns = 0;
    if (activeColumnsCookie) {
        numberOfExtraColumns = showColumnsFromCookies(activeColumnsCookie);
    }
    else {
        numberOfExtraColumns = parseInt($('#ResultsTableData').attr('data-numberofextracolumns'));
    }
    var table = $('#ResultsTable').DataTable({
        pageLength: 50,
        stateSave: false,
        "autoWidth": true,
        "processing": true,
        "lengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
        order: [[3, 'asc']],
        // @ts-ignore
        "columnDefs": [
            {
                "targets": [0],
                "visible": true,
                "searchable": false,
                "width": "2%"
            },
            {
                "targets": [1],
                "visible": false,
            },
            {
                "targets": [2],
                "visible": (showRepositoryColumn.toUpperCase() == 'TRUE'),
            },
            {
                "targets": [3],
                "width": "25%"
            },
            {
                "targets": [4],
                "width": "2%"
            },
        ],
        columns: getColumns(numberOfExtraColumns),
        dom: "R"
            + "<'#TopButtonsRow.row'<'col-md-1 first'l><'col-md-10 restore-columns-row'><'col-md-1 third'if>>"
            + "<rtip> ",
    });
    setupHideButtons(table);
    alignSearchPanel();
    hideDefaultColumnsFromCookies(table);
    setupSearchHighlights();
    var t1 = performance.now();
    console.log("Drawing table: " + (t1 - t0) + " milliseconds.");
    return table;
}
function setupSearchHighlights() {
    var timeout = null;
    $('.table-search').on('input', function () {
        clearTimeout(timeout);
        timeout = setTimeout(function () {
            // @ts-ignore
            $('.projects-table-card').unmark({
                done: function () {
                    // @ts-ignore
                    $('.projects-table-card').mark($('.table-search').val(), {
                        className: 'search-mark',
                        element: 'span'
                    });
                }
            });
        }, 250);
    });
}
function setupTableFeatures(table) {
    setupRowExpanding(table);
    setupFiltering(table);
    alignTopButtonsRow();
    setupAddingColumns(table);
}
function setupRowExpanding(table) {
    $('#ResultsTable tbody').off('click.rc.rows');
    $('#ResultsTable tbody').on('click.rc.rows', '.expand-table-row', function () {
        function format(d) {
            var arr = Object.values(d);
            var details = $(arr[0]);
            return details.html();
        }
        var tr = $(this).closest('tr');
        var row = table.row(tr);
        if (row.child.isShown()) {
            $('div.slider', row.child()).slideUp(function () {
                row.child.hide();
                tr.removeClass('shown');
            });
        }
        else {
            row.child(format(row.data())).show();
            tr.addClass('shown');
            $('div.slider', row.child()).slideDown();
        }
    });
}
function alignTopButtonsRow() {
    $('#TopButtonsRow').children('.first').attr('style', 'margin-right: 10px;');
    $('#TopButtonsRow').children('.third').attr('style', 'margin-left: -10px;');
}
function alignSearchPanel() {
    var container = $('#TableSearchDiv').empty();
    var input = $('#ResultsTable_filter label input').appendTo(container).wrap("<div class='col'><small class='help-block text-secondary'>Search table</small></div>");
    $('#ResultsTable_filter label').remove();
    $(input).attr('style', 'width: 100%;');
    $(input).removeClass('form-control-sm');
    $(input).addClass('form-control-lg');
    $(input).addClass('table-search');
    $(input).attr('placeholder', 'Type to filter the table');
}
function showOverlay() {
    var overlay = $('#ResultsTable').closest('.card').find('.overlay');
    $(overlay).show().fadeIn();
}
function hideOverlay() {
    var overlay = $('#ResultsTable').closest('.card').find('.overlay');
    $(overlay).fadeOut();
}
function getRepositoriesKey() {
    return $('#ResultsTableData').attr('data-repositories');
}
//# sourceMappingURL=projectsTable-declaration.js.map
function propertyFilter(settings, searchData, index, rowData, counter) {
    var filters = getFilters();
    if (filters.length === 0) {
        return true;
    }
    var properties = getProperties(rowData);
    var shouldBeVisible = true;
    var _loop_1 = function (filterIndex) {
        var filter = filters[filterIndex];
        //find properties for the given filter. there can be more than one property with the same name 
        //(multiple components can have same property with different values)
        var matchingProperties = properties.filter(function (p) { return p.key === filter.key; });
        if (matchingProperties && matchingProperties.length) {
            if (!isAnyPropertyValueMatched(matchingProperties, filter)) {
                shouldBeVisible = false;
                return "break";
            }
        }
        else {
            //the project row does not contain this property.
            if (!filter.value.includes("repoCat_no_property")) {
                shouldBeVisible = false;
                return "break";
            }
        }
    };
    for (var filterIndex = 0; filterIndex < filters.length; filterIndex++) {
        var state_1 = _loop_1(filterIndex);
        if (state_1 === "break")
            break;
    }
    return shouldBeVisible;
}
function isAnyPropertyValueMatched(matchingProperties, filter) {
    var propertyMatched = false;
    for (var propertyIndex = 0; propertyIndex < matchingProperties.length; propertyIndex++) {
        propertyMatched = isSinglePropertyMatched(matchingProperties[propertyIndex].value, filter);
        if (propertyMatched) {
            //these are all different values for the same property in the same row.
            //if any is matched, we want to show it, because the filters are whitelists, not blacklists
            break;
        }
    }
    return propertyMatched;
}
function isSinglePropertyMatched(propertyValue, filter) {
    var propertyMatched = false;
    if (Array.isArray(propertyValue)) {
        filter.value.forEach(function (filterValue) {
            if (propertyValue.includes(filterValue)) {
                propertyMatched = true;
            }
        });
    }
    else {
        filter.value.forEach(function (filterValue) {
            if (filterValue === propertyValue) {
                propertyMatched = true;
            }
        });
    }
    return propertyMatched;
}
function getProperties(rowData) {
    var propCell;
    for (var i = 1; i <= Object.keys(rowData).length; i++) {
        if ($(rowData[i]).hasClass('property')) {
            propCell = rowData[i];
            break;
        }
    }
    var properties = [];
    var propertyPairs = $($(propCell).filter(function (tag) { return this.tagName === 'DIV'; }));
    propertyPairs.each(function () {
        var propertyName = $(this).find('.property-name').text();
        var select = $(this).find('select');
        if (select.length) {
            var values_1 = [];
            $(select).children('option').each(function () {
                var val = $(this).text().trim();
                values_1.push(val);
            });
            var property = { key: propertyName, value: values_1 };
            properties.push(property);
        }
        else {
            var val = $(this).find('.description').text().trim();
            var property = { key: propertyName, value: val };
            properties.push(property);
        }
    });
    return properties;
}
function getFilters() {
    var filters = [];
    var activeFilters = $('.filter-active');
    if (!activeFilters) {
        return filters;
    }
    activeFilters.each(function () {
        var value = $(this).val();
        if (value.toString().length > 0) {
            var filter = { key: $(this).data('property'), value: value };
            filters.push(filter);
        }
    });
    return filters;
}
function setupFiltering(table) {
    $('.property-filter').off('change.rc.filter');
    $('.property-filter').on('change.rc.filter', function () {
        if ($(this).hasClass('filter-active')) {
            if ($(this).data('inactive') !== 'TRUE') {
                if ($(this).val() !== '') {
                    showOverlay();
                    setTimeout(function () {
                        table.draw();
                        hideOverlay();
                    }, 10);
                }
            }
        }
    });
    $('.filter-toggle').off('click.rc.filter');
    $('.filter-toggle').on('click.rc.filter', function () {
        var data = $(this).data('property');
        if ($(this).hasClass('add-filter')) {
            var selectBox = $('.property-filter[data-property="' + data + '"');
            selectBox.data('inactive', 'TRUE');
            showFilter(selectBox, this, data);
            addToCollectionDictionaryCookie('activeFilters', getRepositoriesKey(), data);
            selectBox.data('inactive', 'FALSE');
        }
        if ($(this).hasClass('filter-label')) {
            hideFilter(this, data, table);
            removeFromCollectionDictionaryCookie('activeFilters', getRepositoriesKey(), data);
        }
    });
}
function showActiveFilters() {
    var columns = getFromCollectionDictionaryCookie('activeFilters', getRepositoriesKey());
    if (columns) {
        $('.filter-toggle').each(function (index, toggler) {
            var data = $(toggler).data('property');
            for (var i = 0; i < columns.length; i++) {
                if (columns[i] === data) {
                    var selectBox = $('.property-filter[data-property="' + data + '"');
                    showFilter(selectBox, $(toggler), data);
                }
            }
        });
    }
}
function showFilter(selectBox, filterToggle, data) {
    var filterHost = $(selectBox.closest('.filter-host'));
    filterHost.appendTo($('#PropertyFilters'));
    filterHost.find('.property-filter').addClass('filter-active');
    filterHost.fadeIn();
    filterHost.find('i.add-remove-icon').removeClass('icon-plus').addClass('icon-minus');
    $(filterToggle).hide();
    filterHost.find('input').attr('style', 'width: inherit;');
    filterHost.find('.badge-property-name').show();
    filterHost.find('.badge-property-name').attr('title', 'Remove filtering option');
    filterHost.find('.property-filter').trigger("change");
}
function hideFilter(filterToggle, data, table) {
    var $host = $($(filterToggle).closest('.filter-host'));
    $host.hide();
    $host.find('.property-filter').removeClass('filter-active');
    $host.appendTo('#HiddenPropertyFilters');
    $('.add-filter[data-property="' + data + '"').show();
    showOverlay();
    setTimeout(function () {
        table.draw();
        hideOverlay();
    }, 10);
}
//# sourceMappingURL=projectsTable-propertyFilter.js.map
$(document).ready(function () {
    $.get("/Home/NavHeaderStats", function (data) {
        $("#NavHeaderStatsContainer").replaceWith(function () {
            return $(data).hide().fadeIn(500);
        });
    });
    intializeSelect2();
    $('.select2-inline').parent().find('.select2-container').addClass('inline-filter');
});
function initializeDeferredSelect2() {
    var time = 200;
    setTimeout(function () {
        var t0 = performance.now();
        $('.select2-deferred').each(function () {
            // @ts-ignore
            var width = ($(this).width() + 26).toString();
            var css = {
                display: 'inline-block',
                'font-size': 'small',
            };
            $(this).select2({
                theme: 'bootstrap4',
                placeholder: $(this).attr('placeholder'),
                width: width,
                allowClear: false,
                containerCss: css,
            });
        });
        var t1 = performance.now();
        console.log("Drawing select boxes: " + (t1 - t0) + " milliseconds.");
    }, time);
}
function intializeSelect2() {
    $('.select2').each(function () {
        var css = {};
        var width = 'style';
        if ($(this).hasClass('form-control-lg')) {
            css = {
                color: '#4d555d',
                'font-size': 'large'
            };
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
                }
                else {
                    return data.text;
                }
            },
            // @ts-ignore
            templateSelection: function (data) {
                if (data.id && data.id.toString().endsWith(':*')) {
                    return $('<span style="font-style: italic">' + data.text + '</span>');
                }
                else {
                    return data.text;
                }
            }
        });
    });
}
//# sourceMappingURL=selectBox.js.map
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
//# sourceMappingURL=site.js.map