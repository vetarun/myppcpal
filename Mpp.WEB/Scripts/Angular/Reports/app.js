var repapp = angular.module("Reports", ['angular-loading-bar']);
repapp.config(['cfpLoadingBarProvider', function (cfpLoadingBarProvider) {
    cfpLoadingBarProvider.includeSpinner = false;
    cfpLoadingBarProvider.latencyThreshold = 20;
}]);