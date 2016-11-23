$(function () {

    $.get('/callout/predefinedcallouts?need=total', function (response) {

        var render = Handlebars.compile($('#predefined-callouts-pagination-hbs').html());

        var $container = $('#predefined-callouts-container');
        $('#pagination-container').pagination({
            dataSource: '/callout/predefinedcallouts',
            locator: 'callouts',
            totalNumber: response.total,
            pageSize: 10,
            ajax: {
                beforeSend: function() {
                    $container.html('Loading data ...');
                }
            },
            callback: function(callouts, pagination) {

                $container.html(render({
                    callouts: callouts.map(function (c) {
                        return JSON.parse(c);
                    })
                }));
            }
        });
    });
});