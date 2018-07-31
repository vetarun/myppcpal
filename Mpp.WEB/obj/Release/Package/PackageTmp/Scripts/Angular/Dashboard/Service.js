app.service("dashboardService",["$http", function ($http) {
    this.getCampaignsTest = function (options, range) {
        var response = $http({
            method: "POST",
            data: JSON.stringify(options),
            url: "../Main/GetCampData?range=" + range,
            dataType: "json"
        })
        return response;
    }

    this.getKeywordsTest = function (options, range) {
        var response = $http({
            method: "POST",
            data: JSON.stringify(options),
            url: "../Main/GetKeyData?range=" + range,
            dataType: "json"
        })
        return response;
    }
	this.getOptimization = function (options, range, type, id, KeyName) {
		
		var response = $http({
			method: "POST",
			data: JSON.stringify(options),
			url: "../Main/GetOptimization?range=" + range + "&type=" +type + "&id=" +id + "&KeyName=" +KeyName,
			dataType: "json"
		})
		return response;
	}
	this.GetKeyLog = function (options, cid, FilterArg) {
		var response = $http({
			method: "POST",
			data: JSON.stringify(options),
			url: "../Main/GetKeywordLog?CampID=" + cid + "&FilterArg=" + FilterArg,
			dataType: "json"
		})
		return response;
	}
    this.getAdGroupsTest = function (options, range) {
        var response = $http({
            method: "POST",
            data: JSON.stringify(options),
            url: "../Main/GetAdGroupData?range=" + range,
            dataType: "json"
        })
        return response;
    }

    this.getCustomCampTest = function (options, sdate, edate) {
        var response = $http({
            method: "POST",
            data: JSON.stringify(options),
            url: "../Main/GetCustomCampData?startDate=" + sdate + "&endDate=" + edate,
            dataType: "json",
            async: true
        })
        return response;
    }

    this.getCustomKeyTest = function (options, sdate, edate) {
        var response = $http({
            method: "POST",
            data: JSON.stringify(options),
            url: "../Main/GetCustomKeyData?startDate=" + sdate + "&endDate=" + edate,
            dataType: "json",
            async: true
        })
        return response;
    }

    this.getCustomAdTest = function (options, sdate, edate) {
        var response = $http({
            method: "POST",
            data: JSON.stringify(options),
            url: "../Main/GetCustomAdGroupData?startDate=" + sdate + "&endDate=" + edate,
            dataType: "json",
            async: true
        })
        return response;
    }
 
}]);