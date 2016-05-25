angular.module('gettext').run(['gettextCatalog', function (gettextCatalog) {
/* jshint -W100 */
    gettextCatalog.setStrings('de', {});
    gettextCatalog.setStrings('nl', {"Account Type":"Gebruikersaccount","Enter at least 3 characters or more":"Voer ten minste 3 tekens of meer"});
/* jshint +W100 */
}]);