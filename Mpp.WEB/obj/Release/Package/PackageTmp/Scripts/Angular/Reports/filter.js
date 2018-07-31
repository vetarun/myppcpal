repapp.filter('date',["$filter", function ($filter) {
    return function (input) {
        var _date = getNewDate(new Date(parseInt(input.substr(6))));
        if (_date == null) { return ""; } else {
            return _date.toUpperCase();
        }
    };
}]);