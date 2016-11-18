Handlebars.registerHelper('ifCond', function (v1, operator, v2, options) {

    switch (operator) {
        case '==':
            return (v1 == v2) ? options.fn(this) : options.inverse(this);
        case '===':
            return (v1 === v2) ? options.fn(this) : options.inverse(this);
        case '<':
            return (v1 < v2) ? options.fn(this) : options.inverse(this);
        case '<=':
            return (v1 <= v2) ? options.fn(this) : options.inverse(this);
        case '>':
            return (v1 > v2) ? options.fn(this) : options.inverse(this);
        case '>=':
            return (v1 >= v2) ? options.fn(this) : options.inverse(this);
        case '&&':
            return (v1 && v2) ? options.fn(this) : options.inverse(this);
        case '||':
            return (v1 || v2) ? options.fn(this) : options.inverse(this);
        default:
            return options.inverse(this);
    }
});

Handlebars.registerHelper('debug', function (v1) {
    console.log(v1);
});

Handlebars.registerHelper('slice', function (value, start, end) {
    return end && typeof end === "number" ? value.slice(start, end) : value.slice(start);
});

Handlebars.registerHelper("math", function (lvalue, operator, rvalue, options) {
    if (arguments.length < 4) {
        // Operator omitted, assuming "+"
        options = rvalue;
        rvalue = operator;
        operator = "+";
    }

    lvalue = parseFloat(lvalue);
    rvalue = parseFloat(rvalue);

    return {
        "+": lvalue + rvalue,
        "-": lvalue - rvalue,
        "*": lvalue * rvalue,
        "/": lvalue / rvalue,
        "%": lvalue % rvalue
    }[operator];
});

function notification(options, timeout) {
    timeout = timeout || 2000;

    noty($.extend({}, {
        template: $('<div>', {
            class: 'noty_message',
            html: [
                $('<strong>', {
                    class: 'noty_text'
                }),
                $('<div>', {
                    class: 'noty_close'
                })
            ]
        }),
        layout: 'topRight',
        closeWith: ['click'],
        timeout: timeout,
        animation: {
            open: 'animated wobble',
            close: 'animated flipOutY'
        }
    }, options));
}

$(function () {
    /**
     * @param arg
     * @returns {jQuery}
     */
    $.fn.highlightElement = function (arg) {

        $(this).off('focusin');

        var opts;

        if (arguments.length == 2
            &&
            typeof arguments[0] === 'string'
            &&
            typeof +arguments[1] === 'number'
        ) {

            opts = {
                type: arguments[0],
                delay: +arguments[1]
            }

        } else {

            switch (true) {
                case typeof arg === 'string':

                    opts = { type: arg };
                    break;

                case typeof +arg === 'number' && !isNaN(+arg):

                    opts = { delay: arg };
                    break;

                default:

                    opts = arg || {};
            }
        }

        var glyphiconClassByTypeMap = {
            success: 'glyphicon-ok',
            warning: 'glyphicon-warning-sign',
            error: 'glyphicon-remove'
        };

        opts = $.extend({}, {

            type: 'error',
            $parent: $(this).parent(),
            feedback: true

        }, opts);

        $(this).next('.form-control-feedback').remove();
        opts.$parent
            .removeClass('has-feedback')
            .removeClass('has-error')
            .removeClass('has-warning')
            .removeClass('has-success')
        ;

        if (['error', 'warning', 'success'].indexOf(opts.type) !== -1) {

            if (opts.feedback) {
                opts.$parent
                    .addClass('has-' + opts.type)
                    .addClass('has-feedback')
                    .append($('<span>', {
                        class: 'form-control-feedback glyphicon ' + glyphiconClassByTypeMap[opts.type]
                    }));

                if (opts.type === 'error') {
                    $(this).focus();
                }

            } else {

                opts.$parent.addClass('has-' + opts.type);
            }

            var timerId, hide = function () {

                if (opts.feedback) {
                    opts.$parent
                        .removeClass('has-feedback')
                        .removeClass('has-' + opts.type)
                        .find('span.form-control-feedback').remove();

                } else {

                    opts.$parent.removeClass('has-' + opts.type)
                }

                $(this).off('focusin');
            };

            hide = hide.bind(this);

            if (opts.delay) {

                timerId = setTimeout(hide, +opts.delay);
            }

            $(this).focusin(function () {

                if (timerId) {
                    clearTimeout(timerId);
                }

                hide();
            });

        }

        return $(this);
    };
});