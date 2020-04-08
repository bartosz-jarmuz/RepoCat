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
    } else {
        $(columnToggle).hide();
        column.visible(true);
        addToCollectionDictionaryCookie('activeColumns', getRepositoriesKey(), propertyName);
    }

}

function insertCell(propertyName, row) {
    var propertyCell = $(row).find('.properties');
    var propertyDiv = $(propertyCell).find('.property-name').filter(function () { return this.textContent == propertyName }).parent().parent();
    if (propertyDiv[0] !== undefined) {
        $($.parseHTML('<td class="break-word">' + propertyDiv.find('.description').text() + '</td>')).appendTo(row);
    } else {
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
        } else {
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
    var columns = getFromCollectionDictionaryCookie('hiddenDefaultColumns', getRepositoriesKey())
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
           
        }, //expnder
        null,//hidden content for expanded row
        null, //repo name
        null, //project name
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
    $cloned.attr('style', 'margin: 0px 2px; cursor: pointer;')
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
