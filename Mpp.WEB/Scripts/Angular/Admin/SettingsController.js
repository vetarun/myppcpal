app.controller("amazonCodeCtrl", ['$scope', '$filter', '$window', 'settingService', function ($scope, $filter, $window, settingService) {

    /* Scope Properties */
    $scope.AmazonCodeModal = [];
    Get2FAcodes();

    /* Get Sellers*/
    function Get2FAcodes() {
        $scope.adminspinner = true;
        var LoginCodeData = settingService.get2FACodes();
        LoginCodeData.then(function (res) {
            if (res.data.length > 0) {
                $scope.AmazonCodeModal = res.data;
            }
            else {
                $scope.AmazonCodeModal = {};
            }
            $scope.adminspinner = false;
        }, function (xhr) {
            $scope.adminspinner = false;
            console.log(xhr.error);
        });
    }

    $scope.save2FAcode = function (id) {
        var code = $('#txtAuthCode').val();
        if (code == null || code == '') {
            displayMsg("Please enter the code");
            $('#txtAuthCode').focus();
        } else {
            var CodeData = settingService.update2FACode(id, code);
            CodeData.then(function (res) {
                var msg;
                if (res.data == "") {
                    msg = "Saved successfully";
                    //window.location.href = "../Settings/AmazonCode";
                }
                else {
                    msg = res.data;
                }
                displayMsg(msg);
            }, function () {
                msg = "Error!";
                displayMsg(msg);
            });
        }
    }

    //Loader
    function unloadLoader() {
        $("#divProgress").removeClass("showdivmsg");
    }

    function displayMsg(msg) {
        $('.msg-box').css("display", "block");
        document.getElementById('divresponse').innerHTML = msg;
        setTimeout(function () { $('.msg-box').fadeOut(); }, 2000);
    }
}])