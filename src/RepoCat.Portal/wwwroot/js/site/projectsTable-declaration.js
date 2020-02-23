function getProjectsTable(activeColumnsCookie) {

    var t0 = performance.now();

    var showRepositoryColumn = $('#ResultsTableData').data('showrepositorycolumn');
    var numberOfExtraColumns = 0;
    if (activeColumnsCookie) {
        numberOfExtraColumns = showColumnsFromCookies(activeColumnsCookie);
    } else {
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
        searchPanes: {
            layout: "columns-4",
            columns: [3, 4, 7, 8],
            cascadePanes: true,
            viewTotal: true,
            dataLength: 30,
            dtOpts: {
                order: [[1, 'desc']]
            }
        },
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
            },
            {
                "targets": [5],
            },
        ],

        columns: getColumns(numberOfExtraColumns),

        dom: "R<'#SearchPanesHost'P>"
            + "<'#TopButtonsRow.row'<'col-md-1 first'l><'col-md-10 restore-columns-row'><'col-md-1 third'if>>"
            + "<rtip> ",

    });

  

    setupHideButtons(table);
    
    hideDefaultColumnsFromCookies(table);
    var t1 = performance.now();
    console.log("Drawing table: " + (t1 - t0) + " milliseconds.");

    return table;

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

function getColumns(numberOfExtraColumns) {
    var columns = [
        {
            "className": 'details-control',
            "orderable": false,
            "data": null,
            "defaultContent": ''
        }, //expnder
        null,//hidden content for expanded row
        null, //repo name
        {
            searchPanes: {
                show: true,
                orthogonal: 'sp'
            },
            render: function (data, type, row) {
                if (type === 'sp') {
                    var tags = [];
                    $(data).closest('.project-name').each(function (index) {
                        var val = $(this).text();
                        val = val.trim();
                        if (val.length > 0) {
                            tags.push(val);
                        }
                    });
                    return tags;
                }
                return data;
            }
        }, //project name
        {
            searchPanes: {
                show: true,
            },
        },
        null,
        null,
        {
            searchPanes: {
                show: true,
                orthogonal: 'sp'

            },
            render: function (data, type, row) {
                if (type === 'sp') {
                    var tags = [];
                    $(data, "i").each(function (index) {
                        var val = $(this).text();
                        val = val.trim();
                        if (val.length > 0) {
                            tags.push(val);
                        }
                    });
                    return tags;
                }
                return data;
            }
        },
        {
            searchPanes: {
                show: true,
                orthogonal: 'sp'

            },
            render: function (data, type, row) {
                if (type === 'sp') {
                    var tags = [];
                    var descriptions = $(data).children().closest('.description');
                    descriptions.each(function (index) {
                        var val = $(this).text();
                        val = val.trim();
                        if (val.length > 0) {
                            tags.push(val.substring(0, 40));
                        }
                    });
                    return tags;
                }
                return data;
            }
        },
    ];

    if (numberOfExtraColumns !== undefined) {
        for (var i = 0; i < numberOfExtraColumns; i++) {
            columns.push(null);
        }
    }
    return columns;
}

function setupTableFeatures(table) {
    setupRowExpanding(table);

    setupSearchPanes();

    alignSearchPanel();

    setupFiltering(table);

    alignTopButtonsRow();

    setupAddingColumns(table);

    
}

function format(d) {
    var arr = Object.values(d);
    var details = $(arr[0]);
    return details.html(); 
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


function setupRowExpanding(table) {
    $('#ResultsTable tbody').off('click.rc.rows');
    $('#ResultsTable tbody').on('click.rc.rows', 'td.details-control', function () {
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
    $('#TopButtonsRow').children('.first').attr('style', 'margin-right: 10px;')
    $('#TopButtonsRow').children('.third').attr('style', 'margin-left: -10px;')

}

function alignSearchPanel() {
    var spContainer = $('.dtsp-panes.dtsp-container');
    var input = $('#ResultsTable_filter label input').appendTo(spContainer).wrap("<div class='col'></div>");
    $('#ResultsTable_filter label').remove(); 
    $(input).attr('style', 'width: 100%;');
    $(input).removeClass('form-control-sm');
    $(input).addClass('form-control-lg'); 
    $(input).attr('placeholder', 'Search table content...');
}

function setupSearchPanes() {
    $("#SearchPanesCard").empty();
    $("#SearchPanesHost").appendTo("#SearchPanesCard");
    $('#SearchPanesCollapser').off('click.rc.panes');
    $('#SearchPanesCollapser').on('click.rc.panes', function () {
        if ($('#SearchPanesCard').hasClass('show')) {
            $('#SearchPanesCollapser').text('Show filters');
            setCookie('searchPanesOpen', 'false');
        } else {
            setCookie('searchPanesOpen', 'true');
            $('#SearchPanesCollapser').text('Hide filters');
        }
    });
}

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
        $($.parseHTML('<td>' + propertyDiv.find('.description').text() + '</td>')).appendTo(row);
    } else {
        $($.parseHTML('<td></td>')).appendTo(row);
    }
}

function showOverlay() {
    var overlay = $('#ResultsTable').closest('.card').find('.overlay');
    $(overlay).show().fadeIn();
}

function hideOverlay() {
    var overlay = $('#ResultsTable').closest('.card').find('.overlay');
    $(overlay).fadeOut();
}


function addToActiveColumns(columnKey) {
    addToCollectionDictionaryCookie('activeColumns', getRepositoriesKey(), columnKey);
}

function removeFromActiveColumns(columnKey) {
    removeFromCollectionDictionaryCookie('activeColumns', getRepositoriesKey(), columnKey);
}

function getRepositoriesKey() {
    return $('#ResultsTableData').attr('data-repositories');
}