var campapp = angular.module('myCampaigns', ['ngMessages', 'angular-loading-bar']);
campapp.config(['cfpLoadingBarProvider', function (cfpLoadingBarProvider) {
    cfpLoadingBarProvider.includeSpinner = false;
    cfpLoadingBarProvider.latencyThreshold = 50;
}]);