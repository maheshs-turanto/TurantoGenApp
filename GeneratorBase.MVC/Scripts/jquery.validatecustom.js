/// <reference path="jquery.validate.js" />
/// <reference path="jquery.validate.unobtrusive.js" />

$.validator.unobtrusive.adapters.addSingleVal("unique", "unique");

$.validator.addMethod("unique", function (value, element, exclude) {
    if (value != exclude) {
        return true;
    }
    return false;
});