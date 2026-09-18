// Remplace les règles "number" et "range" de jQuery Validate par des versions
// qui lisent la virgule décimale française.
(function ($) {
    if (!$ || !$.validator) {
        return;
    }

    // « 10 990,50 » -> 10990.5 ; tout ce qui n'est pas un nombre français donne NaN.
    function parseFrenchNumber(value) {
        var cleaned = String(value).replace(/[\s\u00a0\u202f]/g, '');
        return /^-?\d+(,\d+)?$/.test(cleaned) ? Number(cleaned.replace(',', '.')) : NaN;
    }

    $.validator.methods.number = function (value, element) {
        return this.optional(element) || !isNaN(parseFrenchNumber(value));
    };

    $.validator.methods.range = function (value, element, param) {
        var number = parseFrenchNumber(value);
        return this.optional(element) || (number >= param[0] && number <= param[1]);
    };
})(window.jQuery);
