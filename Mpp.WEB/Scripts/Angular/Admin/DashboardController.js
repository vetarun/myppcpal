app.controller("DashboardCtrl", ['$scope', '$filter', '$window', '$interval', 'sellerService', 'deleteService', function ($scope, $filter, $window, $interval, sellerService, deleteService) {


    //var _adminuser = this; // Replace scope with this one later
    /* Scope Properties */
    $scope.ModalData = [];
    $scope.IsVisible = false;
   
    //Pagination
    $scope.gap = 5;
    $scope.userId = 0;
    $scope.filteredItems = [];
    $scope.groupedItems = [];
    $scope.itemsPerPage = 5;
    $scope.pagedItems = [];
    $scope.currentPage = 0;
    $scope.PaymentData = [];
    $scope.selectedCampaign = '';
    $scope.filterParams = {
        nameSearch: '',
        SelectedPlan: 'All',
        SelectedStatus: 1
    };
    $scope.CampID = 0;
    $scope.sort = {
        sortingOrder: 'FirstName',
        reverse: false
    };

    $scope.Options = {
        start: 0,
        length: 10
    };

    $scope.message = '';
    $scope.Seller = {
        SellerID: 0,
        Stp_CustID: '',
        FirstName: '',
        LastName: '',
        Email: '',
        Active: '',
        StartDate: '',
        Plan: '',
        ProfileId: '',
        SellerAccess: '',
        PlanStatus: ''
    };

    $window.onload = function () {
        $scope.GetSellerDashboard(0);

        $scope.filterParams.SelectedStatus = 'All';
    }

    $scope.GetSellerDashboard = function (type) {
        $scope.adminspinner = true;
        $scope.SetDefault('FirstName', false);
        var SellerData = sellerService.getSellerDashboard(type);
        SellerData.then(function (res) {
          $scope.AdminData = res.data.Dashboard;
            var _modaldata = res.data.SellerUsers;
            if (_modaldata.length > 0) {
                if (type==0)
                $scope.ModalData = _modaldata;
                $scope.showmsg = false;
                $scope.search();
                $scope.adminspinner = false;
            }
            else {
                $scope.ModalData = {};
                $scope.pagedItems = {};
                $scope.showmsg = true;
                $scope.adminspinner = false;
            }
        }, function (xhr) {
            console.log(xhr.error);
            $scope.adminspinner = false;
        });
    };
    /*function to get Yearly revenue data */
    function GetYearlyRevenue(type) {
        $scope.adminspinner = true;
        var yearlyRevenueData = sellerService.GetYearlyRevenue(type);
        yearlyRevenueData.then(function (yearlyPayDetail) {
            if (yearlyPayDetail.data.length > 0) {
                console.log(yearlyPayDetail.data[0])
                $scope.yearlyPaymentData = yearlyPayDetail.data;
                $scope.showmsg = false;
                $("#YearlyRevenueModel").modal("show");
                $scope.adminspinner = false;
            }
            else {
                $scope.yearlyPaymentData = {};
                $scope.showmsg = true;
                $scope.adminspinner = false;
            }
        }, function (xhr) {
            console.log(xhr.error);
            $scope.adminspinner = false;
        });
    }

    /* get Revenue */
    function GetRevenue(type) {      
        debugger;
        $scope.adminspinner = true;
        var RevenueData = sellerService.GetRevenue(type);
        RevenueData.then(function (payDetail) {
            if (payDetail.data.length > 0) {
                console.log(payDetail.data[0].PayDate)
                $scope.PaymentData = payDetail.data;
                $scope.showmsg = false;
                $("#RevenueModel").modal("show");
                $scope.adminspinner = false;
            }
            else {
                $scope.PaymentData = {};
                $scope.showmsg = true;
                $scope.adminspinner = false;
            }
        }, function (xhr) {
            console.log(xhr.error);
            $scope.adminspinner = false;
        });
    }
    $scope.CloseEditor = function CloseModal() {
        $scope.Seller = {
            SellerID: 0,
            FirstName: '',
            LastName: '',
            Email: '',
            Active: '',
            StartDate: '',
            Plan: '',
            ProfileId: '',
            SellerAccess: '',
            PlanStatus: ''
        };
        $('.ProjAlertFull').removeClass('active');
        $('#divError').html('');
        $('#txtpromocode').removeClass('divPromoError');
        $('#txtpromocode').val('');
        $('#hdncustmid').val('');
    }

    /* Get Sellers*/
    function GetSellers(type) {
        
        $scope.adminspinner = true;
        var SellerData = sellerService.getSellers(type);
        SellerData.then(function (res) {
           
            if (res.data.length > 0) {

                $scope.ModalData = res.data;
                $scope.showmsg = false;

                $scope.search();
                $scope.adminspinner = false;
            }
            else {
                $scope.ModalData = {};
                $scope.pagedItems = {};
                $scope.showmsg = true;
                $scope.adminspinner = false;
            }
        }, function (xhr) {
            console.log(xhr.error);
            $scope.adminspinner = false;
        });
    }
    $scope.Close = function CloseModal() {
        $scope.Seller = {
            SellerID: 0,
            FirstName: '',
            LastName: '',
            Email: '',
            Active: '',
            StartDate: '',
            Plan: '',
            ProfileId: '',
            SellerAccess: '',
            PlanStatus: ''
        };
        $('.ProjAlertFull').removeClass('active');
        $('#divError').html('');
        $('#txtpromocode').removeClass('divPromoError');
        $('#txtpromocode').val('');
        $('#hdncustmid').val('');
    }

    //Check form Validation 
    $scope.$watch('formemail.$valid', function (newValue) {
        $scope.isFormValid = newValue;
        if ($scope.isFormValid) {
            $scope.message = '';
        }
    });

    $scope.SetSellerEmail = function (email) {
        $scope.sellerEmail = email;
    }

    $scope.SendCode = function () {
        var msg;
        $scope.submitted = true;
        $scope.message = '';
        if ($scope.isFormValid && $scope.sellerEmail !== "") {
            $scope.adminspinner = true;
            sellerService.SendEmail($scope.sellerEmail, $scope.SellerCode).then(function (d) {
                if (d.data == '') {
                    msg = "Email sent Successfully!";
                    displayMsg(msg, 1);
                } else {
                    displayMsg(d.data, 2);
                }
                $scope.adminspinner = false;
            }, function (xhr) {
                $scope.adminspinner = false;
                console.log(xhr.error);
                displayMsg(msg, 2);
            });
        }
    }

    $scope.CloseEmailModal = function () {
        $('.modal').modal('hide');
        $scope.SellerCode = '';
        $scope.sellerEmail = '';
        $scope.formemail.$setPristine(true); //here formemail our form name
        $scope.submitted = false;
        $scope.message = '';
    }

    $scope.CancelEmailModal = function () {
        $scope.SellerCode = '';
        $scope.formemail.$setPristine(true); //here formemail our form name
        $scope.submitted = false;
        $scope.message = '';
    }

    $scope.UpdateSeller = function (sid) {
        var status = document.getElementById('chkSeller_' + sid).checked;
        var profileId = $scope.Seller.ProfileId;
        var sellerAccess = $scope.Seller.SellerAccess;
        $('#divError').html('');
        $('#txtpromocode').removeClass('divPromoError');
        var seller_status = document.getElementById('chkSellerAccess_' + sid).checked;

        if ((seller_status != sellerAccess) && (profileId == null || profileId == '')) {
            displayMsg("You can't update the seller access as profileId is not found");
            document.getElementById('chkSellerAccess_' + sid).checked = !seller_status;
            document.getElementById('chkSeller_' + sid).checked = $scope.Seller.Active;

        }else {
            $scope.Seller.Active = status;
            $scope.Seller.SellerAccess = seller_status;
            sellerService.updateSeller($scope.Seller).then(function (d) {
                if (d.data == '') {
                    displayMsg("Updated Successfully!");
                    GetSellers(0);
                } else {
                    $scope.UpdateSlider(sid);
                    //document.getElementById('chkSeller_' + sid).checked = !status;
                    //document.getElementById('chkSellerAccess_' + sid).checked = !seller_status;
                    displayMsg(d.data);
                }
            }, function () {
                $scope.UpdateSlider(sid);
                //document.getElementById('chkSeller_' + sid).checked = !status;
                //document.getElementById('chkSellerAccess_' + sid).checked = !seller_status;
                displayMsg("Error!");
            });
        }
    }
    //function for divshow and hide 
    //This will hide the DIV by default.

    $scope.ShowHide = function () {
        //If DIV is visible it will be hidden and vice versa.
        $scope.IsVisible = $scope.IsVisible ? false : true;
    }
    $scope.DeleteSeller = function (sid, cid) {
        var confrm = false;
        confrm = $window.confirm("Are you sure you want to delete a user? Note: You can't get back the account once it is deleted");
        if (confrm) {
            $scope.adminspinner = true;
            var myDataPromise = deleteService.deleteUser(sid, cid);
            myDataPromise.then(function (result) {
                // this is only run after user() resolves
                if (result == '') {
                    displayMsg("Deleted Successfully!", 1);
                    $scope.CloseEditor();
                    $scope.SetDefault('FirstName', false);
                    GetSellers(0);
                    $scope.adminspinner = false;
                } else {
                    $scope.adminspinner = false;
                    displayMsg(result, 2);
                }
            }, function () {
                $scope.adminspinner = false;
                displayMsg("Error!", 2);
            });
        }
    }

    /* Update Seller details*/
    $scope.UpdateSlider = function (sellerid) {
        $('.ProjAlertFull').addClass('active');
        for (i = 0; i < $scope.pagedItems.length; i++) {
            for (j = 0; j < $scope.pagedItems[i].length; j++) {
                if ($scope.pagedItems[i][j].SellerID == sellerid) {
                    $scope.Seller = {
                        SellerID: sellerid,
                        Stp_CustID: $scope.pagedItems[i][j].Stp_CustID,
                        FirstName: $scope.pagedItems[i][j].FirstName,
                        LastName: $scope.pagedItems[i][j].LastName,
                        Email: $scope.pagedItems[i][j].Email,
                        StartDate: $scope.pagedItems[i][j].StartDate,
                        Plan: $scope.pagedItems[i][j].Plan,
                        Active: $scope.pagedItems[i][j].Active,
                        ProfileId: $scope.pagedItems[i][j].ProfileId,
                        SellerAccess: $scope.pagedItems[i][j].SellerAccess,
                        PlanStatus: $scope.pagedItems[i][j].PlanStatus
                    };
                    break;
                }
            }
        }
    }

    /* Send Activation Email*/
    $scope.ActivationEmail = function () {
        if ($scope.Seller.Active == false) {
            sellerService.activationEmail($scope.Seller).then(function (d) {
                if (d.data == '')
                    displayMsg('Activation email has been sent');
                else
                    displayMsg(d.data);
            }, function () {
                displayMsg("Error!");
            });
        } else {
            displayMsg("User is already active in the system");
        }
    }

    /*Filters - Not required now*/

    $scope.MatchPlanfilter = function (item) {
        return (item.Plan === 'A' || item.Plan === 'B' || item.Plan == 'C' || item.Plan === 'Trial');
    }

    $scope.MatchStatusfilter = function (item) {
        if (item == 'All') {
            return (item.Active === true || item.Active === false);
        }
    }

    $scope.filterStatus = function (test) {
        if (test == 'Active') {
            return true;
        } else {
            return false;
        }
    }

    function displayMsg(msg, type) {
        if (type == 1) {
            $('.msg-box').css("display", "block");
            document.getElementById('divresponse').innerHTML = msg;
            setTimeout(function () { $('.msg-box').fadeOut(); }, 2000);
        } else {
            $('.errormsg').css("display", "block");
            $scope.message = msg;
            setTimeout(function () { $('.errormsg').fadeOut(); }, 2000);
        }
    }

    //Loader
    function unloadLoader() {
        $scope.campspinner = false;
    }

    $scope.$watchCollection('filterParams', function (newNames, oldNames) {
        $scope.search();
    });

    var searchMatch = function (haystack, needle) {
        if (needle.length >= 3) {
            var flag = haystack.toLowerCase().search(needle.toLowerCase());
            return flag >= 0;
        }
        else
            return true;

    };

    // init the filtered items
    $scope.search = function () {  
        var filtered = [];
        filtered = $filter('filter')($scope.ModalData, function (item) {
            if (searchMatch(item['FirstName'], $scope.filterParams.nameSearch) || searchMatch(item['Email'], $scope.filterParams.nameSearch)) {
                return true;
            }
            return false;
        });
        if ($scope.filterParams.SelectedStatus != "All") {
            if ($scope.filterParams.SelectedStatus == 1)
                filtered = $filter('filter')(filtered, { Active: true }, true);
            else
                filtered = $filter('filter')(filtered, { Active: false }, true);
        }

        if ($scope.filterParams.SelectedPlan != 'All')
            filtered = $filter('filter')(filtered, { Plan: $scope.filterParams.SelectedPlan }, true);

        // take care of the sorting order
        if ($scope.sort.sortingOrder !== '') {
            filtered = $filter('orderBy')(filtered, $scope.sort.sortingOrder, $scope.sort.reverse);
        }

        $scope.filteredItems = filtered;
        $scope.currentPage = 0;
        $scope.Options.start = 0;
        // now group by pages
        $scope.groupToPages();
    };

    // calculate page in place
    $scope.groupToPages = function () {
        $scope.pagedItems = [];
        for (var i = 0; i < $scope.filteredItems.length; i++) {
            if (i % $scope.Options.length === 0) {
                $scope.pagedItems[Math.floor(i / $scope.Options.length)] = [$scope.filteredItems[i]];
            } else {
                $scope.pagedItems[Math.floor(i / $scope.Options.length)].push($scope.filteredItems[i]);
            }

        }
    };

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

    $scope.prevPage1 = function () {
       
        if ($scope.currentPage > 0) {
            $scope.campspinner = true;
            var size = parseInt($scope.Options.length);
            $scope.currentPage--;
            $scope.Options.start = parseInt($scope.Options.start) - size;
            $interval(unloadLoader, 500);
        }
    };

    $scope.nextPage1 = function () {
        if ($scope.currentPage < $scope.pagedItems.length - 1) {
         
            $scope.campspinner = true;
            $scope.currentPage++;
            $scope.Options.start = parseInt($scope.Options.start) + parseInt($scope.Options.length);
            $interval(unloadLoader, 500);
        }
    };

    $scope.setPage1 = function () {
        var pageno = this.n;
        if ($scope.currentPage != pageno) {
            $scope.campspinner = true;
            $scope.currentPage = pageno;
            $scope.Options.start = pageno * $scope.Options.length;
            $interval(unloadLoader, 500);
        }
    };

    $scope.getBackgroundColor = function (value) {
        var intial = value.charAt(0);
        var color = setInitialColor(intial);
        return {
            background: color
        };
    };

    $scope.sort_by = function (SortingOrder) {
        if ($scope.sort.sortingOrder == SortingOrder) {
            $scope.sort.reverse = !$scope.sort.reverse;
        }
        $scope.sort.sortingOrder = SortingOrder;
        $scope.campspinner = true;
        $scope.search();
        $interval(unloadLoader, 500);
    };

    $scope.GetDataBySize1 = function () {
        $scope.sort.sortingOrder = 'FirstName';
        $scope.sort.reverse = false
        $scope.campspinner = true;
        $scope.search();
        $interval(unloadLoader, 500);
    }

    $scope.selectedCls = function (column) {
        if (column == $scope.sort.sortingOrder) {
            return ((($scope.sort.reverse) ? 'headerSort headerSortUp' : 'headerSort headerSortDown'));
        }
        else {
            return 'headerSort';
        }
    };

    //Set Modal sorting default values
    $scope.SetDefault = function (name, rev) {
        $scope.sort.sortingOrder = name;
        $scope.sort.reverse = rev;
    };

    //autocomplete for campaign selection in log
    $scope.initAutoComplete = function () {
        $scope.pagedItems1.records = null;
        $scope.pagedItems1.recordsTotal = 0;
        $scope.pagedItems1.lastpage = 0;
        $scope.getCampLog($scope.CampID, $scope.userId);
        //$scope.userId = userId;
        $scope.Type = 1;
        $("#srchCampaigns").val('');
        $("#srchCampaigns").autocomplete({
            minLength: 0,
            clearButton: true,
            selectFirst: true,
            source: function (req, res) {
                $('#srchSpiner').css('display','block')
                var data = sellerService.srchCampaignsByUser(req.term, $scope.userId).then(function (respo) {
                    res($.map(respo.data, function (item) {
                        return {
                            label: item.Name,
                            value: item.Id
                        };

                    }));
                    $('#srchSpiner').css('display', 'none')
                })
            },
            select: function (event, ui) {
                event.preventDefault();
                $(this).val(ui.item.label);
                $scope.selectedCampaign = ui.item.value;
                $scope.getCampLog(ui.item.value, $scope.userId);

            },
            appendTo: '#campaignSearchBox',
        }).bind('focus', function () {
            $(this).val('');
            $(this).autocomplete("search")
        });
    }
    $("#srchCampaigns").blur(function () {
        if ($(this).val() === '')
            $scope.getCampLog(-1, $scope.userId);
    });
    $("#srchCampaigns").click(function () {
        $(this).val('');
        $(this).autocomplete("search")
    });
   
    $scope.gap = 5;
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
    $scope.GetDataBySize = function () {
        var size = $scope.Options.length;
        $scope.Options = {
            start: 0,
            length: size
        };
        $scope.pagedItems1.currentPage = 0;
        if ($scope.Type == 1) {
            $scope.getCampLog($scope.CampID, $scope.userId);
        }
        else if ($scope.Type == 2) {
            $scope.GetUserAccessLog(false);
        }
    }

    $scope.prevPage = function () {
        if ($scope.pagedItems1.currentPage > 0) {
            var size = parseInt($scope.Options.length);
            $scope.pagedItems1.currentPage--;
            $scope.Options.start = parseInt($scope.Options.start) - size;
            if ($scope.Type == 1) {
                $scope.getCampLog($scope.CampID, $scope.userId);
            }
            else if ($scope.Type == 2) {
                $scope.GetUserAccessLog(false);
            }
        }
    };

    $scope.nextPage = function () {
        if ($scope.pagedItems1.currentPage < $scope.pagedItems1.lastpage - 1) {
            $scope.pagedItems1.currentPage++;
            $scope.Options.start = parseInt($scope.Options.start) + parseInt($scope.Options.length);
            if ($scope.Type == 1) {
                $scope.getCampLog($scope.CampID, $scope.userId);
            }
            else if ($scope.Type == 2) {
                $scope.GetUserAccessLog(false);
            }
        }
    };

    $scope.setPage = function () {
        var pageno = this.n;
        if ($scope.pagedItems1.currentPage != pageno) {
            $scope.pagedItems1.currentPage = pageno;
            $scope.Options.start = pageno * $scope.Options.length;
            if ($scope.Type == 1) {
                $scope.getCampLog($scope.CampID, $scope.userId);
            }
            else if ($scope.Type == 2) {
                $scope.GetUserAccessLog(false);
            }
        }
    };

    $scope.Close = function CloseModal() {
        $scope.pagedItems1 = {
            records: [],
            currentPage: 0,
            recordsTotal: 0,
            lastpage: 0
        }
        $scope.Options = {
            start: 0,
            length: 10
        };
        $scope.CampID = 0;
        $("#srchCampaigns").val('');
        $('.modal').modal('hide');
        
    }
    //Get Campaigns Log
    $scope.getCampLog = function (cid, userid) {
        $scope.adminspinner = true;
        $scope.CampID = cid;
        //   $scope.CampLogName = document.getElementById('row_' + cid).innerText;
        // $scope.Type = 1;
        var CmplogData = sellerService.GetCampLog($scope.Options, cid, userid);
        CmplogData.then(function (res) {
     
            var clog = res.data.data;
            if (clog!= null && clog.length > 0) {
                $scope.pagedItems1.records = clog;
                $scope.pagedItems1.recordsTotal = res.data.recordsTotal;
                $scope.pagedItems1.lastpage = Math.ceil(res.data.recordsTotal / $scope.Options.length);
                //if (cid == 0) {
                //    $("#srchCampaigns").val(res.data.FirstCampaign);
                //}
            }
            else {
              
                $scope.pagedItems1.records = null;
                $scope.pagedItems1.recordsTotal = 0;
                $scope.pagedItems1.currentPage = 0;
                $scope.pagedItems1.lastpage = 0;
                $scope.Options.start = 0;
                $scope.Options.length = 10;
               
              
            }
            $scope.adminspinner = false;
        }, function (xhr) {
            $scope.adminspinner = false;
            console.log(xhr.error)
        });

    }
   
    $scope.setUserId = function (userId) {
       
        $scope.pagedItems1.records = [];
        $scope.pagedItems1.currentPage = 0;
        $scope.pagedItems1.recordsTotal = 0;
        $scope.pagedItems1.lastpage = 0;
        $scope.Options.start = 0;
        $scope.Options.length = 10;

        $scope.CampID = 0;
        $scope.userId = userId;
        $('.logOptions').removeClass('active').first().addClass('active');
        $('.tab-pane').removeClass('active').first().addClass('active');
        $scope.initAutoComplete();
      
    }
    $scope.ClearPaging = function () {
        $scope.pagedItems1.records = [];
        $scope.pagedItems1.currentPage = 0;
        $scope.pagedItems1.recordsTotal = 0;
        $scope.pagedItems1.lastpage = 0;
        $scope.Options.start = 0;
        $scope.Options.length = 10;
    }
    
    $scope.GetUserAccessLog = function (flag) {
        
        if (flag)
            $scope.ClearPaging();
      
        $scope.Type = 2;
        $scope.adminspinner = true;
        var userAccessLog = sellerService.getUserAccessLog($scope.Options, $scope.userId);

        if (userAccessLog != null) {
            userAccessLog.then(function (res) {
                var clog = res.data.data;
                if (typeof (clog) !== 'undefined' && clog.length > 0) {
                    $scope.pagedItems1.records = clog;
                    $scope.pagedItems1.recordsTotal = res.data.recordsTotal;
                    $scope.pagedItems1.lastpage = Math.ceil(res.data.recordsTotal / $scope.Options.length);
                }
                else {
                    $scope.pagedItems1.records = null;
                    $scope.Options.start = 0;
                    $scope.Options.length = 10;
                }
                $scope.adminspinner = false;
            }, function (xhr) {
                $scope.adminspinner = false;
                console.log(xhr.error)
            });
        }
        $scope.adminspinner = false;
    }

    $scope.loadData = function (type) {
        $scope.filteredItems = [];
        if (type == 1) {
            GetSellers(1);
        
            $scope.GetSellerDashboard(type);
         
        } else if (type == 2) {
            
            GetSellers(2);
            $scope.GetSellerDashboard(type);
        
        }

        else if (type == 3) {
            
            GetSellers(3);
            $scope.GetSellerDashboard(type);
            
     
        }
        else if (type == 4) {
            $scope.filteredItems = [];
            
            GetSellers(4);
            $scope.GetSellerDashboard(type);
            
        }
        else if (type == 5) {
            
             GetSellers(5);
            $scope.GetSellerDashboard(type);

        }
        else if (type == 6) {
           
            GetSellers(6);
            $scope.GetSellerDashboard(type);

        }

        else if (type == 7) {
           
            GetSellers(7);
            $scope.GetSellerDashboard(type);
          
        }
        else {

            GetSellers(0);
            
        }
        
      
    }
  
    $scope.LoadYearData = function (type) {
        
        GetRevenue(type);
    }
    $scope.RevenueData = function (type) {
        debugger;
        if (type == 1 && $scope.AdminData.CurrentMonthlySales > 0) {
            GetRevenue(13);

        } else if (type == 2 && $scope.AdminData.LastMonthSales> 0) {
            GetRevenue(14);

        }

        else if ( type==0 && $scope.AdminData.TotalYearlySales > 0){
            GetYearlyRevenue(0);
        }

        return false;
    }
    
}])