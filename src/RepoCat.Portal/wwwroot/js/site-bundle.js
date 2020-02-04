$(document).ready(function () {


    $(function () {
        $('[data-toggle="collapse"]').on('click', function () {
            var icon = $(this).find('.toggler-icon');
            if (icon.hasClass('icon-arrow-up')) {
                setArrowDown(icon);
            } else {
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

function collapseAllToggle(sender) {
    if ($(sender).hasClass('open')) {
        $('div.collapse').collapse('hide');
        $(sender).removeClass('open');
        $(sender).text('Expand all');
        setArrowDown($('[data-toggle="collapse"]').find('.toggler-icon'));
    } else {
        $('div.collapse').collapse('show');
        $(sender).addClass('open');
        $(sender).text('Collapse all');
        setArrowUp($('[data-toggle="collapse"]').find('.toggler-icon'));
    }
}
$(document).ready(function () {
    $('.nav-dropdown-toggle').click(function () {
        var itemText = $(this).text().trim();
        var cookie = getCookie('openNavItems');
        var dropdown = $(this).parent();
        if (!dropdown.hasClass('open')) {
            if (cookie) {
                if (!isItemInArray(cookie, '_', itemText)) {
                    cookie += "_" + itemText
                    setCookie('openNavItems', cookie);
                }
            } else {
                setCookie('openNavItems', itemText);
            }
        } else {
            if (cookie) {
                var split = cookie.split('_');
                for (var i = 0; i < split.length; i++) {
                    if (split[i] === itemText) {
                        split.splice(i, 1);
                        i--;
                    }
                }
                var joint = split.join('_');
                setCookie('openNavItems', joint);
            }
        }
    });

    var openItems = getCookie('openNavItems');
    if (openItems) {
        var split = openItems.split('_');
        $('.nav-dropdown-toggle').each(function () {
            if (split.includes($(this).text().trim())) {
                $(this).parent().addClass('open');
            }
        });
    }

    
});

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
function propertyFilter (settings, searchData, index, rowData, counter) {

    var filters = getFilters();

    if (filters.length === 0) {
        return true;
    }
    var properties = getProperties(rowData);

    var shouldBeVisible = true;
    filters.forEach(function (filter) {
        //find a property for the given filter
        var property = properties.filter(function (p) { return p.key === filter.key }).find(function () { return true; });
        if (property) {
            var propertyMatched = false;
            filter.value.forEach(function (filterValue) {
                if (filterValue === property.value) {
                    propertyMatched = true;
                }
            });
            if (!propertyMatched) {
                shouldBeVisible = false;
                return;
            }
        } else {
            //the project row does not contain this property.
        }
    });

    return shouldBeVisible;
}

function getProperties(rowData) {
    var properties = [];
    var propertyPairs = $($(rowData[8]).filter(function (tag) { return this.tagName === 'DIV' }));
    propertyPairs.each(function () {
        var propertyName = $(this).find('.property-name').text();
        var val = $(this).find('.description').text().trim();
        var property = { key: propertyName, value: val };
        properties.push(property);
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
        if (value.length > 0) {
            var filter = { key: $(this).data('property'), value: value }
            filters.push(filter);
        }
    });
    return filters;
}

function setupFiltering(table) {
    $('.property-filter').on('change', function () {
        if ($(this).hasClass('filter-active')) {
            table.draw();
        }
    });

    $('.filter-toggle').on('click', function () {
        var data = $(this).data('property');
        if ($(this).hasClass('add-filter')) {
            showFilter(this, data);
            addToActiveFilters(data);
        }
        if ($(this).hasClass('filter-label')) {
            hideFilter(this, data, table);
            removeFromActiveFilters(data);
        }
    });
}

function showActiveFilters() {
    var cookie = getCookie('activeFilters');
    if (cookie) {
        var split = cookie.split('_');
        $('.filter-toggle').each(function (index, toggler) {
            var data = $(toggler).data('property');
            for (var i = 0; i < split.length; i++) {
                if (split[i] === data) {
                    showFilter(toggler, data);
                }
            }
        });
    }
}

function addToActiveFilters(filterKey) {
    var cookie = getCookie('activeFilters');

    if (cookie) {
        if (!isItemInArray(cookie, '_', filterKey)) {
            cookie += "_" + filterKey
            setCookie('activeFilters', cookie);
        }
    } else {
        setCookie('activeFilters', filterKey);
    }
}

function removeFromActiveFilters(filterKey) {
    var cookie = getCookie('activeFilters');

    if (cookie) {
        var split = cookie.split('_');
        for (var i = 0; i < split.length; i++) {
            if (split[i] === filterKey) {
                split.splice(i, 1);
                i--;
            }
        }
        var joint = split.join('_');
        setCookie('activeFilters', joint);
    } 
}


function showFilter(filterToggle, data) {
    var filterHost = $('.property-filter[data-property="' + data + '"').closest('.filter-host');
    filterHost.appendTo($('#PropertyFilters'))
    filterHost.find('.property-filter').addClass('filter-active');
    filterHost.fadeIn();
    filterHost.find('i').removeClass('fa-plus').addClass('fa-minus');
    $(filterToggle).hide();
    filterHost.find('input').attr('style', 'width: inherit;')
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
    table.draw();
}
$(document).ready(function () {

    $.get("/Home/NavHeaderStats", function (data) {
        $("#NavHeaderStatsContainer").replaceWith(function () {
            return $(data).hide().fadeIn(500);
        });
    });

    intializeSelect2();




    $('.select2-inline').parent().find('.select2-container').addClass('inline-filter');
});


function intializeSelect2() {
    $('.select2').each(function () {


        var css = {};
        if ($(this).hasClass('form-control-lg')) {
            css = {
                color: '#4d555d',
                'font-size': 'large'
            }
        }

        $(this).select2({
            theme: 'bootstrap4',
            placeholder: $(this).attr('placeholder'),
            width: 'style',
            allowClear: Boolean($(this).data('allow-clear')),
            containerCss: css,
        });
    });
}

function getOrganizationFromKey(key) {
    return key.split(":")[0];
}
function getRepositoryFromKey(key) {
    return key.split(":")[1];
}


$(document).ready(function () {

    $.get("/Home/NavHeaderStats", function (data) {
        $("#NavHeaderStatsContainer").replaceWith(function () {
            return $(data).hide().fadeIn(500);
        });
    });

    

    $('.sidebar-minimizer').on('click', function () {
        if ($('body').hasClass('brand-minimized sidebar-minimized')) {
            setCookie('sidebarOpen', 'false');
        } else {
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
    if (text.length > 45) {
        var middlePoint = text.length / 2;
        var firstPart = text.substring(0, middlePoint);
        var secondPart = text.substring(middlePoint);
        return firstPart.substring(0, 20) + '(...)' + secondPart.substring(secondPart.length - 20);
    }
}