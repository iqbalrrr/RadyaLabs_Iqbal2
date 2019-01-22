Datalist = {
    init: function () {
        if (window.MvcDatalist) {
            var lang = document.documentElement.lang;
            MvcDatalist.prototype.lang = window.cultures.datalist[lang];

            [].forEach.call(document.getElementsByClassName('datalist'), function (element) {
                new MvcDatalist(element);
            });
        }
    }
};
