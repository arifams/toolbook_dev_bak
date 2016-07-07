angular.module('newApp').factory('modalService', function () {
    /*
	 *  This service Controls all the modal activity
	 */
    var ModalCtrl = {};
    ModalCtrl.load = function (id) {
        $('#' + id).modal('show'); 
    }
    ModalCtrl.close = function (id) {
        $('#' + id).modal('hide');
    }

    return ModalCtrl;
});