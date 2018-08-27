
app.controller("ClientReportscontroller", ['$scope', '$window', '$filter', 'settingService', function ($scope, $window, $filter, settingService) {

    $scope.KeyWordOptimizeDates = [];
    $scope.SearchTermOptimizeDates = [];
    $scope.KeyWordBulkDates = [];
    $scope.SearchTermBulkDates = [];

    $window.onload = function () {
        $scope.GetReportDates();
    }
    //get clients 

    $scope
    $scope.logTableData = null;
    $scope.NoOfPages = [];
    $scope.selectedClient = 0;
    $scope.validDate = true;

    $("#srchClient").autocomplete({        
        minLength: 0,
        select: function (event, ui) {
            event.preventDefault();
          
            $("#srchClient").val(ui.item.label);
            $scope.selectedClient = ui.item.value;
            $scope.GetReportDates($scope.selectedClient);
        },
        source: function (req, respo) {
            var arr = [];
            settingService.getClients(req.term).then(function (res) {
                if (res.data != null && res.data.length > 0)
                    respo($.map(res.data, function (item) {
                        return {
                            label: item.Name,
                            value: item.Id
                        };

                    }))
            })
        },

    }).bind('focus', function () {
        $(this).autocomplete("search");
    });

    $scope.DownLoadReport = function (date, type) {

        $window.location.href = "/DownloadIn.ashx?date=" + date + "&type=" + type + "&userId=" + $scope.selectedClient;


    }

    //Loader
    $scope.unloadLoader = function () {
        $("#divProgress").removeClass("showdivmsg");
    }

    $scope.GetReportDates = function (userId) {
        debugger;
        //userId = 40;
        $scope.reportspinner = true;
        var repData = settingService.GetReportDates(userId);
        repData.then(function (res) {
            var data = res.data;

            $scope.KeyWordBulkDates = data[0]['Dates'] != '' ? data[0]['Dates'] : null;
            $scope.SearchTermBulkDates = data[1]['Dates'] != '' ? data[1]['Dates'] : null;
            $scope.UpldKeyWordOptimizeDates = data[2]['Dates'] != '' ? data[2]['Dates'] : null;
            $scope.UpldSearchTermOptimizeDates = data[3]['Dates'] != '' ? data[3]['Dates'] : null;
            $scope.reportspinner = false;
        }, function (xhr) {
            console.log(xhr.error);
            $scope.reportspinner = false;
        });
    }
    $scope.GetReportDates();


}]);


