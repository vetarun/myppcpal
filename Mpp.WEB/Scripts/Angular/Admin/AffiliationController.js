app.controller("affiliationCtrl", ['$scope', '$filter', '$window', 'affiliationService', function ($scope, $filter, $window, affiliationService) {

    //var _adminuser = this; // Replace scope with this one later
    /* Scope Properties */
    $scope.submitted = false;
    $scope.message = '';
    $scope.isFormValid = false;
    $scope.modalmsg = true;
    $scope.Affiliation = {
        Code: '',
        Percent: '',
        Duration: '0',
        Amount: '',
        Maxredeem: '',
        Redeemby: ''
    };
    $scope.min = 1;
    $scope.max = 100;
    $scope.pagedItems1 = {
        records: [],
        currentPage: 0,
        recordsTotal: 0,
        lastpage: 0
    };
    $scope.filterParams = {
        nameSearch: ''
    };

    $scope.sort = {
        sortingOrder: 'Code',
        reverse: false
    };

    $scope.Options = {
        start: 0,
        length: 10,
        columnname: $scope.sort.sortingOrder,
        direction: $scope.sort.reverse,
        searchname: ''
    };
    $scope.PayoffType = 0;

    //Pagination
    $scope.gap = 5;
    $window.onload = function () {
        $scope.GetAllCoupons();
    }

    /* Get All Codes */
    $scope.GetAllCoupons = function () {
        $scope.adminspinner = true;
        var couponData = affiliationService.getAllCodes($scope.Options);
        couponData.then(function (res) {
            var data = res.data.data;
            if (data.length > 0) {
                $scope.pagedItems1.records = res.data.data;
                $scope.pagedItems1.recordsTotal = res.data.recordsTotal;
                $scope.pagedItems1.lastpage = Math.floor(res.data.recordsTotal / $scope.Options.length) - 1;
                $scope.showmsg = false;
                $scope.adminspinner = false;
            }
            else {
				$scope.pagedItems1.records = null;
				$scope.pagedItems1.recordsTotal = null;
                $scope.showmsg = true;
                $scope.adminspinner = false;
            }
        }, function (xhr) {
            console.log(xhr.error);
            $scope.adminspinner = false;
        });
    }

    //Loader
    function unloadLoader() {
        $scope.campspinner = false;
    }

    //Check form Validation 
    $scope.$watch('fmcode.$valid', function (newValue) {
        $scope.isFormValid = newValue;
        if ($scope.isFormValid) {
            $scope.message = '';
        }
    });

    /* Get All Codes */
    $scope.GetSalesModel = function (id, code) {
        $scope.adminspinner = true;
        document.getElementById('hdnCodeId').value = id;
        $scope.Act_CouponCode = code;
        var couponData = affiliationService.getSales(id);
        couponData.then(function (res) {
            var data = res.data.data;
            if (data.length > 0) {
                $scope.pagedItems2 = data;
                $scope.modalmsg = false;
                $scope.adminspinner = false;
            }
            else {
                $scope.pagedItems2 = null;
                $scope.modalmsg = true;
                $scope.adminspinner = false;
            }
        }, function () {
            msg = "Error!";
            $scope.adminspinner = false;
        });
    }

    //Save Coupon Data
    $scope.AddUpdateCode = function (data) {
        var msg;
        $scope.submitted = true;
        $scope.message = '';
        if ($scope.isFormValid) {
            $scope.adminspinner = true;
            $scope.Affiliation = data;
            affiliationService.Save($scope.Affiliation).then(function (d) {
                if (d.data == '') {
                    msg = "Created Successfully!";
                    displayMsg(msg, 1);
                    $scope.Close();
                    $scope.adminspinner = false;
                    $scope.GetAllCoupons();
                } else {
                    $scope.adminspinner = false;
                    displayMsg(d.data, 2);
                }
            }, function (xhr) {
                console.log(xhr.error);
                $scope.adminspinner = false;
                displayMsg(msg, 2);
            });
        }
    }

    //Remove Coupon
    $scope.DeleteCode = function (id) {
        var confrm = false;
        confrm = $window.confirm("Deleting this coupon will not affect discounts for customers who have already redeemed the coupon, but new redemptions of the coupon won't be allowed.");
        if (confrm) {
            $scope.adminspinner = true;
            affiliationService.deleteCoupon(id).then(function (d) {
                if (d.data == '') {
                    $scope.SetDefault('Code', false, 10, '');
                    $scope.GetAllCoupons();
                    displayMsg("Deleted Successfully!", 1);
                    $scope.adminspinner = false;
                } else {
                    displayMsg(d.data, 2);
                    $scope.adminspinner = false;
                }
            }, function () {
                displayMsg("Error!", 2);
                $scope.adminspinner = false;
            });
        }
    }

    $scope.Close = function CloseModal() {
        $('.modal').modal('hide');
        $scope.ResetForm();
    }


    //Clear Form (reset)
    $scope.ResetForm = function ClearForm() {
        $scope.Affiliation = {
            Code: '',
            Percent: '',
            Duration: '0'
        };
        $scope.fmcode.$setPristine(true); //here fmuser our form name
        $scope.submitted = false;
        $scope.message = '';
    }

    function displayMsg(msg, type) {
        if (type == 1) {
            $('.msg-box').css("display", "block");
            document.getElementById('divresponse').innerHTML = msg;
            setTimeout(function () { $('.msg-box').fadeOut(); }, 2000);
        } else {
            $('.errormsg').css("display", "block");
            $scope.message = msg;
            //setTimeout(function () { $('.errormsg').fadeOut(); }, 2000);
        }
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
        for (var i = start; i <= end; i++) {
            ret.push(i);
        }
        (ret);
        return ret;
    };

    $scope.prevPage = function () {
        if ($scope.pagedItems1.currentPage > 0) {
            var size = parseInt($scope.Options.length);
            $scope.pagedItems1.currentPage--;
            $scope.Options.start = parseInt($scope.Options.start) - size;
            $scope.GetAllCoupons();
        }
    };

    $scope.nextPage = function () {
        if ($scope.pagedItems1.currentPage < $scope.pagedItems1.lastpage) {
            $scope.pagedItems1.currentPage++;
            $scope.Options.start = parseInt($scope.Options.start) + parseInt($scope.Options.length);
            $scope.GetAllCoupons();
        }
    };

    $scope.setPage = function () {
        var pageno = this.n;
        if ($scope.pagedItems1.currentPage != pageno) {
            $scope.pagedItems1.currentPage = pageno;
            $scope.Options.start = pageno * $scope.Options.length;
            $scope.GetAllCoupons();
        }
    };


    $scope.sort_by = function (newSortingOrder) {
        if ($scope.sort.sortingOrder == newSortingOrder) {
            $scope.sort.reverse = !$scope.sort.reverse;
        }
        $scope.sort.sortingOrder = newSortingOrder;
        $scope.pagedItems1.currentPage = 0;
        var len = $scope.Options.length;
        $scope.Options = {
            start: 0,
            length: len,
            columnname: newSortingOrder,
            direction: $scope.sort.reverse,
            searchname: $scope.filterParams.nameSearch
        };
        if ($scope.pagedItems1.records.length > 0) {
            $scope.GetAllCoupons();
        }
    };

    $scope.GetDataBySize = function () {
        var size = $scope.Options.length;
        var range = $scope.Range;
        var order = $scope.sort.sortingOrder;
        var dir = $scope.sort.direction;
        var search = $scope.filterParams.nameSearch;
        $scope.SetDefault(order, dir, size, search);
        $scope.GetAllCoupons();
    }

    $scope.filter_by = function () {
        $scope.pagedItems1.currentPage = 0;
        $scope.Options.searchname = $scope.filterParams.nameSearch;
        $scope.Options.start = 0;
        $scope.GetAllCoupons();
            
    };

    $scope.selectedCls = function (column) {
        if (column == $scope.sort.sortingOrder) {
            return ((($scope.sort.reverse) ? 'headerSort headerSortUp' : 'headerSort headerSortDown'));
        }
        else {
            return 'headerSort';
        }
    };

    //Set Modal sorting default values
    $scope.SetDefault = function (name, rev, len, search) {
        $scope.sort.sortingOrder = name;
        $scope.sort.reverse = rev;
        $scope.filterParams.nameSearch = search;
        $scope.Options = {
            start: 0,
            length: len,
            columnname: $scope.sort.sortingOrder,
            direction: $scope.sort.reverse,
            searchname: $scope.filterParams.nameSearch
        };
        $scope.pagedItems1.currentPage = 0;
    }

    $scope.SelectedCode = "N/A";
    $scope.SelectedCodeID = 0;
    $scope.SelectedUserID = 0;
    $scope.AssignedUsers = [];
    $scope.ShowSpiner = false;
    $scope.IsUserAssigned = false;
    //assign affiliate code modal
    $scope.AffiliateListModel = function (id, code) {
        $scope.GetAssignedUsers(id);
        $scope.SelectedCode = code;
        $scope.SelectedCodeID = id;

       
            $("#srchAffiliate").autocomplete({
                minLength: 0,
                appendTo: ".filters",
                select: function (event, ui) {
                    $scope.Valid = false;
                    event.preventDefault();
                    $("#srchAffiliate").val(ui.item.label);
                    $scope.SelectedUserID = ui.item.value;
                    $('#btnAssignCode').removeAttr("disabled");

                },
                source: function (req, respo) {
                    
                    $('#srchSpinner').css('display', 'block');
                    var arr = [];
                    affiliationService.getAffilateUsers(req.term, id).then(function (res) {
                        if (res.data != null && res.data.length > 0)
                            respo($.map(res.data, function (item) {
                                return {
                                    label: item.Name,
                                    value: item.Id
                                };

                            }))
                        $('#srchSpinner').css('display', 'none');
                    })  
                  
                },

            }).bind('focus', function () {
                $(this).autocomplete("search");
            });
            $scope.ResetModal();
  
     
        $('#AffiliateListModel').modal('show');

    }
    $('#srchAffiliate').blur(function () {
        if ($(this).val()!='')
        $scope.Valid = false;
    });
    $scope.ResetModal = function () {
        $('#srchAffiliate').val('');
        $('#AffiliateListModel').modal('hide');
        $scope.divShow = false;
        $scope.Valid = false;
        $scope.IsAssigned = false;
        $('#btnAssignCode').attr("disabled", "disabled");
        $scope.IsUserAssigned = false;
        $('#srchSpinner').css('display', 'none');

    }
    $scope.ResetModalShow = function () {
        $('#srchAffiliate').val('');
       // $('#AffiliateListModel').modal('hide');
        $scope.divShow = false;
        $scope.Valid = false;
        $('#btnAssignCode').attr("disabled", "disabled");
        $scope.IsUserAssigned = false;
        $('#srchSpinner').css('display', 'none');

    }
    $scope.divShow = false;
    $scope.Valid = false;
   
    $scope.ResultMsg = "";
    $scope.SubmitAssignCode = function () {
        if ($('#srchAffiliate').val()!='') {
            $scope.adminspinner = true;
            var result = affiliationService.assignAffilitaeCode($scope.SelectedCodeID, $scope.SelectedUserID);
            result.then(function (res) {
                $scope.divShow = true;
                $scope.errorShow = res.data;
                if (res.data) {
                    $scope.ResetModalShow();
                    $scope.GetAssignedUsers($scope.SelectedCodeID);
                   
                }
                //var msg = data ? "Success" : "Failed";
                //$scope.ResultMsg = msg; 
                $scope.adminspinner = false;
            }, function (xhr) {
                console.log(xhr.error);
                $scope.adminspinner = false;
            });
        }
        else {
            $scope.Valid = true;
        }
    }

    $scope.GetAssignedUsers = function (codeid) {
        $scope.adminspinner = true;
        affiliationService.getAssignedUsers(codeid).then(function (res) {
            if (res.data != null && res.data.length > 0)
                $scope.AssignedUsers = res.data;
            else
                $scope.AssignedUsers = [];
            $scope.adminspinner = false;
        })
    }
   
    $scope.$watch('AssignedUsers', function (newValue, oldValue, scope) {
        if (newValue.length > 0)
            $scope.IsUserAssigned = true;
        else
            $scope.IsUserAssigned = false;
    });

    $scope.CouponUserAlert = function (code, IsExpired) {
        if (!IsExpired)
            $scope.AlertMsg = "Maximum Coupon Redemption Limit Reached. You cannot assign this coupon to an affiliate";
        else
            $scope.AlertMsg = "This coupon code has expired.";
        $scope.SelectedCode = code;
        $('#MaxRedeemAlert').modal('show');
    }
}])