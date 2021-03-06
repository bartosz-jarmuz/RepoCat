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
function getColumns(numberOfExtraColumns, isSearchResult) {
    var columns = [
        {
            "className": 'expand-table-row',
            "orderable": isSearchResult,
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
function getProjectsTable(activeColumnsCookie, isSearchResult) {
    var t0 = performance.now();
    var showRepositoryColumn = $('#ResultsTableData').data('showrepositorycolumn');
    var numberOfExtraColumns = 0;
    if (activeColumnsCookie) {
        numberOfExtraColumns = showColumnsFromCookies(activeColumnsCookie);
    }
    else {
        numberOfExtraColumns = parseInt($('#ResultsTableData').attr('data-numberofextracolumns'));
    }
    var ordering;
    if (isSearchResult) {
        ordering = [[0, 'desc']];
    }
    else {
        ordering = [[3, 'asc']];
    }
    var table = $('#ResultsTable').DataTable({
        pageLength: 50,
        stateSave: false,
        "autoWidth": true,
        "processing": true,
        "lengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
        order: ordering,
        // @ts-ignore
        "columnDefs": [
            {
                "targets": [0],
                "visible": true,
                "searchable": false,
                "width": "2%",
                render: function (data, type, full, meta) {
                    if (type === "sort") {
                        // @ts-ignore
                        var api = new $.fn.dataTable.Api(meta.settings);
                        var td = api.cell({ row: meta.row, column: meta.col }).node(); // the td of the row
                        data = $(td).attr('data-order'); // the data it should be sorted by
                    }
                    return data;
                }
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
        columns: getColumns(numberOfExtraColumns, isSearchResult),
        dom: "R"
            + "<'#TopButtonsRow.row'<'col-md-1 first'l><'col-md-2 table-filter'><'col-md-8 restore-columns-row'><'col-md-1 third'if>>"
            + "<rtip> ",
    });
    if (isSearchResult) {
        $('td.expand-table-row').each(function () {
            $(this).html('<span class="accuracy-score" data-toggle="tooltip" title="This score indicates how relevant is this project to the search query.' +
                '\r\nThe higher the better - there is no maximum defined.">' +
                '<i class="far fa-star d-inline"></i>' +
                Number($(this).data('order')).toFixed(2)
                + '</span>');
        });
    }
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
            markSearchPhrases();
        }, 250);
    });
}
function markSearchPhrases() {
    // @ts-ignore
    $('.projects-table-card').unmark({
        done: function () {
            // @ts-ignore
            $('.projects-table-card').mark($('.table-search').val(), {
                className: 'search-mark',
                element: 'span'
            });
            // @ts-ignore
            $('.projects-table-card').mark($('#searchQuery').val(), {
                className: 'search-mark',
                element: 'span'
            });
        }
    });
    redrawStripes();
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
        function getProjectDetails(d) {
            var arr = Object.values(d);
            return $(arr[0]);
        }
        var $expandButton = $($(this).clone());
        var tr = $(this).closest('tr').not('.child-row-shown');
        if (!tr || tr.length == 0) {
            tr = $(this).closest('tr').prev('tr');
        }
        var row = table.row(tr);
        if (row.child.isShown()) {
            $('div.slider', row.child()).fadeOut();
            row.child.hide();
            $(tr).fadeIn();
        }
        else {
            var details = getProjectDetails(row.data());
            row.child(details.html()).show();
            var childRow = $(tr).next('tr');
            childRow.prepend($expandButton);
            childRow.addClass('child-row-shown');
            $('div.slider', row.child()).fadeIn();
            $(tr).hide();
            markSearchPhrases();
        }
        redrawStripes();
    });
}
function redrawStripes() {
    $('#ResultsTable tbody tr:visible').each(function (index) {
        if (index % 2 == 0) {
            $(this).css('background-color', '#f2f2f2');
        }
        else {
            $(this).css('background-color', '#ffffff');
        }
    });
}
function alignTopButtonsRow() {
    $('#TopButtonsRow').children('.first').attr('style', 'margin-right: 10px;');
    $('#TopButtonsRow').children('.third').attr('style', 'margin-left: -10px;');
}
function alignSearchPanel() {
    var input = $('#ResultsTable_filter label input').appendTo($('.table-filter')).wrap("<div class='col'></div>");
    $('#ResultsTable_filter label').remove();
    $(input).attr('style', 'width: 11rem; display: inline;');
    $(input).addClass('table-search');
    $(input).after("<i class='far fa-question-circle d-inline m-2' style='cursor: help;' data-toggle='tooltip'" +
        "title='Filtering is a basic method for searching within a table.\r\n" +
        "It simply shows only the rows which contain a given search string anywhere in the content.\r\n" +
        "Contrary to the repository Search function:\r\n" +
        "  - filtering does not take into account the \"weights\" of matches\r\n" +
        "  - filtering does not change the table sorting.\r\n" +
        "For example, if looking for \"Miss Universe\", a project with name \"Miss Universe\" is more relevant than\r\n" +
        "a project which description states something like \"This project is missing a universal template\"\r\n" +
        "The search function will order the results so that more \"relevant\" matches are shown on the top of the list." +
        "'></i>");
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
function showShareButton() {
    setTimeout(function () {
        if ($('.table-header .share-link').length == 0) {
            var button = '<a tabindex="0" class="btn btn-warning share-link" ><i class="fas fa-share-square"></i>&nbsp;Share</div>';
            $('.table-header').append($(button).hide().fadeIn());
            // @ts-ignore
            $('.share-link').tooltip({
                placement: 'top',
                trigger: 'hover',
                delay: { "show": 900, "hide": 100 },
                title: 'Get the URL of this results set'
            });
        }
    }, 500);
}
//# sourceMappingURL=projectsTable-declaration.js.map
/// <reference path="urlParams.js"/>
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
            if (filterValue === "repoCat_empty" && propertyValue.includes('')) {
                propertyMatched = true; //need special handling of 'empty' because the selectbox does not render options where value is empty
            }
            if (propertyValue.includes(filterValue)) {
                propertyMatched = true;
            }
        });
    }
    else {
        filter.value.forEach(function (filterValue) {
            if (filterValue === "repoCat_empty" && propertyValue.length === 0) {
                propertyMatched = true; //need special handling of 'empty' because the selectbox does not render options where value is empty
            }
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
                    addFilterToUrl($(this).data("property"), $(this).val());
                    showOverlay();
                    setTimeout(function () {
                        table.draw();
                        redrawStripes();
                        hideOverlay();
                        showShareButton();
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
            addFilterToUrl(data, selectBox.val());
            showFilter(selectBox, this, data);
            addToCollectionDictionaryCookie('activeFilters', getRepositoriesKey(), data);
            selectBox.data('inactive', 'FALSE');
        }
        if ($(this).hasClass('filter-label')) {
            hideFilter(this, data, table);
            removeFromCollectionDictionaryCookie('activeFilters', getRepositoriesKey(), data);
            addFilterToUrl($(this).data("property"), []);
        }
    });
}
function showActiveFilters(filtersFromModel) {
    if (filtersFromModel !== undefined && Object.keys(filtersFromModel).length > 0) {
        $('.filter-toggle').each(function (index, toggler) {
            var data = $(toggler).data('property');
            for (var _i = 0, _a = Object.entries(filtersFromModel); _i < _a.length; _i++) {
                var _b = _a[_i], key = _b[0], value = _b[1];
                if (key === data) {
                    var selectBox = $('.property-filter[data-property="' + key + '"');
                    showFilter(selectBox, $(toggler), data);
                    $(selectBox).val(value);
                }
            }
        });
    }
    else {
        var columns_1 = getFromCollectionDictionaryCookie('activeFilters', getRepositoriesKey());
        if (columns_1) {
            $('.filter-toggle').each(function (index, toggler) {
                var data = $(toggler).data('property');
                for (var i = 0; i < columns_1.length; i++) {
                    if (columns_1[i] === data) {
                        var selectBox = $('.property-filter[data-property="' + data + '"');
                        showFilter(selectBox, $(toggler), data);
                    }
                }
            });
        }
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
    //setTimeout(function () {
    var t0 = performance.now();
    $('.card').on('mouseover', '.select2-deferred', function (event) {
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
    //}, time);
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
        btn.popover({
            trigger: 'focus'
        });
        try {
            copyTextToClipboard(document.location.href, function () {
                btn.attr('data-content', 'Link copied to clipboard!');
                // @ts-ignore
                btn.popover('show');
                // @ts-ignore
                setTimeout(function () { btn.popover('hide'); }, 2000);
            }, function () {
                btn.attr('data-content', 'Failed to copy link to clipboard. Copy it manually :(');
                // @ts-ignore
                btn.popover('show');
                // @ts-ignore
                setTimeout(function () { btn.popover('hide'); }, 2000);
            });
        }
        catch (_a) {
            btn.attr('data-content', 'Failed to copy link to clipboard. Copy it manually :(');
            // @ts-ignore
            btn.popover('show');
        }
    });
    $(document).on('click', '.download-link', function () {
        showTempPopoverNoTitle($(this), "Hold on...", 1300, 'left');
        var counter = $(this).next('.download-count').find('span')[0];
        if (!counter) {
            counter = $(this).closest('.download').prev('.download-count').find('span')[0];
        }
        if (counter) {
            var value = parseInt($(counter).text());
            if (!isNaN(value)) {
                $(counter).text(value + 1);
            }
        }
    });
    attachShowMoreTagsHandlers();
});
function showTempPopoverNoTitle(element, text, timeout, placement) {
    var title = $(element).attr('title'); //hide title as it interferes with tooltip
    $(element).removeAttr('title');
    // @ts-ignore
    $(element).popover({
        trigger: 'manual',
        placement: placement,
        content: text,
    });
    // @ts-ignore
    $(element).popover('show');
    $(element).attr('title', title);
    // @ts-ignore
    setTimeout(function () { $(element).popover('hide'); }, timeout);
}
function copyTextToClipboard(text, success, fail) {
    if (!navigator || !navigator.clipboard) {
        fallbackCopyTextToClipboard(text, success, fail);
        return;
    }
    navigator.clipboard.writeText(text).then(function () {
        success();
    }, function (err) {
        fail();
        console.log(err);
    });
}
function fallbackCopyTextToClipboard(text, success, fail) {
    var textArea = document.createElement("textarea");
    textArea.value = text;
    // Avoid scrolling to bottom
    textArea.style.top = "0";
    textArea.style.left = "0";
    textArea.style.position = "fixed";
    document.body.appendChild(textArea);
    textArea.focus();
    textArea.select();
    var successful = document.execCommand('copy');
    if (successful) {
        success();
    }
    else {
        fail();
    }
    document.body.removeChild(textArea);
}
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
function getAllActiveFiltersQuery() {
    var filters = getFilters();
    var allFiltersQuery = '';
    if (filters.length > 0) {
        filters.forEach(function (filter) {
            allFiltersQuery += '&' + getNewFiltersString(filter.key, filter.value);
        });
    }
    return allFiltersQuery;
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