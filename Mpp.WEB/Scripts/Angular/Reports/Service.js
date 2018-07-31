repapp.service("ReportsService",["$http", function ($http) {
   
    //Get orginal reports
    this.getorgReports = function (options) {
        var response = $http({
            method: "POST",
            data: JSON.stringify(options),
            url: "../Reports/GetReportsIn",
            dataType: "json"
        })
        return response;
    }

    //Get upload reports
    this.getuploadReports = function (options) {
        var response = $http({
            method: "POST",
            data: JSON.stringify(options),
            url: "../Reports/GetReportsOut",
            dataType: "json"
        })
        return response;
    }

    //Get Download Original file
    this.downloadOrgFile = function (startdate, enddate) {
        var response = $http({
            method: "GET",
            url: "../Reports/DownloadFileData?startDate=" + startdate + "&endDate=" + enddate,
            dataType: "json"
        })
        return response;
    }

    //Get Download upload file
    this.downloadUploadFile = function (startdate, enddate) {
        var response = $http({
            method: "GET",
            url: "../Reports/Uploadfile?startDate=" + startdate + "&endDate=" + enddate,
            dataType: "json"
        })
        return response;
    }

    //Get Download upload dates
    this.GetReportDates = function () {
        var response = $http({
            method: "GET",
            url: "../Reports/GetReportDates",
            dataType: "json"
        })
        return response;
    }

}]);