// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function validacoesEvento() {

    $.validator.methods.range = function (value, element, param) {
        var globalizedValue = value.replace(",", ".");
        return this.optional(element) || (globalizedValue >= param[0] && globalizedValue <= param[1]);
    };

    $.validator.methods.number = function (value, element) {
        return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:[\s\.,]\d{3})+)(?:[\.,]\d+)?$/.test(value);
    };

    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
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

    //$('#DataInicio').datepicker({
    //    format: "dd/mm/yyyy",
    //    startDate: "tomorrow",
    //    language: "pt-BR",
    //    orientation: "bottom right",
    //    autoclose: true
    //});

    //$('#DataFim').datepicker({
    //    format: "dd/mm/yyyy",
    //    startDate: "tomorrow",
    //    language: "pt-BR",
    //    orientation: "bottom right",
    //    autoclose: true
    //});
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

