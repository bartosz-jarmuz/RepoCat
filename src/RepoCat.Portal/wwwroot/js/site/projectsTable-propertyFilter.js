function propertyFilter(settings, searchData, index, rowData, counter) {

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
        if (value.toString().length > 0) {
            var filter = { key: $(this).data('property'), value: value }
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
    var columns = getFromCollectionDictionaryCookie('activeFilters', getRepositoriesKey()) 
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
    filterHost.appendTo($('#PropertyFilters'))
    filterHost.find('.property-filter').addClass('filter-active');
    filterHost.fadeIn();
    filterHost.find('i.add-remove-icon').removeClass('icon-plus').addClass('icon-minus');
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

    showOverlay();
    setTimeout(function () {
        table.draw();
        hideOverlay();
    }, 10);
}

