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
});