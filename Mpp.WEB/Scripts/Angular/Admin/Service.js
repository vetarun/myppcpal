app.service("sellerService", ["$http", function ($http) {

    //Update Seller
    this.SendEmail = function (email, code) {
        var response = $http({
            data: JSON.stringify({ Email: email, AccessCode: code }),
            method: "POST",
            headers: { 'content-type': 'application/json' },
            url: "../Seller/SendAccessCode",
            dataType: "json"
        })
        return response;
    }

    //Get Users
    this.getSellerDashboard = function (type) {
        var response = $http({
            method: "GET",
            url: "../Seller/GetSellerDashboard?type=" + parseInt(type),
            dataType: "json"
        })
        return response;
    }

    //Get Users
    this.getSellers = function (type) {
        var response = $http({
            method: "GET",
            url: "../Seller/GetSellers?type=" + parseInt(type),
            dataType: "json"
        })
        return response;
    }
    //get revenue
    this.GetRevenue = function (type) {
        var response = $http({
            method: "GET",
            url: "../Seller/GetRevenue?type=" + parseInt(type),
            dataType: "json"
        })
        return response;
    }
    //get yearlyRevenue
    this.GetYearlyRevenue = function (type) {
     
        var response = $http({
            method: "GET",
            url: "../Seller/GetYearlyRevenue?type=" + parseInt(type),
            dataType: "json"
        })
        return response;
    }
    //Update Seller
    this.updateSeller = function (Sellerdata) {
        var response = $http({
            data: JSON.stringify(Sellerdata),
            method: "POST",
            headers: { 'content-type': 'application/json' },
            url: "../Seller/UpdateSeller",
            dataType: "json"
        })
        return response;
    }

    //Activation Email
    this.activationEmail = function (Sellerdata) {
        var response = $http({
            data: JSON.stringify(Sellerdata),
            method: "POST",
            headers: { 'content-type': 'application/json' },
            url: "../Seller/SendActivationEmail",
            dataType: "json"
        })
        return response;
    }

    this.srchCampaignsByUser = function (term, userId) {
        var response = $http({
            method: "GET",
            url: "../Seller/SearchCampaigns?term=" + term + "&userId=" + userId,
            dataType: "json",
        })
        return response;
    };
    //Get Campaigns Log
    this.GetCampLog = function (options, cid, userId) {
        var response = $http({
            method: "POST",
            data: JSON.stringify(options),
            url: "../Seller/GetCampaignLog?CampID=" + cid + "&userId=" + userId,
            dataType: "json"
        })
        return response;
    };

    //Get user access Log
    this.getUserAccessLog = function (options, userId) {
        var response = $http({
            method: "POST",
            data: JSON.stringify(options),
            url: "../Seller/GetUserAccessLog?userId=" + userId,
            dataType: "json"
        })
        return response;
    };
}]);

app.service("adminuserService", ["$http", function ($http) {

    //Get Users
    this.getUsers = function () {
        var response = $http({
            method: "GET",
            url: "../Account/GetUsers",
            dataType: "json"
        })
        return response;
    }

    //Save Users
    this.saveUser = function (user) {
        var response = $http({
            data: JSON.stringify(user),
            method: "POST",
            headers: { 'content-type': 'application/json' },
            url: "../Account/AddUser",
            dataType: "json"
        })
        return response;
    }

    //Delete User
    this.removeUser = function (userID) {
        var response = $http({
            data: JSON.stringify({ UserID: userID }),
            method: "POST",
            headers: { 'content-type': 'application/json' },
            url: "../Account/DeleteUser",
            dataType: "json"
        })
        return response;
    }

    //Delete User
    this.updateStatus = function (userID, status) {
        var response = $http({
            data: JSON.stringify({ UserID: userID, Status: status }),
            method: "POST",
            headers: { 'content-type': 'application/json' },
            url: "../Account/UpdateStatus",
            dataType: "json"
        })
        return response;
    }
}]);

app.service("affiliationService",["$http", function ($http) {

    //Get Coupons
    this.getAllCodes = function (options) {
        var response = $http({
            method: "POST",
            data: JSON.stringify(options),
            url: "../Affiliation/GetAllCodes",
            dataType: "json"
        })
        return response;
    }

    //Get Sales 
    this.getSales = function (Id) {
        var response = $http({
            method: "POST",
            data: JSON.stringify({ AffiliateID: Id }),
            url: "../Affiliation/GetAffiliateDetails",
            dataType: "json"
        })
        return response;
    }

    //Save Coupon
    this.Save = function (data) {
        var response = $http({
            data: JSON.stringify(data),
            method: "POST",
            headers: { 'content-type': 'application/json' },
            url: "../Affiliation/AddUpdateCode",
            dataType: "json"
        })
        return response;
    }

    //Delete Coupon
    this.deleteCoupon = function (codeID) {
        var response = $http({
            data: JSON.stringify({ CodeID: codeID }),
            method: "POST",
            headers: { 'content-type': 'application/json' },
            url: "../Affiliation/DeleteCode",
            dataType: "json"
        })
        return response;
    }

    //Update Coupon Status
    this.updateStatus = function (codeID, status) {
        var response = $http({
            data: JSON.stringify({ CodeID: codeID, Status: status }),
            method: "POST",
            headers: { 'content-type': 'application/json' },
            url: "../Affiliation/UpdateStatus",
            dataType: "json"
        })
        return response;
    }

    //Get affiliate users by search term 
    this.getAffilateUsers = function (term,code) {
        
        var response = $http({
            method: "get",
            url: "../Affiliation/GetAffiliateUsers?serachParam=" + term+"&code="+code,
        })
        return response;
    }

    //set affiliate code and user maping
    this.assignAffilitaeCode = function (code,userId) {

        var response = $http({
            method: "get",
            url: "../Affiliation/AssignCodeToAffiliate?codeId=" + code + "&userId=" + userId,
        })
        return response;
    }

    //get affiliate code and user maping
    this.getAssignedUsers = function (code) {

        var response = $http({
            method: "get",
            url: "../Affiliation/GetAssignedAffiliates?codeId=" + code,
        })
        return response;
    }
}]);

app.service("settingService",["$http", function ($http) {

    //Get 2FACodes
    this.get2FACodes = function () {
        var response = $http({
            method: "GET",
            url: "../Preferences/GetLoginCodes",
            dataType: "json"
        })
        return response;
    }

    //Update code
    this.update2FACode = function (Id,Code) {
        var response = $http({
            method: "POST",
            data: JSON.stringify({ CodeID: Id, LoginCode: Code }),
            url: "../Preferences/UpdateLoginCode",
            dataType: "json"
        })
        return response;
    }



        //Get Download upload dates
        this.GetReportDates = function (userId) {
            var response = $http({
                method: "GET",
                url: "../Preferences/GetReportDates?userId=" +userId,
                dataType: "json"
            })
            return response;
        }

    //get clients for log listing
    this.getClients = function (term) {
        var response = $http({
            method: "GET",   
            url: "../Preferences/GetClients?srchTerm=" + term,
            dataType: "json"
        })
        return response;
    }

    //get reports log data
    this.getReportLog = function (data) {
        var response = $http({
            method: "post",
            data: JSON.stringify(data),
            url: "../Preferences/GetReportLogs",
            dataType: "json"
        })
        return response;
    }

    this.getEmailLog = function (data) {
        var response = $http({
            method: "post",
            data: JSON.stringify(data),
            url: "../Preferences/GetEmailLog",
            dataType: "json"
        })
        return response;
    }
    
}]);
