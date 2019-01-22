/*!
 * Datalist 6.2.0
 * https://github.com/NonFactors/MVC5.Datalist
 *
 * Copyright © NonFactors
 *
 * Licensed under the terms of the MIT License
 * http://www.opensource.org/licenses/mit-license.php
 */
var MvcDatalistFilter = (function () {
    function MvcDatalistFilter(datalist) {
        var data = datalist.group.dataset;

        this.datalist = datalist;
        this.sort = data.sort;
        this.order = data.order;
        this.search = data.search;
        this.page = parseInt(data.page);
        this.rows = parseInt(data.rows);
        this.additional = data.filters.split(',').filter(Boolean);
    }

    MvcDatalistFilter.prototype = {
        formUrl: function (search) {
            var encode = encodeURIComponent;
            var url = this.datalist.url.split('?')[0];
            var urlQuery = this.datalist.url.split('?')[1];
            var filter = this.datalist.extend({ ids: [], checkIds: [], selected: [] }, this, search);
            var query = '?' + (urlQuery ? urlQuery + '&' : '') + 'search=' + encode(filter.search);

            for (var i = 0; i < filter.additional.length; i++) {
                var filters = document.querySelectorAll('[name="' + filter.additional[i] + '"]');
                for (var j = 0; j < filters.length; j++) {
                    query += '&' + encode(filter.additional[i]) + '=' + encode(filters[j].value);
                }
            }

            for (i = 0; i < filter.selected.length; i++) {
                query += '&selected=' + encode(filter.selected[i].Id);
            }

            for (i = 0; i < filter.checkIds.length; i++) {
                query += '&checkIds=' + encode(filter.checkIds[i].value);
            }

            for (i = 0; i < filter.ids.length; i++) {
                query += '&ids=' + encode(filter.ids[i].value);
            }

            query += '&sort=' + encode(filter.sort) +
                '&order=' + encode(filter.order) +
                '&rows=' + encode(filter.rows) +
                '&page=' + encode(filter.page) +
                '&_=' + Date.now();

            return url + query;
        }
    };

    return MvcDatalistFilter;
}());
var MvcDatalistDialog = (function () {
    function MvcDatalistDialog(datalist) {
        var dialog = this;
        var element = document.getElementById(datalist.group.dataset.dialog);

        dialog.element = element;
        dialog.datalist = datalist;
        dialog.title = datalist.group.dataset.title;
        dialog.options = { preserveSearch: true, rows: { min: 1, max: 99 }, openDelay: 100 };

        dialog.overlay = new MvcDatalistOverlay(this);
        dialog.table = element.querySelector('table');
        dialog.tableHead = element.querySelector('thead');
        dialog.tableBody = element.querySelector('tbody');
        dialog.rows = element.querySelector('.datalist-rows');
        dialog.pager = element.querySelector('.datalist-pager');
        dialog.header = element.querySelector('.datalist-title');
        dialog.search = element.querySelector('.datalist-search');
        dialog.selector = element.querySelector('.datalist-selector');
        dialog.closeButton = element.querySelector('.datalist-close');
        dialog.error = element.querySelector('.datalist-dialog-error');
        dialog.loader = element.querySelector('.datalist-dialog-loader');
    }

    MvcDatalistDialog.prototype = {
        open: function () {
            var dialog = this;
            var filter = dialog.datalist.filter;
            MvcDatalistDialog.prototype.current = this;

            dialog.error.style.display = 'none';
            dialog.loader.style.display = 'none';
            dialog.header.innerText = dialog.title;
            dialog.rows.value = dialog.limitRows(filter.rows);
            dialog.selected = dialog.datalist.selected.slice();
            dialog.error.innerHTML = dialog.datalist.lang.error;
            dialog.search.placeholder = dialog.datalist.lang.search;
            filter.search = dialog.options.preserveSearch ? filter.search : '';
            dialog.selector.style.display = dialog.datalist.multi ? '' : 'none';
            dialog.selector.innerText = dialog.datalist.lang.select.replace('{0}', dialog.datalist.selected.length);

            dialog.bind();
            dialog.refresh();
            dialog.search.value = filter.search;

            setTimeout(function () {
                if (dialog.isLoading) {
                    dialog.loader.style.opacity = 1;
                    dialog.loader.style.display = '';
                }

                dialog.overlay.show();
            }, dialog.options.openDelay);
        },
        close: function () {
            var dialog = MvcDatalistDialog.prototype.current;
            dialog.datalist.group.classList.remove('datalist-error');

            dialog.datalist.select(dialog.selected, true);
            dialog.datalist.search.focus();
            dialog.datalist.stopLoading();
            dialog.overlay.hide();

            MvcDatalistDialog.prototype.current = null;
        },
        refresh: function () {
            var dialog = this;
            dialog.isLoading = true;
            dialog.error.style.opacity = 0;
            dialog.error.style.display = '';
            dialog.loader.style.display = '';
            var loading = setTimeout(function () {
                dialog.loader.style.opacity = 1;
            }, dialog.datalist.options.loadingDelay);

            dialog.datalist.startLoading({ selected: dialog.selected }, function (data) {
                dialog.isLoading = false;
                clearTimeout(loading);
                dialog.render(data);
            }, function () {
                dialog.isLoading = false;
                clearTimeout(loading);
                dialog.render();
            });
        },

        render: function (data) {
            var dialog = this;
            dialog.pager.innerHTML = '';
            dialog.tableBody.innerHTML = '';
            dialog.tableHead.innerHTML = '';
            dialog.loader.style.opacity = 0;

            setTimeout(function () {
                dialog.loader.style.display = 'none';
            }, dialog.datalist.options.loadingDelay);

            if (data) {
                dialog.error.style.display = 'none';

                dialog.renderHeader(data.Columns);
                dialog.renderBody(data.Columns, data.Rows);
                dialog.renderFooter(data.FilteredRows);
            } else {
                dialog.error.style.opacity = 1;
            }
        },
        renderHeader: function (columns) {
            var row = document.createElement('tr');

            for (var i = 0; i < columns.length; i++) {
                if (!columns[i].Hidden) {
                    row.appendChild(this.createHeaderColumn(columns[i]));
                }
            }

            row.appendChild(document.createElement('th'));
            this.tableHead.appendChild(row);
        },
        renderBody: function (columns, rows) {
            if (!rows.length) {
                var empty = document.createElement('td');
                var row = document.createElement('tr');

                empty.innerHTML = this.datalist.lang.noData;
                empty.colSpan = columns.length + 1;
                row.className = 'datalist-empty';

                this.tableBody.appendChild(row);
                row.appendChild(empty);
            }

            var hasSplit = false;
            var hasSelection = rows.length && this.datalist.indexOf(this.selected, rows[0].Id) >= 0;

            for (var i = 0; i < rows.length; i++) {
                var dataRow = this.createDataRow(rows[i]);
                var selection = document.createElement('td');

                for (var j = 0; j < columns.length; j++) {
                    if (!columns[j].Hidden) {
                        var data = document.createElement('td');
                        data.className = columns[j].CssClass || '';
                        data.innerText = rows[i][columns[j].Key] || '';

                        dataRow.appendChild(data);
                    }
                }

                dataRow.appendChild(selection);

                if (!hasSplit && hasSelection && this.datalist.indexOf(this.selected, rows[i].Id) < 0) {
                    var separator = document.createElement('tr');
                    var content = document.createElement('td');

                    separator.className = 'datalist-split';
                    content.colSpan = columns.length + 1;

                    this.tableBody.appendChild(separator);
                    separator.appendChild(content);

                    hasSplit = true;
                }

                this.tableBody.appendChild(dataRow);
            }
        },
        renderFooter: function (filteredRows) {
            var dialog = this;
            var filter = dialog.datalist.filter;

            dialog.totalRows = filteredRows + dialog.selected.length;
            var totalPages = Math.ceil(filteredRows / filter.rows);
            filter.page = dialog.limitPage(filter.page);

            if (totalPages) {
                var startingPage = Math.floor(filter.page / 4) * 4;

                if (filter.page && 4 < totalPages) {
                    dialog.renderPage('&laquo', 0);
                    dialog.renderPage('&lsaquo;', filter.page - 1);
                }

                for (var i = startingPage; i < totalPages && i < startingPage + 4; i++) {
                    dialog.renderPage(i + 1, i);
                }

                if (4 < totalPages && filter.page < totalPages - 1) {
                    dialog.renderPage('&rsaquo;', filter.page + 1);
                    dialog.renderPage('&raquo;', totalPages - 1);
                }
            } else {
                filter.page = 0;
                dialog.renderPage(1, 0);
            }
        },
        renderPage: function (text, value) {
            var page = document.createElement('button');
            var filter = this.datalist.filter;
            page.type = 'button';
            var dialog = this;

            if (filter.page == value) {
                page.className = 'active';
            }

            page.innerHTML = text;
            page.addEventListener('click', function () {
                filter.page = dialog.limitPage(value);

                dialog.refresh();
            });

            dialog.pager.appendChild(page);
        },

        createHeaderColumn: function (column) {
            var header = document.createElement('th');
            var filter = this.datalist.filter;
            var dialog = this;

            if (column.CssClass) {
                header.classList.add(column.CssClass);
            }

            if (filter.sort == column.Key) {
                header.classList.add('datalist-' + filter.order.toLowerCase());
            }

            header.innerText = column.Header || '';
            header.addEventListener('click', function () {
                filter.order = filter.sort == column.Key && filter.order == 'Asc' ? 'Desc' : 'Asc';
                filter.sort = column.Key;

                dialog.refresh();
            });

            return header;
        },
        createDataRow: function (data) {
            var dialog = this;
            var datalist = this.datalist;
            var row = document.createElement('tr');
            if (datalist.indexOf(dialog.selected, data.Id) >= 0) {
                row.className = 'selected';
            }

            row.addEventListener('click', function () {
                if (!window.getSelection().isCollapsed) {
                    return;
                }

                var index = datalist.indexOf(dialog.selected, data.Id);
                if (index >= 0) {
                    if (datalist.multi) {
                        dialog.selected.splice(index, 1);

                        this.classList.remove('selected');
                    }
                } else {
                    if (datalist.multi) {
                        dialog.selected.push(data);
                    } else {
                        dialog.selected = [data];
                    }

                    this.classList.add('selected');
                }

                if (datalist.multi) {
                    dialog.selector.innerText = dialog.datalist.lang.select.replace('{0}', dialog.selected.length);
                } else {
                    dialog.close();
                }
            });

            return row;
        },

        limitPage: function (value) {
            return Math.max(0, Math.min(value, Math.ceil((this.totalRows - this.selected.length) / this.datalist.filter.rows) - 1));
        },
        limitRows: function (value) {
            value = Math.max(this.options.rows.min, Math.min(parseInt(value), this.options.rows.max));

            return isNaN(value) ? this.datalist.filter.rows : value;
        },

        bind: function () {
            var dialog = this;

            dialog.selector.addEventListener('click', dialog.close);
            dialog.rows.addEventListener('change', dialog.rowsChanged);
            dialog.closeButton.addEventListener('click', dialog.close);
            dialog.search.addEventListener('keyup', dialog.searchChanged);
        },
        rowsChanged: function () {
            var dialog = MvcDatalistDialog.prototype.current;
            var rows = dialog.limitRows(this.value);
            this.value = rows;

            if (dialog.datalist.filter.rows != rows) {
                dialog.datalist.filter.rows = rows;
                dialog.datalist.filter.page = 0;

                dialog.refresh();
            }
        },
        searchChanged: function (e) {
            var input = this;
            var dialog = MvcDatalistDialog.prototype.current;

            dialog.datalist.stopLoading();
            clearTimeout(dialog.searching);
            dialog.searching = setTimeout(function () {
                if (dialog.datalist.filter.search != input.value || e.keyCode == 13) {
                    dialog.datalist.filter.search = input.value;
                    dialog.datalist.filter.page = 0;

                    dialog.refresh();
                }
            }, dialog.datalist.options.searchDelay);
        }
    };

    return MvcDatalistDialog;
}());
var MvcDatalistOverlay = (function () {
    function MvcDatalistOverlay(dialog) {
        this.element = this.findOverlay(dialog.element);
        this.dialog = dialog;

        this.bind();
    }

    MvcDatalistOverlay.prototype = {
        findOverlay: function (element) {
            var overlay = element;

            if (!overlay) {
                throw new Error('Datalist dialog element was not found.');
            }

            while (overlay && !overlay.classList.contains('datalist-overlay')) {
                overlay = overlay.parentElement;
            }

            if (!overlay) {
                throw new Error('Datalist dialog has to be inside a datalist-overlay.');
            }

            return overlay;
        },

        show: function () {
            var body = document.body.getBoundingClientRect();
            if (body.left + body.right < window.innerWidth) {
                var scrollWidth = window.innerWidth - document.body.clientWidth;
                var paddingRight = parseFloat(getComputedStyle(document.body).paddingRight);

                document.body.style.paddingRight = paddingRight + scrollWidth + 'px';
            }

            document.body.classList.add('datalist-open');
            this.element.style.display = 'block';
        },
        hide: function () {
            document.body.classList.remove('datalist-open');
            document.body.style.paddingRight = '';
            this.element.style.display = '';
        },

        bind: function () {
            this.element.addEventListener('click', this.onClick);
            document.addEventListener('keydown', this.onKeyDown);
        },
        onClick: function (e) {
            var targetClasses = (e.target || e.srcElement).classList;

            if (targetClasses.contains('datalist-overlay') || targetClasses.contains('datalist-wrapper')) {
                MvcDatalistDialog.prototype.current.close();
            }
        },
        onKeyDown: function (e) {
            if (e.which == 27 && MvcDatalistDialog.prototype.current) {
                MvcDatalistDialog.prototype.current.close();
            }
        }
    };

    return MvcDatalistOverlay;
}());
var MvcDatalistAutocomplete = (function () {
    function MvcDatalistAutocomplete(datalist) {
        this.datalist = datalist;
        this.activeItem = null;
        this.element = document.createElement('ul');
        this.element.className = 'datalist-autocomplete';
        this.options = { minLength: 1, rows: 20, sort: datalist.filter.sort, order: datalist.filter.order };
    }

    MvcDatalistAutocomplete.prototype = {
        search: function (term) {
            var autocomplete = this;
            var datalist = autocomplete.datalist;

            datalist.stopLoading();
            clearTimeout(autocomplete.searching);
            autocomplete.searching = setTimeout(function () {
                if (term.length < autocomplete.options.minLength || datalist.readonly) {
                    autocomplete.hide();

                    return;
                }

                datalist.startLoading({
                    search: term,
                    rows: autocomplete.options.rows,
                    page: 0,
                    sort: autocomplete.options.sort,
                    order: autocomplete.options.order
                }, function (data) {
                    autocomplete.hide();

                    data = data.Rows.filter(function (row) {
                        return !datalist.multi || datalist.indexOf(datalist.selected, row.Id) < 0;
                    });

                    for (var i = 0; i < data.length; i++) {
                        var item = document.createElement('li');
                        item.innerText = data[i].Label;
                        item.dataset.id = data[i].Id;

                        autocomplete.element.appendChild(item);
                        autocomplete.bind(item, [data[i]]);

                        if (i == 0) {
                            autocomplete.activeItem = item;
                            item.classList.add('active');
                        }
                    }

                    if (data.length) {
                        autocomplete.show();
                    }
                });
            }, autocomplete.datalist.options.searchDelay);
        },
        previous: function () {
            if (!this.element.parentElement) {
                this.search(this.datalist.search.value);

                return;
            }

            this.activeItem.classList.remove('active');
            this.activeItem = this.activeItem.previousElementSibling || this.element.lastElementChild;
            this.activeItem.classList.add('active');
        },
        next: function () {
            if (!this.element.parentElement) {
                this.search(this.datalist.search.value);

                return;
            }

            this.activeItem.classList.remove('active');
            this.activeItem = this.activeItem.nextElementSibling || this.element.firstElementChild;
            this.activeItem.classList.add('active');
        },
        show: function () {
            var search = this.datalist.search.getBoundingClientRect();

            this.element.style.left = search.left + window.pageXOffset + 'px';
            this.element.style.top = search.top + search.height + window.pageYOffset + 'px';

            document.body.appendChild(this.element);
        },
        hide: function () {
            this.activeItem = null;
            this.element.innerHTML = '';

            if (this.element.parentElement) {
                document.body.removeChild(this.element);
            }
        },

        bind: function (item, data) {
            var autocomplete = this;
            var datalist = autocomplete.datalist;

            item.addEventListener('mousedown', function (e) {
                e.preventDefault();
            });

            item.addEventListener('click', function () {
                if (datalist.multi) {
                    datalist.select(datalist.selected.concat(data), true);
                } else {
                    datalist.select(data, true);
                }

                datalist.stopLoading();
                autocomplete.hide();
            });

            item.addEventListener('mouseenter', function () {
                if (autocomplete.activeItem) {
                    autocomplete.activeItem.classList.remove('active');
                }

                this.classList.add('active');
                autocomplete.activeItem = this;
            });
        }
    };

    return MvcDatalistAutocomplete;
}());
var MvcDatalist = (function () {
    function MvcDatalist(element, options) {
        var datalist = this;
        var group = datalist.findDatalist(element);
        if (group.dataset.id) {
            return datalist.instances[parseInt(group.dataset.id)].set(options || {});
        }

        datalist.items = [];
        datalist.events = {};
        datalist.group = group;
        datalist.selected = [];
        datalist.for = group.dataset.for;
        datalist.url = group.dataset.url;
        datalist.multi = group.dataset.multi == 'true';
        datalist.readonly = group.dataset.readonly == 'true';
        datalist.group.dataset.id = datalist.instances.length;
        datalist.options = { searchDelay: 500, loadingDelay: 300 };

        datalist.search = group.querySelector('.datalist-input');
        datalist.browser = group.querySelector('.datalist-browser');
        datalist.control = group.querySelector('.datalist-control');
        datalist.error = group.querySelector('.datalist-control-error');
        datalist.valueContainer = group.querySelector('.datalist-values');
        datalist.values = datalist.valueContainer.querySelectorAll('.datalist-value');

        datalist.instances.push(datalist);
        datalist.filter = new MvcDatalistFilter(datalist);
        datalist.dialog = new MvcDatalistDialog(datalist);
        datalist.autocomplete = new MvcDatalistAutocomplete(datalist);

        datalist.set(options || {});
        datalist.reload(false);
        datalist.cleanUp();
        datalist.bind();
    }

    MvcDatalist.prototype = {
        instances: [],
        lang: {
            search: 'Search...',
            select: 'Select ({0})',
            noData: 'No data found',
            error: 'Error while retrieving records'
        },

        findDatalist: function (element) {
            var datalist = element;

            if (!datalist) {
                throw new Error('Datalist element was not specified.');
            }

            while (datalist && !datalist.classList.contains('datalist')) {
                datalist = datalist.parentElement;
            }

            if (!datalist) {
                throw new Error('Datalist can only be created from within datalist structure.');
            }

            return datalist;
        },

        extend: function () {
            var options = {};

            for (var i = 0; i < arguments.length; i++) {
                for (var key in arguments[i]) {
                    if (arguments[i].hasOwnProperty(key)) {
                        if (Object.prototype.toString.call(options[key]) == '[object Object]') {
                            options[key] = this.extend(options[key], arguments[i][key]);
                        } else {
                            options[key] = arguments[i][key];
                        }
                    }
                }
            }

            return options;
        },
        set: function (options) {
            this.options.loadingDelay = options.loadingDelay == null ? this.options.loadingDelay : options.loadingDelay;
            this.options.searchDelay = options.searchDelay == null ? this.options.searchDelay : options.searchDelay;
            this.autocomplete.options = this.extend(this.autocomplete.options, options.autocomplete);
            this.setReadonly(options.readonly == null ? this.readonly : options.readonly);
            this.dialog.options = this.extend(this.dialog.options, options.dialog);
            this.events = this.extend(this.events, options.events);

            return this;
        },
        setReadonly: function (readonly) {
            var datalist = this;
            datalist.readonly = readonly;

            if (readonly) {
                datalist.search.tabIndex = -1;
                datalist.search.readOnly = true;
                datalist.group.classList.add('datalist-readonly');

                if (datalist.browser) {
                    datalist.browser.tabIndex = -1;
                }
            } else {
                datalist.search.removeAttribute('readonly');
                datalist.search.removeAttribute('tabindex');
                datalist.group.classList.remove('datalist-readonly');

                if (datalist.browser) {
                    datalist.browser.removeAttribute('tabindex');
                }
            }

            datalist.resize();
        },

        browse: function () {
            if (!this.readonly) {
                if (this.browser) {
                    this.browser.blur();
                }

                this.dialog.open();
            }
        },
        reload: function (triggerChanges) {
            var rows = [];
            var datalist = this;
            var originalValue = datalist.search.value;
            var ids = [].filter.call(datalist.values, function (element) {
                return element.value;
            });

            if (ids.length) {
                datalist.startLoading({ ids: ids, rows: ids.length, page: 0 }, function (data) {
                    for (var i = 0; i < ids.length; i++) {
                        var index = datalist.indexOf(data.Rows, ids[i].value);
                        if (index >= 0) {
                            rows.push(data.Rows[index]);
                        }
                    }

                    datalist.select(rows, triggerChanges);
                });
            } else {
                datalist.stopLoading();
                datalist.select(rows, triggerChanges);

                if (!datalist.multi && datalist.search.name) {
                    datalist.search.value = originalValue;
                }
            }
        },
        select: function (data, triggerChanges) {
            var datalist = this;
            triggerChanges = triggerChanges == null || triggerChanges;

            if (datalist.events.select && datalist.events.select.call(datalist, data, triggerChanges) === false) {
                return;
            }

            if (triggerChanges && data.length == datalist.selected.length) {
                triggerChanges = false;
                for (var i = 0; i < data.length && !triggerChanges; i++) {
                    triggerChanges = data[i].Id != datalist.selected[i].Id;
                }
            }

            datalist.selected = data;

            if (datalist.multi) {
                datalist.search.value = '';
                datalist.valueContainer.innerHTML = '';
                datalist.items.forEach(function (item) {
                    item.parentElement.removeChild(item);
                });

                datalist.items = datalist.createSelectedItems(data);
                datalist.items.forEach(function (item) {
                    datalist.control.insertBefore(item, datalist.search);
                });

                datalist.values = datalist.createValues(data);
                datalist.values.forEach(function (value) {
                    datalist.valueContainer.appendChild(value);
                });

                datalist.resize();
            } else if (data.length) {
                datalist.values[0].value = data[0].Id;
                datalist.search.value = data[0].Label;
            } else {
                datalist.values[0].value = '';
                datalist.search.value = '';
            }

            if (triggerChanges) {
                var change = null;
                if (typeof (Event) === 'function') {
                    change = new Event('change');
                } else {
                    change = document.createEvent('Event');
                    change.initEvent('change', true, true);
                }

                datalist.search.dispatchEvent(change);
                [].forEach.call(datalist.values, function (value) {
                    value.dispatchEvent(change);
                });
            }
        },
        selectFirst: function (triggerChanges) {
            var datalist = this;

            datalist.startLoading({ search: '', rows: 1, page: 0 }, function (data) {
                datalist.select(data.Rows, triggerChanges);
            });
        },
        selectSingle: function (triggerChanges) {
            var datalist = this;

            datalist.startLoading({ search: '', rows: 2, page: 0 }, function (data) {
                if (data.Rows.length == 1) {
                    datalist.select(data.Rows, triggerChanges);
                } else {
                    datalist.select([], triggerChanges);
                }
            });
        },

        createSelectedItems: function (data) {
            var items = [];

            for (var i = 0; i < data.length; i++) {
                var button = document.createElement('button');
                button.className = 'datalist-deselect';
                button.innerText = '×';
                button.type = 'button';

                var item = document.createElement('div');
                item.innerText = data[i].Label || '';
                item.className = 'datalist-item';
                item.appendChild(button);
                items.push(item);

                this.bindDeselect(button, data[i].Id);
            }

            return items;
        },
        createValues: function (data) {
            var inputs = [];

            for (var i = 0; i < data.length; i++) {
                var input = document.createElement('input');
                input.className = 'datalist-value';
                input.value = data[i].Id;
                input.type = 'hidden';
                input.name = this.for;

                inputs.push(input);
            }

            return inputs;
        },

        startLoading: function (search, success, error) {
            var datalist = this;

            datalist.stopLoading();
            datalist.loading = setTimeout(function () {
                datalist.autocomplete.hide();
                datalist.group.classList.add('datalist-loading');
            }, datalist.options.loadingDelay);
            datalist.group.classList.remove('datalist-error');

            datalist.request = new XMLHttpRequest();
            datalist.request.open('GET', datalist.filter.formUrl(search), true);
            datalist.request.setRequestHeader('X-Requested-With', 'XMLHttpRequest');

            datalist.request.onload = function () {
                if (200 <= datalist.request.status && datalist.request.status < 400) {
                    datalist.stopLoading();

                    success(JSON.parse(datalist.request.responseText));
                } else {
                    datalist.request.onerror();
                }
            };

            datalist.request.onerror = function () {
                datalist.group.classList.add('datalist-error');
                datalist.error.title = datalist.lang.error;
                datalist.autocomplete.hide();
                datalist.stopLoading();

                if (error) {
                    error();
                }
            };

            datalist.request.send();
        },
        stopLoading: function () {
            if (this.request && this.request.readyState != 4) {
                this.request.abort();
            }

            clearTimeout(this.loading);
            this.group.classList.remove('datalist-loading');
        },

        bindDeselect: function (close, id) {
            var datalist = this;

            close.addEventListener('click', function () {
                datalist.select(datalist.selected.filter(function (value) { return value.Id != id; }), true);
                datalist.search.focus();
            });
        },
        indexOf: function (selection, id) {
            for (var i = 0; i < selection.length; i++) {
                if (selection[i].Id == id) {
                    return i;
                }
            }

            return -1;
        },
        cleanUp: function () {
            var data = this.group.dataset;

            delete data.readonly;
            delete data.filters;
            delete data.dialog;
            delete data.search;
            delete data.multi;
            delete data.order;
            delete data.title;
            delete data.page;
            delete data.rows;
            delete data.sort;
            delete data.url;
        },
        resize: function () {
            var datalist = this;

            if (datalist.items.length) {
                var style = getComputedStyle(datalist.control);
                var contentWidth = datalist.control.clientWidth;
                var lastItem = datalist.items[datalist.items.length - 1];
                contentWidth -= parseFloat(style.paddingLeft) + parseFloat(style.paddingRight);
                var widthLeft = Math.floor(contentWidth - lastItem.offsetLeft - lastItem.offsetWidth);

                if (widthLeft > contentWidth / 3) {
                    style = getComputedStyle(datalist.search);
                    widthLeft -= parseFloat(style.marginLeft) + parseFloat(style.marginRight) + 4;
                    datalist.search.style.width = widthLeft + 'px';
                } else {
                    datalist.search.style.width = '';
                }
            } else {
                datalist.search.style.width = '';
            }
        },
        bind: function () {
            var datalist = this;

            window.addEventListener('resize', function () {
                datalist.resize();
            });

            datalist.search.addEventListener('focus', function () {
                datalist.group.classList.add('datalist-focus');
            });

            datalist.search.addEventListener('blur', function () {
                datalist.stopLoading();
                datalist.autocomplete.hide();
                datalist.group.classList.remove('datalist-focus');

                var originalValue = this.value;
                if (!datalist.multi && datalist.selected.length) {
                    if (datalist.selected[0].Label != this.value) {
                        datalist.select([], true);
                    }
                } else {
                    this.value = '';
                }

                if (!datalist.multi && datalist.search.name) {
                    this.value = originalValue;
                }
            });

            datalist.search.addEventListener('keydown', function (e) {
                if (e.which == 8 && !this.value.length && datalist.selected.length) {
                    datalist.select(datalist.selected.slice(0, -1), true);
                } else if (e.which == 38) {
                    e.preventDefault();

                    datalist.autocomplete.previous();
                } else if (e.which == 40) {
                    e.preventDefault();

                    datalist.autocomplete.next();
                } else if (e.which == 13 && datalist.autocomplete.activeItem) {
                    e.preventDefault();

                    var click = null;
                    if (typeof (Event) === 'function') {
                        click = new Event('click');
                    } else {
                        click = document.createEvent('Event');
                        click.initEvent('click', true, true);
                    }

                    datalist.autocomplete.activeItem.dispatchEvent(click);
                }
            });
            datalist.search.addEventListener('input', function () {
                if (!this.value.length && !datalist.multi && datalist.selected.length) {
                    datalist.autocomplete.hide();
                    datalist.select([], true);
                }

                datalist.autocomplete.search(this.value);
            });

            if (datalist.browser) {
                datalist.browser.addEventListener('click', function () {
                    datalist.browse();
                });
            }

            for (var i = 0; i < datalist.filter.additional.length; i++) {
                var inputs = document.querySelectorAll('[name="' + datalist.filter.additional[i] + '"]');

                for (var j = 0; j < inputs.length; j++) {
                    inputs[j].addEventListener('change', function () {
                        datalist.stopLoading();
                        datalist.filter.page = 0;

                        if (datalist.events.filterChange && datalist.events.filterChange.call(datalist, this) === false) {
                            return;
                        }

                        if (datalist.selected.length) {
                            var rows = [];
                            var ids = [].filter.call(datalist.values, function (element) { return element.value; });

                            datalist.startLoading({ checkIds: ids, rows: ids.length }, function (data) {
                                for (var i = 0; i < ids.length; i++) {
                                    var index = datalist.indexOf(data.Rows, ids[i].value);
                                    if (index >= 0) {
                                        rows.push(data.Rows[index]);
                                    }
                                }

                                datalist.select(rows, true);
                            }, function () {
                                datalist.select(rows, true);
                            });
                        }
                    });
                }
            }
        }
    };

    return MvcDatalist;
}());
