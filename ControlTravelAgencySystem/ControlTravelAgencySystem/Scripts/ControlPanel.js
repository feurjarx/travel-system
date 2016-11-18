var cache = {
    data: {},
    get: function (key) {
        return this.data[key];
    },
    set: function (key, value) {
        this.data[key] = value;
    }
};

$(function () {

    var domControl = {

        _calloutsRemoveBtnVisible: false,

        get $calloutsRemoveBtn() {
            return $('#actions-remove-btn');
        },

        get calloutsRemoveBtnVisible() {
            return this._calloutsRemoveBtnVisible;
        },
        set calloutsRemoveBtnVisible(on) {

            if (on) {

                this.$calloutsRemoveBtn.fadeIn();

            } else {

                this.$calloutsRemoveBtn.fadeOut();
            }

            this._calloutsRemoveBtnVisible = on;
        }
    };

    var render = {
        painters: {},
        getPainterFor: function (object) {

            var result;

            if (object instanceof jQuery) {

                var html = object.html();
                var key = html.length;
                if (!this.painters[key]) {
                    this.painters[key] = Handlebars.compile(html);
                }

                result = this.painters[key];

            } else {

                var selector = object;

                if (!this.painters[selector]) {
                    this.painters[selector] = Handlebars.compile($(selector).html());
                }

                result = this.painters[selector];
            }

            return result;
        }
    };
    // START CREATE ZONE
    $('form.add-entity-modal').submit(function () {
        event.preventDefault();

        var $form = $(this);

        // clear previous validation elements
        $form.find('.has-error').removeClass('has-error');
        $form.find('.has-feedback').removeClass('has-feedback');

        var paramsList = $form.serializeArray();
        var errors = [], match;

        var entityName = $form.data('entity');
        paramsList.forEach(function (param) {

            switch (entityName) {

                case 'hotel':
                case 'tour':
                    break;

                case 'room':

                    if (param.name === 'number') {
                        match = param.value.match(/\d/g);
                        if (!match) {
                            $('#number-input').highlightElement('error');
                            errors.push('Некорректно заполнено поле номера помещения');
                        }
                    }

                    break;

                case 'employee':

                    switch (param.name) {
                        case 'fullname':

                            if (param.value && param.value.trim().split(' ').length != 3) {

                                $('#fullname-input').highlightElement('error');
                                errors.push('Некорректно заполнено поле ФИО');
                            }

                            break;

                        case 'passport_series':

                            match = param.value.match(/\d/g);
                            if (!match || match.length != 6 ) {
                                $('#passport-series-input').highlightElement('error');
                                errors.push('Некорректно указана серия паспорта');
                            }

                            break;

                        case 'passport_code':

                            match = param.value.match(/\d/g);
                            if (!match || match.length != 4 ) {
                                $('#passport-code-input').highlightElement('error');
                                errors.push('Некорректно указан номер паспорта');
                            }

                            break;
                    }

                    break;

                default:
                    throw new Error('Unexcepted entity');

            }
        });

        if (errors.length) {

            var warningLines = '';
            errors.forEach(function (err, i) {
                warningLines += (i + 1) + '. ' + err + '<br>';
            });

            notification({
                text: 'Внимание! <br> ' + warningLines,
                type: 'warning'
            }, 5000);

        } else {

            $form.find('.modal-title').mask();

            $.ajax({
                method: 'post',
                url: $form.data('create-url'),
                data: paramsList,
                success: function (response) {

                    if (response.error) {

                        console.error(response.error);

                        notification({
                            text: 'Ошибка! <br> Создание объекта завершилось с ошибкой. Обратитесь в техподдержку сайта',
                            type: 'error'
                        }, 5000);

                    } else {

                        $form.modal('hide').get(0).reset();
                        location.reload();
                    }
                },
                error: function (err) {
                    console.error(err);

                    notification({
                        text: 'Ошибка! <br> Отказ на сервере. Обратитесь в техподдержку сайта',
                        type: 'error'
                    });
                },
                complete: function () {
                    $form.find('.modal-title').unmask();
                }
            });
        }
    });
    // END CREATE ZONE

    // START READ ZONE
    var $calloutsTable = $('#callouts-table');
    $calloutsTable.find('.checkbox').on('change', function () {
        domControl.calloutsRemoveBtnVisible = $(this).closest('table').find('input:checked').length > 0;
    });
    $calloutsTable.find('tbody > tr').on('click', function (event) {

        if (event.target.type === 'checkbox'
            ||
            !$(event.target).index()
            ||
            $(event.target).hasClass('checkbox')
            ||
            event.target.tagName === 'LABEL'
        ) {
            return;
        }

        var successAction = function (params) {

            $('#suggestions-modal')
                .find('.modal-body')
                .html(render.getPainterFor('#suggestions-box')(params))
                .end()
                .find('.modal-title .fullname')
                .text(params.fullname)
                .end()
                .modal()
            ;
        };

        var $calloutItem = $(this);
        var calloutId = $calloutItem.data('id');
        var fullname = $calloutItem.data('fullname');

        var responseFromCache = cache.get(calloutId);
        if (responseFromCache) {
            successAction(responseFromCache);

        } else {

            $calloutItem.find('td').first().mask();
            $.ajax({
                method: 'post',
                url: '/callout/suggestions/' + calloutId,
                success: function (response) {
                    response.fullname = fullname;
                    successAction(response);
                    cache.set(calloutId, response);
                },
                error: function (err) {
                    console.error(err);
                },
                complete: function () {
                    $calloutItem.find('td').first().unmask();
                }
            });
        }
    });
    // END READ ZONE

    // START REMOVE ZONE
    domControl.$calloutsRemoveBtn.on('click', function () {

        var calloutsIds = [];
        $calloutsTable.find('input[type="checkbox"]:checked').each(function (_, elem) {
            calloutsIds.push($(elem).closest('tr').data('id'));
        });

        notification({
            text: 'Вы действительно желаете удалить ' + (calloutsIds.length > 1 ? 'отмеченные заявки (x' + calloutsIds.length + ')'  : 'отмеченную заявку'),
            type: 'confirm',
            layout: 'top',
            dismissQueue: true,
            animation: {
                open: {height: 'toggle'},
                close: {height: 'toggle'},
                easing: 'swing',
                speed: 500
            },
            killer: true,
            buttons: [{
                addClass: 'btn btn-danger',
                text: 'Да',
                onClick: function($noty) {

                    var $body = $('body');
                    $body.mask('center');

                    $.ajax({
                        method: 'delete',
                        url: '/callout/delete',
                        data: {
                            callouts_ids: calloutsIds
                        },
                        success: function (response) {

                            if (response.error) {

                                console.error(response.error);

                            } else {

                                location.reload();
                            }
                        },
                        error: function (err) {
                            console.error(err);
                        },
                        complete: function () {
                            $body.unmask();

                            $noty.close();
                        }
                    });
                }
            }, {
                addClass: 'btn btn-default',
                text: 'Отмена',
                onClick: function($noty) {
                    $noty.close();
                }
            }]
        });
    });
    $('.btn-employee-remove').on('click', function () {

        var $item = $(this).closest('tr');
        var employeeId = $item.data('id');

        notification({
            text: 'Вы действительно желаете удалить учетную запись сотрудника',
            type: 'confirm',
            layout: 'top',
            dismissQueue: true,
            animation: {
                open: {height: 'toggle'},
                close: {height: 'toggle'},
                easing: 'swing',
                speed: 500
            },
            killer: true,
            buttons: [{
                addClass: 'btn btn-danger',
                text: 'Да',
                onClick: function($noty) {

                    $item.find('td').first().mask();

                    $.ajax({
                        method: 'delete',
                        url: '/employee/delete/' + employeeId,
                        success: function (response) {

                            if (response.error) {

                                console.error(response.error);

                            } else {

                                $item.fadeOut();
                            }
                        },
                        error: function (err) {
                            console.error(err);
                        },
                        complete: function () {
                            $item.find('td').first().unmask();
                            $noty.close();
                        }
                    });
                }
            }, {
                addClass: 'btn btn-default',
                text: 'Отмена',
                onClick: function($noty) {
                    $noty.close();
                }
            }]
        });
    });
    // END REMOVE ZONE

    // START EDIT ZONE
    $(document).on('click', '.btn-modal-edit', function () {
        var $activeRow = $(this).closest('tr');
        var $modalClone = $($activeRow.data('modal-target')).clone();
        var $template = $modalClone.find('.hbs');
        var modalBodyHtml = render.getPainterFor($template)($activeRow.data('json'));
        $modalClone.find('.modal-title').text('Редактирование объекта');
        $modalClone.find('.modal-body').html(modalBodyHtml);
        $modalClone.modal('show');
        $modalClone
            .on('hidden.bs.modal', function (e) {
                $(this).remove();
            })
            .submit(function (e) {
                e.preventDefault();
                debugger
            });
    });
    // END EDIT ZONE

});