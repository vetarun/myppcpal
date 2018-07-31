app.directive("datepicker", function () {
    return {
        restrict: "A",
        require: "ngModel",
        link: function (scope, elem, attrs, dashboardModal) {
            var updateModel = function (dateText) {
                scope.$apply(function () {
                    dashboardModal.$setViewValue(dateText);
                });
            };
            var options = {
                dateFormat: "mm/dd/yy",
                onSelect: function (dateText) {
                    updateModel(dateText);
                }
            };
            elem.datepicker(options);
        }
    }
});

app.directive("customSort", function () {
    return {
        restrict: 'A',
        transclude: false, // transclude is true when you want to insert tempplate so that it makes insertion point in tanscluded DOM
        scope: {
            order: '=',
            sort: '='
        },
        //template:
        //  '  <span ng-transclude></span>' +
        //  '  <i ng-class="selectedCls(order)"></i>',
        link: function (scope) {
            // change sorting order
            scope.sort_by = function (newSortingOrder) {
                var sort = scope.sort;

                if (sort.sortingOrder == newSortingOrder) {
                    sort.reverse = !sort.reverse;
                }
                sort.sortingOrder = newSortingOrder;
            };

            scope.selectedCls = function (column) {
                if (column == scope.sort.sortingOrder) {
                    return (((scope.sort.reverse) ? 'header headerSortUp' : 'header headerSortDown'));
                }
                else {
                    return 'header';
                }
            };
        }// end link
    }
});



