$(function () {

    $('#callout-modal').submit(function (event) {

        var $form = $(this);
        $form.find('.has-error').removeClass('has-error');
        $form.find('.has-feedback').removeClass('has-feedback');

        var errors = [],
            match;

        $form.serializeArray().forEach(function (param) {

            if (param.value) {
                switch (param.name) {
                    case 'Fullname':

                        if (param.value && param.value.trim().split(' ').length != 3) {
                            $('#fullname-input').highlightElement('error');
                            errors.push('Некорректно заполнено поле ФИО');
                        }

                        break;

                    case 'Phone':

                        match = param.value.match(/[^\d]/g);
                        if (match || param.value.length < 6) {
                            $('#phone-input').highlightElement('error');
                            errors.push('Некорректно указан номер телефона');
                        }

                        break;

                    default:
                }
            }
        });

        if (errors.length) {

            event.preventDefault(); // stop action

            var warningLines = '';
            errors.forEach(function (err, i) {
                warningLines += (i + 1) + '. ' + err + '<br>';
            });

            notification({
                text: 'Внимание! <br> ' + warningLines,
                type: 'warning'
            }, 5000);
        }
    });
});