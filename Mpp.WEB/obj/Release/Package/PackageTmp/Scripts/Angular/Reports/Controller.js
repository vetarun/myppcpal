repapp.controller("orgReportsController", ['$scope', '$window', '$filter', 'ReportsService', function ($scope, $window, $filter, ReportsService) {
    //Pagination
    $scope.gap = 5;
    $scope.KeyWordOptimizeDates = [];
    $scope.SearchTermOptimizeDates = [];
    $scope.KeyWordBulkDates = [];
    $scope.SearchTermBulkDates = [];
    $scope.currentPage = 0;
    $scope.pagedItems1 = {
        records: [],
        currentPage: 0,
        recordsTotal: 0,
        lastpage: 0
    };

    $scope.Options = {
        start: 0,
        length: 10
    };

    $window.onload = function () {
        $scope.GetReportsIN();
    }

    $scope.GetReportsIN = function () {
        $scope.reportspinner = true;
        var repData = ReportsService.getorgReports($scope.Options);
        repData.then(function (res) {
            var data = res.data.data;
            if (data.length > 0) {
                $scope.pagedItems1.records = data;
                $scope.pagedItems1.recordsTotal = res.data.recordsTotal;
                $scope.pagedItems1.lastpage = Math.ceil(res.data.recordsTotal / $scope.Options.length);
                $scope.showmsg = false;
            }
            else {
                $scope.pagedItems1.records = null;
                $scope.showmsg = true;
            }
            $scope.reportspinner = false;
        }, function (xhr) {
            console.log(xhr.error);
            $scope.reportspinner = false;
        });
    }

    $scope.DownLoadReport = function (date, type) {
     
        $window.location.href = "../DownloadIn.ashx?date=" + date + "&type=" + type;

        
    }

    $scope.GetDataBySize = function () {
        var size = $scope.Options.length;
        $scope.Options = {
            start: 0,
            length: size
        };
        $scope.pagedItems1.currentPage = 0;
        $scope.GetReportsIN();
    }

    $scope.pagerange = function (size, start, end) {
        var ret = [];
        if (size < end) {
            end = size;
            if (size < $scope.gap) {
                start = 0;
            } else {
                start = size - $scope.gap;
            }
        }
        for (var i = start; i < end; i++) {
            ret.push(i);
        }
        return ret;
    };

    $scope.prevPage = function () {
        if ($scope.pagedItems1.currentPage > 0) {
            var size = parseInt($scope.Options.length);
            $scope.pagedItems1.currentPage--;
            $scope.Options.start = parseInt($scope.Options.start) - size;
            $scope.GetReportsIN();
        }
    };

    $scope.nextPage = function () {
        if ($scope.pagedItems1.currentPage < $scope.pagedItems1.lastpage-1) {
            $scope.pagedItems1.currentPage++;
            $scope.Options.start = parseInt($scope.Options.start) + parseInt($scope.Options.length);
            $scope.GetReportsIN();
        }
    };

    $scope.setPage = function () {
        var pageno = this.n;
        if ($scope.pagedItems1.currentPage != pageno) {
            $scope.pagedItems1.currentPage = pageno;
            $scope.Options.start = pageno * $scope.Options.length;
            $scope.GetReportsIN();
        }
    };

    //Loader
    $scope.unloadLoader = function () {
        $("#divProgress").removeClass("showdivmsg");
    }

    $scope.GetReportDates = function () {
        $scope.reportspinner = true;
        var repData = ReportsService.GetReportDates();
        repData.then(function (res) {
            var data = res.data;
   
            $scope.KeyWordBulkDates = data[0]['Dates'] != '' ? data[0]['Dates'] : null;
            $scope.SearchTermBulkDates = data[1]['Dates'] != '' ? data[1]['Dates'] : null;
            $scope.reportspinner = false;
        }, function (xhr) {
            console.log(xhr.error);
            $scope.reportspinner = false;
        });
    }
    $scope.GetReportDates();


  
}]);

