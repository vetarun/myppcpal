var app = angular.module("adminapp", ['ngMessages', 'angular-loading-bar']);
app.config(['cfpLoadingBarProvider', function (cfpLoadingBarProvider) {
    cfpLoadingBarProvider.includeSpinner = false;
    cfpLoadingBarProvider.latencyThreshold = 10;
}]);