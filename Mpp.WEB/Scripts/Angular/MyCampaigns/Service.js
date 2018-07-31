campapp.service("CampService",["$http", function ($http) {

    //Get myCampaigns
    this.getmyCampaigns = function () {
        var response = $http({
            method: "GET",
            url: "../MyCampaigns/GetMyCampaigns",
            dataType: "json"
        })
        return response;
    }

    this.RevertChanges = function (keyId, cid, aid, rid, rsnid, ModifyDate,checked) {
        
        var response = $http({
            method: "GET",
            url: "../MyCampaigns/RevertChanges?KeyID=" + keyId + "&CID=" + cid + "&AID=" + aid + "&RID=" + rid + "&RSNID=" + rsnid + "&ModifyDate=" + ModifyDate + "&Checked=" + checked,
            dataType: "json"
        })
        return response;
    }
    //Get Formula
    this.getCampFormula = function (cid) {
        var response = $http({
            method: "GET",
            url: "../MyCampaigns/GetFormula?CampID=" + cid,
            dataType: "json"
        })
        return response;
    }

    //Save Formula
    this.saveFormula = function (cid, formula) {
        var response = $http({
            method: "POST",
            url: "../MyCampaigns/SaveFormula?CampID=" + cid,
            data: JSON.stringify(formula),
            dataType: "json"
        })
        return response;
    }

    //Copy Formula
    this.pasteFormula = function (copyid, pasteid) {
        var response = $http({
            method: "POST",
            url: "../MyCampaigns/PasteCampFormula?copyID=" + copyid + "&pasteID=" + pasteid,
            dataType: "json"
        })
        return response;
    }

    //Paste Formula to all camps
    this.PastAllCampaigns = function (copyid) {
        var response = $http({
            method: "POST",
            url: "../MyCampaigns/PasteAllCampFormula?cID=" + copyid,
            dataType: "json"
        })
        return response;
    }

    //Run Formula
    this.runCampaign = function (cid, status) {
        var response = $http({
            method: "POST",
            url: "../MyCampaigns/RunCampaign?CampID=" + cid + "&status=" + status,
            dataType: "json"
        })
        return response;
    }

    //Run Formula
    this.runCampaigns = function (clist) {
        var response = $http({
            method: "POST",
            data: JSON.stringify({ 'Camplist': clist }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            url: "../MyCampaigns/RunCampaigns"
        })
        return response;
    }

    //Get Campaigns Log
    this.GetCampLog = function (options, cid) {
        var response = $http({
            method: "POST",
            data: JSON.stringify(options),
            url: "../MyCampaigns/GetCampaignLog?CampID=" + cid,
            dataType: "json"
        })
        return response;
    }

    //Get Keywords Log
    this.GetKeyLog = function (options, cid) {
        var response = $http({
            method: "POST",
            data: JSON.stringify(options),
            url: "../MyCampaigns/GetKeywordLog?CampID=" + cid,
            dataType: "json"
        })
        return response;
    }

    //Get Keywords on demand
    this.FetchNewKeys = function () {
        var response = $http({
            method: "GET",
            url: "../MyCampaigns/FetchNewCampaigns",
            dataType: "json"
        })
        return response;
    }

}]);