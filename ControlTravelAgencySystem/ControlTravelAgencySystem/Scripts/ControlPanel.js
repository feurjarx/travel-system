$(function () {

    var template = $('#suggestions-box').html();
    var render;
    try {
        render = Handlebars.compile(template);
    } catch (err) {}

    var cache = {
        data: {},
        get: function (key) {
            return this.data[key];
        },
        set: function (key, value) {
            this.data[key] = value;
        }
    };

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
                .html(render(params))
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

    $('#employee-add-modal').submit(function (event) {
        event.preventDefault();

        var $form = $(this);

        // start validation
        $form.find('.has-error').removeClass('has-error');
        $form.find('.has-feedback').removeClass('has-feedback');

        var paramsList = $form.serializeArray();

        var errors = [];
        paramsList.forEach(function (param) {

            var match;

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
        });
        // end validation

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
                url: '/employee/create',
                data: paramsList,
                success: function (response) {

                    if (response.error) {

                        console.error(response.error);

                        notification({
                            text: 'Ошибка! <br> Вероятно, вы попытались создать учетную запись с уже существующим email',
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
                        text: 'Ошибка! <br> отказ на сервере',
                        type: 'error'
                    });
                },
                complete: function () {
                    $form.find('.modal-title').unmask();
                }
            });
        }
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
});