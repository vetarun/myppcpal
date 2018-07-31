app.factory('deleteService',["$http", function ($http) {
    var user = function (SellerId, CustId) {
        // Angular $http() and then() both return promises themselves 
        return $http({
            data: JSON.stringify({ SellerID: SellerId, CustID: CustId }),
            method: "POST",
            headers: { 'content-type': 'application/json' },
            url: "../Seller/DeleteSeller",
            dataType: "json"
        }).then(function (result) {
            // What we return here is the data that will be accessible 
            // to us after the promise resolves
            return result.data;
        });
    }
    return {
        deleteUser: function (sellerId, custId) {
            return user(sellerId, custId)
        }
    }
}]);
