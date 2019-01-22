Header = {
    init: function () {
        $('.header .dropdown').on('mouseleave', function () {
            if (this.classList.contains('show')) {
                this.children[0].Dropdown.toggle();
            }
        });
    }
};
