function getProjectsTable(showRepositoryColumn) {
    return $('#ResultsTable').DataTable({
        pageLength: 100,
        stateSave: false,
        "autoWidth": true,
        "processing": true,
        "lengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
        order: [[3, 'asc']],
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
                "visible": showRepositoryColumn,
            },
            {
                "targets": [3],
                "max-width": "15%"
            },
            {
                "targets": [5],
                "max-width": "25%"
            },

        ],

        columns: [
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
                        $($(data), "i").each(function (index) {
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
        ],

        dom: "<'#SearchPanesHost'P>"
            + "<'row'<'col-md-1'l><'col-md-11'f>>"
            + "<rtip> ",

    });
}

function format(d) {
    var arr = Object.values(d);
    var details = $(arr[0]);
    return details.html();
}

function setupRowExpanding(table) {
    $('#ResultsTable tbody').on('click', 'td.details-control', function () {
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

function alignSearchPanel() {
    var input = $('#ResultsTable_filter label input').appendTo($('#ResultsTable_filter')).wrap("<div class='col'></div>");
    $('#ResultsTable_filter label').remove();
    $(input).attr('style', 'width: 100%; margin-left: 1rem; margin-right: 1rem;');
    $(input).attr('placeholder', 'Search table content...');
}

function setupSearchPanes() {
    $("#SearchPanesHost").appendTo("#SearchPanesCard");

    $('#SearchPanesCollapser').on('click', function () {
        if ($('#SearchPanesCard').hasClass('show')) {
            setCookie('searchPanesOpen', 'false');
        } else {
            setCookie('searchPanesOpen', 'true');
        }
    });
}