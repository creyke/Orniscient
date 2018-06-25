(function (orniscientutils, $, undefined) {
    orniscientutils.stringArrToSelectOptions = function(arr) {
        return arr.map(function (obj) {
            return {
                value: obj,
                label: obj
            }
        });
    }

    orniscientutils.stringArrToFilterNames = function (arr) {
        return arr.map(function (obj) {
            return {
                value: obj.fullName,
                label: obj.shortName
            }
        });
    }

    orniscientutils.methodsToSelectOptions = function(arr) {
        return arr.map(function (obj) {
            return {
                value: obj.methodId,
                label: obj.name,
                parameters: obj.parameters
            }
        });
    }

    orniscientutils.isNullOrUndefined = function(value) {
        if (value === null || value === undefined) {
            return true;
        } else {
            return false;
        }
    }

    orniscientutils.isNullOrUndefinedOrEmpty = function (value) {
        if (orniscientutils.isNullOrUndefined(value)) {
            return true;
        }
        return value.length === 0;
    }

}(window.orniscientutils = window.orniscientutils || {}, jQuery));