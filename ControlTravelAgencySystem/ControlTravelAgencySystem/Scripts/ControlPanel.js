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
    var reloadText = '<br>Обновление страницы <i class="fa fa-circle-o-notch fa-spin fa-fw"></i>';

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

    // START CALLOUT CRUD ZONE
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

                                notification({
                                    text: 'Успешно! Объект(-ы) удален(-ы)' + reloadText,
                                    type: 'success'
                                }, 5000);

                                $form.modal('hide').get(0).reset();
                                setTimeout(function () {
                                    location.reload();
                                }, 3000);
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
    // END CALLOUT CRUD ZONE

    // START READ ZONE
    // ...
    // END READ ZONE

    // START REMOVE ZONE
    $(document).on('click', '.btn-entity-remove', function () {

        var $activeRow = $(this).closest('tr');

        var warningText =  $activeRow.data('warning-text') || '';
        if (warningText) {
            warningText = '<br><span class="text-danger">' + warningText + '</span>';
        }

        notification({
            text: 'Вы действительно желаете удалить объект "' + ($activeRow.data('title') || '') + '"' + warningText,
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

                    $activeRow.find('td').first().mask();

                    $.ajax({
                        method: 'delete',
                        url: $activeRow.data('delete-url'),
                        success: function (response) {

                            if (response.error) {

                                console.error(response.error);

                                notification({
                                    text: 'Ошибка! ' + response.error,
                                    type: 'error'
                                }, 5000);

                            } else {

                                notification({
                                    text: 'Успешно! Объект удален',
                                    type: 'success'
                                }, 5000);

                                $activeRow.fadeOut();
                            }
                        },
                        error: function (err) {
                            console.error(err);
                        },
                        complete: function () {
                            $activeRow.find('td').first().unmask();
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

    function preSubmit($form) {

        var result;

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
                            $form.find('[name="number"]').highlightElement('error');
                            errors.push('Некорректно заполнено поле номера помещения');
                        }
                    }

                    break;

                case 'employee':

                    switch (param.name) {
                        case 'fullname':

                            if (param.value && param.value.trim().split(' ').length != 3) {
                                $form.find('[name="fullname"]').highlightElement('error');
                                errors.push('Некорректно заполнено поле ФИО');
                            }

                            break;

                        case 'passport_series':

                            match = param.value.match(/\d/g);
                            if (!match || match.length != 4) {
                                $form.find('[name="passport_series"]').highlightElement('error');
                                errors.push('Некорректно указана серия паспорта');
                            }

                            break;

                        case 'passport_code':

                            match = param.value.match(/\d/g);
                            if (!match || match.length != 6) {
                                $form.find('[name="passport_code"]').highlightElement('error');
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

            result = false;

        } else {

            result = paramsList;
        }

        return result;
    }
    // START EDIT ZONE
    $(document).on('click', '.btn-modal-edit', function () {
        var $activeRow = $(this).closest('tr');
        var $modalClone = $($activeRow.data('modal-target')).clone();
        var $template = $modalClone.find('.hbs');
        var modalBodyHtml = render.getPainterFor($template)($activeRow.data('json'));
        $modalClone.find('.modal-title').text('Редактирование объекта');
        $modalClone.find('.modal-body').html(modalBodyHtml);

        $modalClone
            .on('shown.bs.modal', function (e) {

                if ($modalClone.data('entity') === 'employee') {
                    var $birthdayAtDatetimepicker = $('#edit-birthday-at-datetimepicker');

                    if ($birthdayAtDatetimepicker.data('DateTimePicker')) {
                        $birthdayAtDatetimepicker.data('DateTimePicker').destroy();
                    }

                    var ts = $activeRow.data('json')['person']['birthday_at'];
                    $birthdayAtDatetimepicker.datetimepicker({
                        defaultDate: new Date(ts * 1000),
                        locale: 'ru',
                        format: 'DD/MM/YYYY',
                        viewMode: 'years'
                    });
                }
            })
            .on('hidden.bs.modal', function (e) {
                $(this).remove();
            })
            .submit(function (event) {
                event.preventDefault();

                var $form = $(this);

                var paramsList = preSubmit($form);
                if (paramsList) {
                    $form.find('.modal-title').mask();

                    $.ajax({
                        method: 'post',
                        url: $form.data('edit-url') + '/' + $activeRow.data('id'),
                        data: paramsList,
                        success: function (response) {

                            if (response.error) {

                                console.error(response.error);

                                notification({
                                    text: 'Ошибка! <br> Операция с объектом завершилась с ошибкой. Обратитесь в техподдержку сайта',
                                    type: 'error'
                                }, 5000);

                            } else {

                                notification({
                                    text: 'Успешно! Объект изменен' + reloadText,
                                    type: 'success'
                                }, 5000);

                                $form.modal('hide').get(0).reset();

                                setTimeout(function () {
                                    location.reload();
                                }, 3000);
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

        $modalClone.modal('show');
    });
    // END EDIT ZONE

    // START CREATE ZONE
    $('form.add-entity-modal').submit(function () {
        event.preventDefault();

        var $form = $(this);

        var paramsList = preSubmit($form);
        if (paramsList) {
            $form.find('.modal-title').mask();

            $.ajax({
                method: 'post',
                url: $form.data('create-url'),
                data: paramsList,
                success: function (response) {

                    if (response.error) {

                        console.error(response.error);

                        notification({
                            text: 'Ошибка! <br> Операция с объектом завершилась с ошибкой. Обратитесь в техподдержку сайта',
                            type: 'error'
                        }, 5000);

                    } else {

                        notification({
                            text: 'Успешно! Объект добавлен' + reloadText,
                            type: 'success'
                        }, 5000);

                        $form.modal('hide').get(0).reset();
                        setTimeout(function () {
                            location.reload();
                        }, 3000);
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
});