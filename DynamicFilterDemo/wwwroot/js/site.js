
$(document).ready(function () {
    $('.select2').select2({
        placeholder: 'Seçiniz',
        theme: "classic"
    });
});

var removeOption = {};

(function ($) {
    'use strict';

    //toastr.options = {
    //    "closeButton": true,
    //    "debug": false,
    //    "newestOnTop": true,
    //    "progressBar": false,
    //    "positionClass": "toast-top-right",
    //    "preventDuplicates": false,
    //    "onclick": null,
    //    "showDuration": "300",
    //    "hideDuration": "1000",
    //    "timeOut": "5000",
    //    "extendedTimeOut": "1000",
    //    "showEasing": "swing",
    //    "hideEasing": "linear",
    //    "showMethod": "fadeIn",
    //    "hideMethod": "fadeOut"
    //}

    //Json Serialize
    $.fn.extend({
        serializeFormJSON: function (event) {
            debugger;
            var that = this;
            var o = {};
            var items = this.serializeArray();
            $.each(items, function () {
                debugger;
                if (o[this.name]) {
                    if (!o[this.name].push) {
                        o[this.name] = [o[this.name]];
                    }
                    o[this.name].push(this.value || '');
                } else {
                    o[this.name] = this.value || '';
                }
            });
            if (event) {
                return JSON.stringify(o);
            } else {
                return o;
            }
        }
    });

})(jQuery);