repapp.controller("uploadReportsController", ['$scope', '$window', '$filter', 'ReportsService', function ($scope, $window, $filter, ReportsService) {
    //Pagination
    $scope.gap = 5;
    $scope.currentPage = 0;

    $scope.pagedItems1 = {
        records: [],
        currentPage: 0,
        recordsTotal: 0,
        lastpage: 0
    };

    $scope.Options = {
        start: 0,
        length: 10
    };

    $window.onload = function () {
        $scope.GetReportsOUT();
    }

    $scope.GetReportsOUT = function () {
        $scope.reportspinner = true;
        var repData = ReportsService.getuploadReports($scope.Options);
        repData.then(function (res) {
            var data = res.data.data;
            if (data.length > 0) {
                $scope.pagedItems1.records = data;
                $scope.pagedItems1.recordsTotal = res.data.recordsTotal;
                $scope.pagedItems1.lastpage = Math.ceil(res.data.recordsTotal / $scope.Options.length);
                $scope.showmsg = false;
            }
            else {
                $scope.pagedItems1.records = null;
                $scope.showmsg = true;
            }
            $scope.reportspinner = false;
        }, function (xhr) {
            console.log(xhr.error);
        });
    }

    $scope.downloadupldFile = function (startdate, enddate) {
        startdate = $filter('date')(startdate);
        enddate = $filter('date')(enddate);
        $window.location.href = "../DownloadOut.ashx?startDate=" + startdate + "&endDate=" + enddate;
        //var repData = ReportsService.downloadUploadFile(startdate, enddate);

        //repData.then(function (res) {
        //    if (res.data.length > 0) {
        //        var hiddenElement = document.createElement('a');
        //        hiddenElement.href = 'data:attachment/csv,' + encodeURI(res.data);
        //        hiddenElement.target = '_blank';
        //        hiddenElement.download = 'Sponsered Products Bulk File' + startdate + "-" + enddate + ".csv";
        //        hiddenElement.click();
        //    } else {
        //        $scope.divresponse = res.data;
        //    }
        //}, function () {
        //    msg = "Error occured!";
        //});
    }

    $scope.GetDataBySize = function () {
        var size = $scope.Options.length;
        $scope.Options = {
            start: 0,
            length: size
        };
        $scope.pagedItems1.currentPage = 0;
        $scope.GetReportsOUT();
    }

    $scope.pagerange = function (size, start, end) {
        var ret = [];
        if (size < end) {
            end = size;
            if (size < $scope.gap) {
                start = 0;
            } else {
                start = size - $scope.gap;
            }
        }
        for (var i = start; i < end; i++) {
            ret.push(i);
        }
        return ret;
    };

    $scope.prevPage = function () {
        if ($scope.pagedItems1.currentPage > 0) {
            var size = parseInt($scope.Options.length);
            $scope.pagedItems1.currentPage--;
            $scope.Options.start = parseInt($scope.Options.start) - size;
            $scope.GetReportsIN();
        }
    };

    $scope.nextPage = function () {
        if ($scope.pagedItems1.currentPage < $scope.pagedItems1.lastpage - 1) {
            $scope.pagedItems1.currentPage++;
            $scope.Options.start = parseInt($scope.Options.start) + parseInt($scope.Options.length);
            $scope.GetReportsIN();
        }
    };

    $scope.setPage = function () {
        var pageno = this.n;
        if ($scope.pagedItems1.currentPage != pageno) {
            $scope.pagedItems1.currentPage = pageno;
            $scope.Options.start = pageno * $scope.Options.length;
            $scope.GetReportsIN();
        }
    };

    //Loader
    $scope.unloadLoader = function () {
        $("#divProgress").removeClass("showdivmsg");
    }

    $scope.UpldKeyWordOptimizeDates =    [];
    $scope.UpldSearchTermOptimizeDates = [];

    $scope.GetReportDates = function () {
        $scope.reportspinner = true;
        var repData = ReportsService.GetReportDates();
        repData.then(function (res) {
            var data = res.data;
            $scope.UpldKeyWordOptimizeDates = data[2]['Dates'] != '' ? data[2]['Dates'] : null;
            $scope.UpldSearchTermOptimizeDates = data[3]['Dates'] != '' ? data[3]['Dates'] : null;
            $scope.reportspinner = false;
        }, function (xhr) {
            console.log(xhr.error);
            $scope.reportspinner = false;
        });
    }

    $scope.GetReportDates();

    $scope.DownLoadReport = function (date, type) {

        $window.location.href = "../DownloadIn.ashx?date=" + date + "&type=" + type;


    }
}]);