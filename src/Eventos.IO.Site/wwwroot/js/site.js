// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function validacoesEvento() {

    $.validator.methods.range = function (value, element, param) {
        var globalizedValue = value.replace(",", ".");
        return this.optional(element) || globalizedValue >= param[0] && globalizedValue <= param[1];
    };

    $.validator.methods.number = function (value, element) {
        return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:[\s\.,]\d{3})+)(?:[\.,]\d+)?$/.test(value);
    };

    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": true,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };

    // Define a callback for when the toast is shown/hidden/clicked
    //toastr.options.onShown = function () { console.log('hello'); };
    toastr.options.onHidden = function () {
        console.log('goodbye');
        window.location = '/meus-eventos';
    };
    toastr.options.onclick = function () {
        console.log('clicked');
        window.location = '/meus-eventos';
    };
    toastr.options.onCloseClick = function () {
        console.log('close button clicked');
        window.location = '/meus-eventos';
    };

}

$(document).ready(function () {

    DateTimePicker();
    $("#txtDataInicio").change(function () {
        var date2 = $("#txtDataInicio").datepicker('getDate', '+1d');
        console.log(date2);
        date2.setDate(date2.getDate() + 1);
        $("#txtDataFim").datepicker('option', 'minDate', date2);
        $("#txtDataFim").datepicker('setDate', date2);
    });

    // Validacoes de exibicao do endereco

    var $input = $('#Online');
    var $inputGratuito = $("#Gratuito");

    MostrarEndereco();
    MostrarValor();

    $input.click(function () {
        MostrarEndereco();
    });

    $inputGratuito.click(function () {
        MostrarValor();
    });

    function MostrarEndereco() {
        if ($input.is(':checked')) {
            $('#EnderecoForm').hide();
        } else {
            $('#EnderecoForm').show();
        }
    }

    function MostrarValor() {
        if ($inputGratuito.is(':checked')) {
            // $('#Valor').val(0);
            $("#Valor").prop("disabled", true);
        } else {
            // $('#Valor').val('');
            $('#Valor').prop('disabled', false);
        }
    }


});

function DateTimePicker() {
    $(".datepicker").datepicker({

        closeText: 'Fechar',
        prevText: '<Anterior',
        nextText: 'Próximo>',
        currentText: 'Hoje',
        monthNames: ['Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho',
            'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'],
        monthNamesShort: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun',
            'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'],
        dayNames: ['Domingo', 'Segunda-feira', 'Terça-feira', 'Quarta-feira', 'Quinta-feira', 'Sexta-feira', 'Sabado'],
        dayNamesShort: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sab'],
        dayNamesMin: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sab'],

        gotoCurrent: true,
        dateFormat: 'dd/mm/yy',
        changeMonth: true,
        changeYear: true,
        showOn: 'button',
        sideBySide: true,
        controlType: 'select',
        buttonText: '<i class="fa fa-calendar-times-o" aria-hidden="true"></i>',
        minDate: new Date(),
        locale: 'pt-br'

    });
}

function AjaxModal() {

    $(document).ready(function () {

        $(function () {

            $.ajaxSetup({ cache: false });

            $("a[data-modal]").on("click",
                function (e) {

                    console.log('Url da Controller ', this.href);

                    $('#myModalContent').load(this.href,
                        function () {

                            $('#myModal').modal({
                                keyboard: true
                            }, 'show');

                            bindForm(this);
                        });
                    return false;
                });
        });

        function bindForm(dialog) {

            $('form', dialog).submit(function () {
                $.ajax({
                    url: this.action,
                    type: this.method,
                    data: $(this).serialize(),
                    success: function (result) {
                        if (result.success) {

                            console.log('Result Url: ', result.url);

                            $('#myModal').modal('hide');
                            $('#EnderecoTarget').load(result.url); // Carrega o resultado HTML para a div demarcada
                        } else {
                            $('#myModalContent').html(result);
                            bindForm(dialog);
                        }
                    }
                });
                return false;
            });
        }


    });
}
