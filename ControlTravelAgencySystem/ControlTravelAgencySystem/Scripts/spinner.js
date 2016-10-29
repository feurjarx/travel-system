$(function () {
    $.fn.mask = function () {
        $(this).append($('<div>',{
            class: 'sk-three-bounce spinner',
            html: [
                $('<div>', {
                    class: 'sk-child sk-bounce1'
                }),
                $('<div>', {
                    class: 'sk-child sk-bounce2'
                }),
                $('<div>', {
                    class: 'sk-child sk-bounce3'
                })
            ]
        }))
    };

    $.fn.unmask = function () {
        $(this).find('.spinner').fadeOut();
    }
});