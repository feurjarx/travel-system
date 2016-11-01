$(function () {
    $.fn.mask = function (type) {

        var $mask = $('<div>',{
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
        });

        if (type === 'center') {

            $(this).prepend($mask.css({
                position: 'fixed',
                'z-index': 99999,
                right: 0,
                left: 0,
                bottom: '50%'
            }));

        } else {

            $(this).append($mask)
        }
    };

    $.fn.unmask = function () {
        $(this).find('.spinner').fadeOut();
    }
});