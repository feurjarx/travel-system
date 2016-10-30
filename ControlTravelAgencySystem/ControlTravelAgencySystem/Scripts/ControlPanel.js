$(function () {

    var template = $('#suggestions-box').html();
    var render = Handlebars.compile(template);

    var cache = {
        data: {},
        get: function (key) {
            return this.data[key];
        },
        set: function (key, value) {
            this.data[key] = value;
        }
    };

    $('#callouts-table').find('tbody > tr').on('click', function (event) {

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

    $('#employee-add-modal').submit(function (event) {
        event.preventDefault();

        var $form = $(this);

        $form.find('.modal-title').mask();

        $.ajax({
            method: 'post',
            url: '/employee/create',
            data: $form.serializeArray(),
            success: function (response) {

                if (response.error) {

                    console.error(response.error);

                    $form.find('.error-label').text('Ошибка на сервере');

                } else {

                    $form.modal('hide').get(0).reset();
                    location.reload();
                }
            },
            error: function (err) {
                console.error(err);
            },
            complete: function () {
                $form.find('.modal-title').unmask();
            }
        });
    });

    $('.btn-employee-remove').on('click', function () {

        var $item = $(this).closest('tr');
        var employeeId = $item.data('id');

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
            }
        });
    });
});