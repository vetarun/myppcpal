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

app.directive('numbersOnly', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl) {
            //element.bind('blur', function () {
            //   if (parseInt(element.val(), 10) < attrs.ngMin) {
            //       modelCtrl.$setValidity('belowminimum', false);
            //       scope.$apply(function () {
            //           element.val('');
            //        });
            //    }
            //});
            modelCtrl.$parsers.push(function (inputValue) {
                // this next if is necessary for when using ng-required on your input. 
                // In such cases, when a letter is typed first, this parser will be called
                // again, and the 2nd time, the value will be undefined
                if (inputValue == undefined) return ''
                var transformedInput = inputValue.replace(/[^0-9]/g, '');
                if (transformedInput != inputValue || (parseInt(transformedInput, 10) < parseInt(attrs.ngMin, 10)) || parseInt(transformedInput, 10) > parseInt(attrs.ngMax, 10)) {
                    if (transformedInput != inputValue) {
                        modelCtrl.$setValidity('nonnumeric', false);
                    } else {
                        modelCtrl.$setValidity('nonnumeric', true);
                    }
                    if (parseInt(transformedInput, 10) < parseInt(attrs.ngMin, 10)) {
                        modelCtrl.$setValidity('belowminimum', false);
                    } else {
                        modelCtrl.$setValidity('belowminimum', true);
                    }
                    if (parseInt(transformedInput, 10) > parseInt(attrs.ngMax, 10)) {
                        modelCtrl.$setValidity('abovemaximum', false);
                    } else {
                        modelCtrl.$setValidity('abovemaximum', true);
                    }
                    return inputValue;
                }
                modelCtrl.$setValidity('nonnumeric', true);
                modelCtrl.$setValidity('belowminimum', true);
                modelCtrl.$setValidity('abovemaximum', true);
                return inputValue;
            });
        }
    };
});

app.directive('allowOnlyNumbers', function () {
    return {
        restrict: 'A',
        link: function (scope, elm, attrs, ctrl) {
            elm.on('keydown', function (event) {
                
                var target = event.target || event.srcElement || event.curentTarget;
                var val = target.value!=''?parseInt(target.value):0;
                var c = String.fromCharCode(event.which);
                if (!isNaN(val))
                    val = (val ==0?parseInt(c):parseInt(val + c));
                var total = parseInt(val + c);
                if (event.which == 64 || event.which == 16) {
                    // to allow numbers  
                    return false;
                }
                else if (event.which >= 48 && event.which <= 57) {
                    // to allow numbers  
                    return true;
                } else if (event.which >= 96 && event.which <= 105) {
                    // to allow numpad number  
                    return true;
                } else if ([8, 13, 27, 37, 38, 39, 40].indexOf(event.which) > -1) {
                    // to allow backspace, enter, escape, arrows  
                    return true;
                }
                else {
                    event.preventDefault();
                    // to stop others  
                    return false;
                }
            });
        }
    }
});  