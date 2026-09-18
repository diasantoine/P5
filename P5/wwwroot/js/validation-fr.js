// jQuery Validate ne connaît que la notation anglaise des nombres : « 1800.50 ».
// Le site est en culture française : le serveur affiche et attend « 1800,50 ». Sans ce fichier,
// le navigateur refuserait le prix que le serveur vient lui-même de pré-remplir dans le formulaire.
// On redéfinit donc les deux règles qui lisent un nombre, pour qu'elles acceptent la virgule.
(function ($) {
    if (!$ || !$.validator) {
        return;
    }

    // « 10 990,50 » -> 10990.5 ; tout ce qui n'est pas un nombre français donne NaN.
    function parseFrenchNumber(value) {
        var cleaned = String(value).replace(/[\s  ]/g, '');
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
