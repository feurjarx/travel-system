var cache = {
    data: {},
    get: function (key) {
        return this.data[key];
    },
    set: function (key, value) {
        this.data[key] = value;
    }
};

var monthConverter = {
    "дек": 11,
    "ноя": 10,
    "окт": 9,
    "сен": 8,
    "авг": 7,
    "июл": 6,
    "июн": 5,
    "май": 4,
    "апр": 3,
    "мар": 2,
    "фев": 1,
    "янв": 0
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

        var $calloutItem = $(this);
        var calloutId = $calloutItem.data('id');

        var successAction = function (params) {

            $('#suggestions-modal')
                .find('.modal-body')
                .html(render.getPainterFor('#suggestions-box')(params))
                .end()
                .find('.modal-title .fullname')
                .text(params.fullname)
                .end()
                .modal()
                .data('callout-id', calloutId)
            ;
        };

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
                                    text: 'Успешно! Объект(-ы) удален(-ы)',
                                    type: 'success'
                                }, 5000);


                                $calloutsTable.find('input[type="checkbox"]:checked').each(function (_, elem) {

                                    $(elem).closest('tr').fadeOut(300, function () {
                                        $(this).remove();
                                    });
                                });
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
        if (!$activeRow.length) {
            $activeRow = $(this).closest('.panel');
        }

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

                                $activeRow.fadeOut(300, function () {
                                    $(this).remove();
                                });
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

                case 'excursion_order':
                case 'hotel_service_order':
                case 'transfer':
                case 'airticket':
                case 'callout_room':
                case 'hotel_service':
                case 'excursion':
                case 'route':
                case 'flight':
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
    // START EDIT
    $(document).on('click', '.btn-modal-edit', function () {

        var $activeItem = $(this).closest('tr');
        if (!$activeItem.length) {
            $activeItem = $(this).closest('.panel');
        }

        var $modalClone = $($activeItem.data('modal-target')).clone();
        var $template = $modalClone.find('.hbs');
        var modalBodyHtml = render.getPainterFor($template)($activeItem.data('json'));
        $modalClone.find('.modal-title').text('Редактирование объекта');
        $modalClone.find('.modal-body').html(modalBodyHtml);

        $modalClone
            .on('shown.bs.modal', function (e) {

                var $datetimepickerBlock, ts, startingTime, startingDateTime, timeParts, dateParts;
                switch (true) {

                    case $modalClone.data('entity') === 'excursion_order':

                        $datetimepickerBlock = $('#edit-starting-at-datetimepicker');
                        if ($datetimepickerBlock.data('DateTimePicker')) {
                            $datetimepickerBlock.data('DateTimePicker').destroy();
                        }

                        ts = $activeItem.data('json')['starting_at'];
                        $datetimepickerBlock.datetimepicker({
                            defaultDate: new Date(ts * 1000),
                            locale: 'ru'
                        });

                        break;

                    case $modalClone.data('entity') === 'hotel_service_order':

                        $datetimepickerBlock = $('#edit-provision-at-datetimepicker');
                        if ($datetimepickerBlock.data('DateTimePicker')) {
                            $datetimepickerBlock.data('DateTimePicker').destroy();
                        }

                        ts = $activeItem.data('json')['provision_at'];
                        $datetimepickerBlock.datetimepicker({
                            defaultDate: new Date(ts * 1000),
                            locale: 'ru'
                        });

                        break;

                    case $modalClone.data('entity') === 'transfer':

                        $datetimepickerBlock = $('#edit-starting-date-datetimepicker');
                        if ($datetimepickerBlock.data('DateTimePicker')) {
                            $datetimepickerBlock.data('DateTimePicker').destroy();
                        }

                        startingDateTime = $activeItem.data('json')['starting_date'];
                        if (startingDateTime) {
                            dateParts = startingDateTime.split(' ');
                            $datetimepickerBlock.datetimepicker({
                                defaultDate: new Date(dateParts[2], monthConverter[dateParts[1]], dateParts[0], 0, 0, 0),
                                locale: 'ru',
                                format: 'DD/MM/YYYY'
                            });
                        }

                        break;

                    case $modalClone.data('entity') === 'airticket':

                        $datetimepickerBlock = $('#edit-departure-at-datetimepicker');
                        if ($datetimepickerBlock.data('DateTimePicker')) {
                            $datetimepickerBlock.data('DateTimePicker').destroy();
                        }

                        ts = $activeItem.data('json')['departure_at'];
                        $datetimepickerBlock.datetimepicker({
                            defaultDate: new Date(ts * 1000),
                            locale: 'ru'
                        });

                        break;

                    case $modalClone.data('entity') === 'callout_room':

                        $datetimepickerBlock = $('#edit-start-living-at-datetimepicker');
                        if ($datetimepickerBlock.data('DateTimePicker')) {
                            $datetimepickerBlock.data('DateTimePicker').destroy();
                        }

                        ts = $activeItem.data('json')['start_living_at'];
                        $datetimepickerBlock.datetimepicker({
                            defaultDate: new Date(ts * 1000),
                            locale: 'ru'
                        });

                        break;

                    case $modalClone.data('entity') === 'employee':

                        $datetimepickerBlock = $('#edit-birthday-at-datetimepicker');
                        if ($datetimepickerBlock.data('DateTimePicker')) {
                            $datetimepickerBlock.data('DateTimePicker').destroy();
                        }

                        ts = $activeItem.data('json')['person']['birthday_at'];
                        $datetimepickerBlock.datetimepicker({
                            defaultDate: new Date(ts * 1000),
                            locale: 'ru',
                            format: 'DD/MM/YYYY',
                            viewMode: 'years'
                        });

                        break;
                    case $modalClone.data('entity') === 'flight':

                        $datetimepickerBlock = $('#edit-flight-at-datetimepicker');
                        if ($datetimepickerBlock.data('DateTimePicker')) {
                            $datetimepickerBlock.data('DateTimePicker').destroy();
                        }

                        ts = $activeItem.data('json')['flight_at'];
                        $datetimepickerBlock.datetimepicker({
                            defaultDate: new Date(ts * 1000),
                            locale: 'ru',
                            format: 'LT'
                        });

                        break;

                    case ['route', 'excursion', 'hotel_service'].indexOf($modalClone.data('entity')) != -1:

                        $datetimepickerBlock = $('#edit-starting-time-datetimepicker');
                        if ($datetimepickerBlock.data('DateTimePicker')) {
                            $datetimepickerBlock.data('DateTimePicker').destroy();
                        }

                        startingTime = $activeItem.data('json')['starting_time'];
                        if (startingTime) {
                            timeParts = $activeItem.data('json')['starting_time'].split(':');
                            $datetimepickerBlock.datetimepicker({
                                defaultDate: new Date(0, 0, 0, timeParts[0], timeParts[1], timeParts[2]),
                                locale: 'ru',
                                format: 'LT'
                            });
                        }

                        break;

                    default:
                        break;

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

                    // for suggestions
                    paramsList.push({
                        name: 'entity',
                        value: $form.data('entity')
                    });

                    // for suggestions
                    paramsList.push({
                        name: 'callout_id',
                        value: $('#suggestions-modal').data('callout-id')
                    });

                    $.ajax({
                        method: 'post',
                        url: $form.data('edit-url') + '/' + $activeItem.data('id'),
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

            // for suggestions
            paramsList.push({
                name: 'entity',
                value: $form.data('entity')
            });

            // for suggestions
            paramsList.push({
                name: 'callout_id',
                value: $('#suggestions-modal').data('callout-id')
            });

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