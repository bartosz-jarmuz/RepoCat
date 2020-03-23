function propertyFilter(settings, searchData, index, rowData, counter) {
    let filters = getFilters();

    if (filters.length === 0) {
        return true; 
    }
    let properties = getProperties(rowData);

    let shouldBeVisible = true;
    for (let filterIndex = 0; filterIndex < filters.length; filterIndex++) {
        let filter = filters[filterIndex];
        //find properties for the given filter. there can be more than one property with the same name 
        //(multiple components can have same property with different values)
        let matchingProperties = properties.filter(function (p) { return p.key === filter.key });
        if (matchingProperties && matchingProperties.length) {
            if (!isAnyPropertyValueMatched(matchingProperties, filter)) {
                shouldBeVisible = false;
                break;
            }
        } else {
            //the project row does not contain this property.
            if (!filter.value.includes("repoCat_no_property")) {
                shouldBeVisible = false;
                break;
            }
        } 
    } 

    return shouldBeVisible;
}


function isAnyPropertyValueMatched(matchingProperties, filter) {
    let propertyMatched = false;
    for (let propertyIndex = 0; propertyIndex < matchingProperties.length; propertyIndex++) {
        propertyMatched = isSinglePropertyMatched(matchingProperties[propertyIndex].value, filter);
        if (propertyMatched) {
            //these are all different values for the same property in the same row.
            //if any is matched, we want to show it, because the filters are whitelists, not blacklists
            break;
        }
    }

    return propertyMatched ;
}

function isSinglePropertyMatched(propertyValue, filter) {
    let propertyMatched = false;

    if (Array.isArray(propertyValue)) {
        filter.value.forEach(function (filterValue) {
            if (propertyValue.includes(filterValue)) {
                propertyMatched = true;
            }
        });
    } else {
        filter.value.forEach(function (filterValue) {
            if (filterValue === propertyValue) {
                propertyMatched = true;
            }
        });
    }
    return propertyMatched;
}

function getProperties(rowData) {
    let propCell;
    for (var i = 1; i <= Object.keys(rowData).length; i++) {
        if ($(rowData[i]).hasClass('property')) {
            propCell = rowData[i];
            break;
        }
    }

    var properties = [];
    var propertyPairs = $($(propCell).filter(function (tag) { return this.tagName === 'DIV' }));
    propertyPairs.each(function () {
        let propertyName = $(this).find('.property-name').text();
        let select = $(this).find('select');
        if (select.length) {
            let values = [];
            $(select).children('option').each(function () {
                let val = $(this).text().trim();
                values.push(val);
            });
            let property = { key: propertyName, value: values };
            properties.push(property);
        } else {
            let val = $(this).find('.description').text().trim();
            let property = { key: propertyName, value: val };
            properties.push(property);
        }
    }); 
    return properties;
} 
 
function getFilters() { 
    let filters = []; 
     
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

