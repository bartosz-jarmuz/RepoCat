function getProjectsTable(activeColumnsCookie) {

    let t0 = performance.now();

    let showRepositoryColumn = $('#ResultsTableData').data('showrepositorycolumn');
    let numberOfExtraColumns = 0;
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

    let t1 = performance.now();
    console.log("Drawing table: " + (t1 - t0) + " milliseconds.");

    return table;

}

function setupSearchHighlights() {
    let timeout = null;
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
        let $expandButton = $($(this).clone());
        let tr = $(this).closest('tr').not('.child-row-shown'); 
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
            let details = getProjectDetails(row.data());
            row.child(details.html()).show(); 
            let childRow = $(tr).next('tr');
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
        } else {
            $(this).css('background-color', '#ffffff');
        }
    });
}

function alignTopButtonsRow() {
    $('#TopButtonsRow').children('.first').attr('style', 'margin-right: 10px;')
    $('#TopButtonsRow').children('.third').attr('style', 'margin-left: -10px;')
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