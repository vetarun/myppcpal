app.filter('date', ["$filter", function ($filter) {
    return function (input) {
        if (input == null)
        {
            return "-";
        } else {
            var _date = getMonthDate(new Date(parseInt(input.substr(6))));
            if (_date == null) { return ""; } else {
                return _date;
            }
        }
    };
}]);

app.filter('firstAlphabet',["$filter", function ($filter) {
    return function (input) {
        var intial = input.charAt(0);
        if (intial == null) {
            return "";
        } else {
            return intial;
        }
    };
}]);

app.filter('capitalize', function () {
    return function (input) {
        return (!!input) ? input.charAt(0).toUpperCase() + input.substr(1).toLowerCase() : '';
    }
});

app.filter('removezero', function () {
    return function (input) {
        if (input == null || input == 0) {
            return "-";
        } else {
            return input;
        }
    };
});

app.filter('PlanName', function ($filter) {
    return function (input) {
        if (input == "1") {
            return "TRIAL";
        } else if(input == "2") {
            return "SOLOPRENEUR";
        } else if (input == "3") {
            return "STARTUP";
        } else if (input == "4") {
            return "BUSINESS TIME";
        } else if (input == "5") {
            return "BIG BUSINESS";
        } else if (input == "6") {
            return "ENTERPRISE";
        } else if (input == "7") {
            return "NO PLAN";
        } else {
            return "CUSTOM";
        }
    };
});