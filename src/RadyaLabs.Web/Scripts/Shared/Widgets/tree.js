Tree = {
    init: function () {
        [].forEach.call(document.getElementsByClassName('mvc-tree'), function (element) {
            new MvcTree(element);
        });
    }
};
