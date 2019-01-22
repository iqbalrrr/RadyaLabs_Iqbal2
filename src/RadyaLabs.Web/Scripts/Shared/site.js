// Widgets init
(function () {
    DatePicker.init();
    Validator.init();
    Datalist.init();
    Alerts.init();
    Header.init();
    Menu.init();
    Grid.init();
    Tree.init();
})();

// Read only binding
(function () {
    $(document).on('click', '[readonly]', function () {
        return false;
    });

    var widgets = $('.widget-box.readonly');
    widgets.find('input').attr({ readonly: 'readonly', tabindex: -1 });
    widgets.find('textarea').attr({ readonly: 'readonly', tabindex: -1 });

    if (window.MvcDatalist) {
        widgets.find('.datalist').each(function (i, element) {
            new MvcDatalist(element, { readonly: true });
        });
    }

    if (window.MvcTree) {
        widgets.find('.mvc-tree').each(function (i, element) {
            new MvcTree(element, { readonly: true });
        });
    }
})();

// Input focus binding
(function () {
    var invalidInput = $('.content-container .input-validation-error:visible:not([readonly],.datepicker,.datetimepicker):first');
    if (invalidInput.length > 0) {
        invalidInput[0].setSelectionRange(invalidInput[0].value.length, invalidInput[0].value.length);
        invalidInput.focus();
    } else {
        var input = $('.content-container input:text:visible:not([readonly],.datepicker,.datetimepicker):first');
        if (input.length > 0) {
            input[0].setSelectionRange(input[0].value.length, input[0].value.length);
            input.focus();
        }
    }
})();
