campapp.controller("myCampaignsController", ['$scope', '$filter', '$window', '$interval', 'CampService', function ($scope, $filter, $window, $interval, CampService) {
    $scope.Campaigns = [];
    $scope.chklist = [];
    $scope.idSelectedCamp = null;
    $scope.iscopied = 0;
    $scope._formula = {
        FormulaName: '',
        AcosPause: '',
        AcosRaise: '',
        AcosLower: '',
        AcosNegative: '',
        ClicksPause: '',
        ClicksNegative: '',
        ClicksLower: '',
        SpendPause: '',
        SpendLower: '',
        SpendNegative: '',
        CTRPause: '',
        CTRNegative: '',
        CTRLower: '',
        BidLower: '',
        BidRaise: '',
        MaxBid: '',
        MinBid: '',
        BestSearchACos:'',
        BestSearchImpressons: '',
        IsBestSrchCheked: false
    };
    $scope.DisableBestSrchTerm = false;
    $scope.message = '';
    $scope.accessformula = 0;
    $scope.isFormValid = false;
    $scope.submitted = false;
    $scope.filteredItems = [];
    $scope.groupedItems = [];
    $scope.pagedItems = [];
    $scope.currentPage = 0;
    $scope.myCampaigns = {};
    $scope.filterParams = {
        nameSearch: '',
        SelectedName: 'All'
    };
 
    $scope.sort = {
        sortingOrder: 'Name',
        reverse: false
    };
    $scope.formula = "";
    //Pagination
    $scope.gap = 5;
    $scope.pagedItems1 = {
        records: [],
        currentPage: 0,
        recordsTotal: 0,
        lastpage: 0
    };
    //MyCamp options
    $scope.Options1 = {
        start: 0,
        length: 10
    };
    //Log options
    $scope.Options = {
        start: 0,
        length: 10
    };

    $window.onload = function () {
        $scope.GetCampaigns();
    }

    $scope.GetCampaigns = function () {
        debugger
        $scope.SetDefault('Name', false);
        $scope.campspinner = true;
        var CmpData = CampService.getmyCampaigns();
        CmpData.then(function (res) {
            var myCampaigns = res.data.optimizecamp;
            console.log(myCampaigns);
            var formula = res.data.formula;
            $scope.formula = formula.FormulaName;
            $scope.templatename = $scope.formula;
            $scope.DisableBestSrchTerm = formula.IsBestSrchCheked;
            if (formula.IsBestSrchTerm && formula.FormulaName === 'Custom') {
                $scope.Formula.BestSearchACos = formula.BestSearchACos;
                $scope.Formula.BestSearchImpressons = formula.BestSearchImpressons;
            }
            $scope.accessformula = res.data.IsSetFormula;
            if (myCampaigns.length > 0) {
                $scope.myCampaigns = myCampaigns;
                $scope.Formula = formula;
                if ($scope.accessformula == 0) {
                    disableClick();
                }
                setTempFormula(formula);
                var idselected = $scope.myCampaigns[0].RecordID;
                $scope.setSelected(idselected, 1);
                $scope.search();
            }
            else {
                $scope.myCampaigns = {};
            }
            $scope.campspinner = false;
        }, function (xhr) {
            $scope.campspinner = false;
            console.log(xhr.error);
        });
    }
    $scope.KID1 = 0;
    $scope.CID1 = 0;
    $scope.AID1 = 0;
    $scope.RID1 = 0;
    $scope.RepID1 = 0;
    $scope.IsChecked = 0;
	var PopUp1 = 0;
	$scope.RevertChanges = function (KeyId, CampId, AdgroupID, ReportID, ReasonID, ModifiedOn) {
		
        $scope.modalspinner1 = true;
        $scope.KID1 = KeyId;
        $scope.CID1 = CampId;
        $scope.AID1 = AdgroupID;
        $scope.RID1 = ReasonID;
        $scope.RepID1 = ReportID;
        $scope.ModifiedDate = ModifiedOn;
		if (PopUp1 != 1) {
			$scope.modalspinner1 = false;
            $('.RevertedDataInfo').modal('show');
           
        }
        else {
            $scope.RevertChangesConfirmed(KeyId, CampId, AdgroupID, ReportID, ReasonID, ModifiedOn);
        }
        
    }
    $scope.RevertChangesConfirmed = function (KeyId, CampId, AdgroupID, ReportID, ReasonID, ModifiedDate) {
      
		$('.RevertedDataInfo').modal('hide');
        $scope.modalspinner1 = true;
        var checked = $('#cb').is(":checked");
        //if ($('#cb').is(":checked")) {  
        //    var Data = CampService.HideWarning();
        //    Data.then(function (res) {
        var kData = CampService.RevertChanges(KeyId, CampId, AdgroupID, ReportID, ReasonID, ModifiedDate, checked);
                kData.then(function (res) {
                    $scope.getKeyLog(CampId);
                   // $scope.UpdateOnAmz(KeyId, CampId, AdgroupID, ReportID, ReasonID, ModifiedDate);
                    $scope.modalspinner1 = false;
                });
            //});
        //}
        //else {
        //    var kData = CampService.RevertChanges(KeyId, CampId, AdgroupID, ReportID, ReasonID, ModifiedDate);
        //    kData.then(function (res) {

        //       // $scope.UpdateOnAmz(KeyId, CampId, AdgroupID, ReportID, ReasonID, ModifiedDate);
        //        $scope.modalspinner1 = false;
        //    });
        //}
       }
    //$scope.UpdateOnAmz = function (keyId, CampId, AdgroupID, ReportID, ReasonID, ModifiedDate) {
    //    var kData = CampService.UpdateChanges(keyId, CampId, AdgroupID, ReportID, ReasonID, ModifiedDate);
    //    kData.then(function (res) {

    //       
    //        $scope.modalspinner1 = false;
    //    });
    //}
    $scope.ClosePopUp = function () {
        $scope.modalspinner1 = false;
    }
    //Get Formula
 
    $scope.getFormula = function (cid, index, isBSTChecked) {
        debugger
        if ($scope.accessformula != 0) {
            $scope.iscopied = false;
            $scope.setSelected(cid, index);
            var CmpData = CampService.getCampFormula(cid);
            CmpData.then(function (res) {
                var formula = res.data;
                $scope.formula = res.data.FormulaName;
                $scope.templatename = $scope.formula;
                $scope.DisableBestSrchTerm = isBSTChecked;
                $scope.Formula = formula;
                setTempFormula(formula);
            }, function () {
                $scope.divresponse = "Error!";
            });
        } else {
            disableClick();
        }
    }

    ////Check form Validation 
    //$scope.$watch('formulaForm.$valid', function (newValue) {
    //    
    //    $scope.isFormValid = newValue;
    //    if ($scope.isFormValid) {
    //        $scope.message = '';
    //    }
    //});

    //Check form Validation 
    $scope.$watchGroup(['customForm.$valid', 'customForm.$dirty'], function (newValue) {
        $scope.isFormValid = newValue[0];
        var isFormDirty = newValue[1];
        if ($scope.isFormValid) {
            $scope.message = '';
            $('.msg-box1').css('display', 'none');
        } else {
            if (!$scope.isFormValid && isFormDirty) {
                $('.msg-box1').css('display', 'block');
            }
        }
    });


    $scope.$watchCollection('filterParams', function (newNames, oldNames) {
        $scope.campspinner = true;
        $scope.search();
        $interval(unloadLoader, 500);
    });

    //Save formula
    $scope.SaveFormula = function (formula) {
        var msg;
        
        if ($scope.accessformula != 0) {
            $scope.submitted = true;
            if ($scope.isFormValid) {
               
                    var confm = confirm("Please make sure the values are correct!");
                    if (confm) {
                        var cid = $scope.idSelectedCamp;
                        if (cid > 0) {
                            $scope.modalspinner = true;
                            var CmpData = CampService.saveFormula(cid, formula);
                            CmpData.then(function (res) {
                                
                                var result = res.data;
                                if (result == "") {
                                    msg = "Updated Successfully!";
                                    $scope.modalspinner = false;
                                    displayMsg(msg, 1);
                                    $scope.ResetForm();
                                    $scope.DisableBestSrchTerm = formula.IsBestSrchCheked;
                                    for (i = 0; i < $scope.myCampaigns.length; i++) {
                                        if ($scope.myCampaigns[i].RecordID == cid) {
                                            $scope.myCampaigns[i].FormulaName = formula.FormulaName;
                                            break;
                                        }
                                    }
                                    setTempFormula(formula);
                                }
                                else {
                                    $scope.modalspinner = false;
                                    displayMsg(result, 2);
                                }
                            }, function () {
                                $scope.modalspinner = false;
                                msg = "Error occured!";
                                displayMsg(msg, 2);
                            });
                        }
                        else {
                            msg = "Something went wrong! Please try again";
                            displayMsg(msg, 1);
                        }
                    }
               
                else {

                }
            } else {
                msg = 'Please fill the required fields';
                displayMsg(msg, 2);
                $('.msg-box1').css('display', 'block');
            }
        } else {
            disableClick();
        }
    }

    //Copy Formula
    $scope.copyFormula = function (cid, index) {
        if ($scope.accessformula != 0) {
            $scope.templatename = "";
            $scope.getFormula(cid, index);
            $scope.iscopied = true;
            displayMsg("Copied!", 1);
        } else {
            disableClick();
        }
    }

    //Paste Formula
    $scope.pasteFormula = function (pid) {
        var msg;
        var copyid = $scope.idSelectedCamp;
        if ($scope.accessformula != 0) {
            if (copyid != pid) {
                var confrm = confirm("Are you sure you want to apply the new formula?");
                if (confrm) {
                    if ($scope.iscopied == true && copyid > 0) {
                        $scope.modalspinner = true;
                        var result = CampService.pasteFormula(copyid, pid);
                        result.then(function (res) {
                            var result = res.data;
                            if (result == "") {
                                msg = "Updated Successfully!";
                                for (i = 0; i < $scope.myCampaigns.length; i++) {
                                    if ($scope.myCampaigns[i].RecordID == copyid) {
                                        var fname = $scope.myCampaigns[i].FormulaName;
                                        for (j = 0; j < $scope.myCampaigns.length; j++) {
                                            if ($scope.myCampaigns[j].RecordID == pid) {
                                                $scope.myCampaigns[j].FormulaName = fname;
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                            else {
                                msg = result;
                            }
                            $scope.modalspinner = false;
                            displayMsg(msg, 1);
                        }, function () {
                            msg = "Error occured!";
                            $scope.modalspinner = false;
                            displayMsg(msg, 1);
                        });
                    }
                    else {
                        msg = "Please copy any formula to apply!";
                        displayMsg(msg, 1);
                    }
                    displayMsg(msg, 1);
                }
            }
        } else {
            disableClick();
        }
    }

    $scope.pastAll = function () {
        var copyid = $scope.idSelectedCamp;
        var fname;
        if ($scope.accessformula != 0) {
            var confrm = confirm("Are you sure you want to apply the formula to all campaigns?");
            if (confrm) {
                if ($scope.iscopied == true) {
                    $scope.modalspinner = true;
                    var result = CampService.PastAllCampaigns(copyid);
                    result.then(function (res) {
                        var result = res.data;
                        if (result == "") {
                            for (i = 0; i < $scope.myCampaigns.length; i++) {
                                if ($scope.myCampaigns[i].RecordID == copyid) {
                                    fname = $scope.myCampaigns[i].FormulaName;
                                    break;
                                }
                            }
                            for (j = 0; j < $scope.myCampaigns.length; j++) {
                                $scope.myCampaigns[j].FormulaName = fname;
                            }
                            $scope.modalspinner = false;
                            displayMsg("Updated Successfully!", 1);
                        }
                        else {
                            $scope.modalspinner = false;
                            displayMsg(result, 2);
                        }
                    }, function () {
                        $scope.modalspinner = false;
                        displayMsg("Error occured!", 1);
                    });
                } else {
                    displayMsg("Please copy any formula to apply!", 1);
                }
            }
        } else {
            disableClick();
        }
    }

    //Run Formula
    $scope.runCampaign = function (cid, status) {
        var confrm = false;
        var newstatus;
        var checkid = 'check' + cid;
        var alerttxt = '';
        if ($scope.accessformula == 0) {
            alerttxt = "First time activation means that you are accepting all terms&conditions. Please click on OK to proceed."
        } else {
            alerttxt = "Are you sure you want to run campaign?";
        }
        if (status == true) {
            confrm = $window.confirm("Are you sure you don't want to run campaign? ");
            newstatus = 0;
        }
        else {
            confrm = $window.confirm(alerttxt);
            newstatus = 1;
        }
        if (confrm) {
            $scope.campspinner = true;
            var CmpData = CampService.runCampaign(cid, newstatus);
            CmpData.then(function (res) {
                var result = res.data;
                if (result == "") {
                    for (i = 0; i < $scope.myCampaigns.length; i++) {
                        if ($scope.myCampaigns[i].RecordID == cid) {
                            $scope.myCampaigns[i].Status = newstatus;
                            break;
                        }
                    }
                    if ($scope.accessformula == 0)
                        $('#divSuccessPopup').css('display', 'block');
                    else
                        displayMsg("Updated Successfully!", 1);
                    enableClick();
                }
                else {
                    document.getElementById(checkid).checked = status;
                    displayMsg(result, 1);
                }
                $scope.campspinner = false;
            }, function () {
                document.getElementById(checkid).checked = status;
                $scope.campspinner = false;
                displayMsg("Error occured!", 1);
            });
        }
        else {
            document.getElementById(checkid).checked = status;
        }
    }

    $scope.checkAllCamps = function () {
        $scope.chklist = [];
        setCampaignStatus('check_');
        if ($scope.chklist.length > 0) {
            $scope.modalspinner = true;
            var CmpData = CampService.runCampaigns($scope.chklist);
            CmpData.then(function (res) {
                var result = res.data;
                if (result == "") {
                    $scope.CloseModel();
                    $scope.modalspinner = false;
                    displayMsg("Updated Successfully!", 1);
                    UpdateCampaignStatus();
                }
                else {
                    $scope.modalspinner = false;
                    displayModalMsg(result, 2);
                }
            }, function () {
                $scope.modalspinner = false;
                displayModalMsg("Error occured!", 2);
            });
        }
    }

    // Set row selection
    $scope.setSelected = function (idSelectedCamp, index) {
        $scope.idSelectedCamp = idSelectedCamp;
        $scope.idSelectedaction = index;

    }

    // Set Formula templates
    $scope.ChangeFormula = function (templatename,current) {
        
        $scope.Formula.IsBestSrchCheked = false;
        $scope.newData = true;
        $scope.Formula = {};
        if (templatename == 'Recommended') {
            $scope.Formula.FormulaID = 2;
            $scope.Formula.MinBid = current == 'Recommended' ? $scope._formula.MinBid : 0.25;
            $scope.Formula.MaxBid = current == 'Recommended' ? $scope._formula.MaxBid : 2.50;
        }
        else if (templatename == 'Conservative') {
            $scope.Formula.FormulaID = 1;
            $scope.Formula.MinBid = current == 'Conservative' ? $scope._formula.MinBid :0.10;
            $scope.Formula.MaxBid = current == 'Conservative' ? $scope._formula.MaxBid :1.50;
        }
        else if (templatename == 'Aggressive') {
            $scope.Formula.FormulaID = 3;
            $scope.Formula.MinBid = current == 'Aggressive' ? $scope._formula.MinBid :0.50;
            $scope.Formula.MaxBid = current == 'Aggressive' ? $scope._formula.MaxBid :3.50;
        }
        else if (templatename == 'Custom') {
            $scope.Formula.BestSearchACos = $scope._formula.BestSearchACos;
            $scope.Formula.BestSearchImpressons = $scope._formula.BestSearchImpressons;
            $scope.Formula.FormulaName = $scope._formula.FormulaName
            $scope.Formula.AcosPause = $scope._formula.AcosPause;
            $scope.Formula.AcosRaise = $scope._formula.AcosRaise;
            $scope.Formula.AcosLower = $scope._formula.AcosLower;
            $scope.Formula.AcosNegative = $scope._formula.AcosNegative;
            $scope.Formula.ClicksPause = $scope._formula.ClicksPause;
            $scope.Formula.ClicksNegative = $scope._formula.ClicksNegative;
            $scope.Formula.ClicksLower = $scope._formula.ClicksLower;
            $scope.Formula.SpendPause = $scope._formula.SpendPause;
            $scope.Formula.SpendLower = $scope._formula.SpendLower;
            $scope.Formula.SpendNegative = $scope._formula.SpendNegative;
            $scope.Formula.CTRPause = $scope._formula.CTRPause;
            $scope.Formula.CTRNegative = $scope._formula.CTRNegative;
            $scope.Formula.CTRLower = $scope._formula.CTRLower;
            $scope.Formula.BidLower = $scope._formula.BidLower;
            $scope.Formula.BidRaise = $scope._formula.BidRaise;
            $scope.Formula.MaxBid = $scope._formula.MaxBid;
            $scope.Formula.MinBid = $scope._formula.MinBid;
            $scope.Formula.IsBestSrchCheked = $scope._formula.IsBestSrchCheked;
        }
        $scope.Formula.FormulaName = templatename;
        $scope.ResetForm();
        if (templatename == "") {
            $scope.Cancel();
        }
    }

    //Get Campaigns Log
    $scope.getCampLog = function (cid) {
        if ($scope.accessformula != 0) {
            $scope.modalspinner = true;
            $scope.CampID = cid;
            $scope.CampLogName = document.getElementById('row_' + cid).innerText;
            $scope.Type = 1;
            var CmplogData = CampService.GetCampLog($scope.Options, cid);
            CmplogData.then(function (res) {
                var clog = res.data.data;
                if (clog.length > 0) {
                    $scope.pagedItems1.records = clog;
                    $scope.pagedItems1.recordsTotal = res.data.recordsTotal;
                    $scope.pagedItems1.lastpage = Math.ceil(res.data.recordsTotal / $scope.Options.length);
                }
                else {
                    $scope.pagedItems1.records = null;
                }
                $('.logDataInfo').modal('show');
                $scope.modalspinner = false;
            }, function (xhr) {
                $scope.modalspinner = false;
                console.log(xhr.error)
            });
        } else {
            disableClick();
        }
    }

    //Get Keywords Log
	$scope.getKeyLog = function (cid) {
		
        if ($scope.accessformula != 0) {
            $scope.modalspinner = true;
            $scope.CampLogName = document.getElementById('row_' + cid).innerText;
            $scope.CampID = cid;
            $scope.Type = 2;
            var KeylogData = CampService.GetKeyLog($scope.Options, cid);
            KeylogData.then(function (res) {
                var clog = res.data.data;
				if (clog.length > 0) {
				 PopUp1 = clog[0].PopUpID;
                    $scope.pagedItems1.records = clog;
                    $scope.pagedItems1.recordsTotal = res.data.recordsTotal;
                    $scope.pagedItems1.lastpage = Math.ceil(res.data.recordsTotal / $scope.Options.length);
                }
                else {
                    $scope.pagedItems1.records = null;
                }
                $('.keylogDataInfo').modal('show');
                $scope.modalspinner = false;
            }, function () {
                var msg = "Error!";
                $scope.modalspinner = false;
            });
        } else {
            disableClick();
        }
    }

    //Loader
    function unloadLoader() {
        $scope.campspinner = false;
    }

    $scope.Cancel = function () {
        if ($scope.accessformula != 0) {
            $scope.Formula.FormulaName = $scope._formula.FormulaName
            $scope.Formula.AcosPause = $scope._formula.AcosPause;
            $scope.Formula.AcosRaise = $scope._formula.AcosRaise;
            $scope.Formula.AcosLower = $scope._formula.AcosLower;
            $scope.Formula.AcosNegative = $scope._formula.AcosNegative;
            $scope.Formula.ClicksPause = $scope._formula.ClicksPause;
            $scope.Formula.ClicksNegative = $scope._formula.ClicksNegative;
            $scope.Formula.ClicksLower = $scope._formula.ClicksLower;
            $scope.Formula.SpendPause = $scope._formula.SpendPause;
            $scope.Formula.SpendLower = $scope._formula.SpendLower;
            $scope.Formula.SpendNegative = $scope._formula.SpendNegative;
            $scope.Formula.CTRPause = $scope._formula.CTRPause;
            $scope.Formula.CTRNegative = $scope._formula.CTRNegative;
            $scope.Formula.CTRLower = $scope._formula.CTRLower;
            $scope.Formula.BidLower = $scope._formula.BidLower;
            $scope.Formula.BidRaise = $scope._formula.BidRaise;
            $scope.Formula.MaxBid = $scope._formula.MaxBid;
            $scope.Formula.MinBid = $scope._formula.MinBid;
            $scope.Formula.IsBestSrchCheked = $scope._formula.IsBestSrchCheked;
            $scope.Formula.BestSearchACos = $scope._formula.BestSearchACos;
            $scope.Formula.BestSearchImpressons = $scope._formula.BestSearchImpressons;
            $scope.templatename = '';
            $scope.submitted = false;
        } else {
            disableClick();
        }
    }

    //Model open and close hanlder 
    $scope.Select = function () {
        if ($scope.accessformula != 0) {
            $('.campSelectionDataInfo').modal("show");
            $scope.activeall = true;
        } else {
            $('#chkselectall').prop('checked', false);
            disableClick();
        }
    }

    $scope.sort_by = function (SortingOrder) {
        if ($scope.sort.sortingOrder == SortingOrder) {
            $scope.sort.reverse = !$scope.sort.reverse;
        }
        $scope.sort.sortingOrder = SortingOrder;
        $scope.campspinner = true;
        $scope.search();
        $interval(unloadLoader, 500);
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
    $scope.SetDefault = function (name, rev) {
        $scope.sort.sortingOrder = name;
        $scope.sort.reverse = rev;
    }

    $scope.CloseModel = function () {
        $('#chkselectall').prop('checked', false);
        $('.campSelectionDataInfo').modal("hide");
    }

    //Clear Form (reset)
    $scope.ResetForm = function () {
        $scope.customForm.$setPristine(true); //here fmuser our form name
        $scope.submitted = false;
        $scope.message = '';
    }

    /*Mycamps data by page size selection */
    $scope.GetCampDataBySize = function () {
        $scope.sort.sortingOrder = 'Name';
        $scope.sort.reverse = false
        $scope.campspinner = true;
        $scope.search();
        $interval(unloadLoader, 500);
    }

    $scope.GetDataBySize = function () {
        var size = $scope.Options.length;
        $scope.Options = {
            start: 0,
            length: size
        };
        $scope.pagedItems1.currentPage = 0;
        if ($scope.Type == 1)
            $scope.getCampLog($scope.CampID);
        else
            $scope.getKeyLog($scope.CampID);
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
            if ($scope.Type == 1)
                $scope.getCampLog($scope.CampID);
            else
                $scope.getKeyLog($scope.CampID);
        }
    };

    $scope.nextPage = function () {
       
        if ($scope.pagedItems1.currentPage < $scope.pagedItems1.lastpage - 1) {
            $scope.pagedItems1.currentPage++;
            $scope.Options.start = parseInt($scope.Options.start) + parseInt($scope.Options.length);
            if ($scope.Type == 1)
                $scope.getCampLog($scope.CampID);
            else
                $scope.getKeyLog($scope.CampID);
        }
    };

    $scope.setPage = function () {
      
        var pageno = this.n;
        if ($scope.pagedItems1.currentPage != pageno) {
            $scope.pagedItems1.currentPage = pageno;
            $scope.Options.start = pageno * $scope.Options.length;
            if ($scope.Type == 1)
                $scope.getCampLog($scope.CampID);
            else
                $scope.getKeyLog($scope.CampID);
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
        $('.modal').modal('hide');
    }

    /* MyCampaigns - paging */
    var searchMatch = function (haystack, needle) {
        return haystack.toLowerCase().includes(needle.toLowerCase());
    };

    // init the filtered items
    $scope.search = function () {
        var filtered = [];
        filtered = $filter('filter')($scope.myCampaigns, function (item) {
            if (searchMatch(item['Name'], $scope.filterParams.nameSearch)) {
                return true;
            }
            return false;
        });

        if ($scope.filterParams.SelectedName != 'All')
            filtered = $filter('filter')(filtered, { FormulaName: $scope.filterParams.SelectedName }, true);

        // take care of the sorting order
        if ($scope.sort.sortingOrder !== '') {
            filtered = $filter('orderBy')(filtered, $scope.sort.sortingOrder, $scope.sort.reverse);
        }

        $scope.currentPage = 0;
        $scope.Options1.start = 0;
        $scope.filteredItems = filtered;
        // now group by pages
        $scope.groupToPages();
    };

    // calculate page in place
    $scope.groupToPages = function () {
        $scope.pagedItems = [];
        for (var i = 0; i < $scope.filteredItems.length; i++) {
            if (i % $scope.Options1.length === 0) {
                $scope.pagedItems[Math.floor(i / $scope.Options1.length)] = [$scope.filteredItems[i]];
            } else {
                $scope.pagedItems[Math.floor(i / $scope.Options1.length)].push($scope.filteredItems[i]);
            }
        }
    };

    $scope.prevPage1 = function () {
        if ($scope.currentPage > 0) {
            $scope.campspinner = true;
            var size = parseInt($scope.Options1.length);
            $scope.currentPage--;
            $scope.Options1.start = parseInt($scope.Options1.start) - size;
            $interval(unloadLoader, 500);
        }
    };

    $scope.nextPage1 = function () {
        if ($scope.currentPage < $scope.pagedItems.length - 1) {
            $scope.campspinner = true;
            $scope.currentPage++;
            $scope.Options1.start = parseInt($scope.Options1.start) + parseInt($scope.Options1.length);
            $interval(unloadLoader, 500);
        }
    };

    $scope.setPage1 = function () {
        var pageno = this.n;
        if ($scope.currentPage != pageno) {
            $scope.campspinner = true;
            $scope.currentPage = pageno;
            $scope.Options1.start = pageno * $scope.Options1.length;
            $interval(unloadLoader, 500);
        }
    };

    function displayMsg(msg, type) {
        if (type == 1) {
            $('.msg-box').css("display", "block");
            document.getElementById('divresponse').innerHTML = msg;
            setTimeout(function () { $('.msg-box').fadeOut(); }, 2000);
        } else {
            $('.errormsg').css("display", "block");
            $scope.message = msg;
            setTimeout(function () { $('.errormsg').fadeOut(); }, 4000);
        }
    }

    function displayModalMsg(msg, type) {
        if (type == 1) {
            $('.msg-modalbox').css("display", "block");
            document.getElementById('divmodalresponse').innerHTML = msg;
            setTimeout(function () { $('.msg-modalbox').fadeOut(); }, 2000);
        } else {
            $('.modalerror').css("display", "block");
            $scope.modalmessage = msg;
            setTimeout(function () { $('.modalerror').fadeOut(); }, 2000);
        }
    }

    //Temporary storage data for Cancel event 
    function setTempFormula(formula) {
        $scope._formula.FormulaName = formula.FormulaName;
        $scope._formula.AcosPause = formula.AcosPause;
        $scope._formula.AcosRaise = formula.AcosRaise;
        $scope._formula.AcosLower = formula.AcosLower;
        $scope._formula.AcosNegative = formula.AcosNegative;
        $scope._formula.ClicksPause = formula.ClicksPause;
        $scope._formula.ClicksNegative = formula.ClicksNegative;
        $scope._formula.ClicksLower = formula.ClicksLower;
        $scope._formula.SpendPause = formula.SpendPause;
        $scope._formula.SpendLower = formula.SpendLower;
        $scope._formula.SpendNegative = formula.SpendNegative;
        $scope._formula.CTRPause = formula.CTRPause;
        $scope._formula.CTRNegative = formula.CTRNegative;
        $scope._formula.CTRLower = formula.CTRLower;
        $scope._formula.BidLower = formula.BidLower;
        $scope._formula.BidRaise = formula.BidRaise;
        $scope._formula.MaxBid = formula.MaxBid;
        $scope._formula.MinBid = formula.MinBid;
        $scope._formula.IsBestSrchCheked = formula.IsBestSrchCheked;
        $scope.Formula.BestSearchACos = formula.BestSearchACos;
        $scope.Formula.BestSearchImpressons = formula.BestSearchImpressons;
    }

    function setCampaignStatus(id) {
        for (i = 0; i < $scope.myCampaigns.length; i++) {
            $scope._campselection = {
                RecordID: '',
                Status: ''
            };
            $scope._campselection.RecordID = $scope.myCampaigns[i].RecordID
            $scope._campselection.Status = document.getElementById(id + $scope.myCampaigns[i].RecordID).checked;
            $scope.chklist.push($scope._campselection);
        }
    }

    function UpdateCampaignStatus() {
        for (i = 0; i < $scope.myCampaigns.length; i++) {
            for (j = 0; j < $scope.chklist.length; j++) {
                if ($scope.myCampaigns[i].RecordID == $scope.chklist[j].RecordID) {
                    $scope.myCampaigns[i].Status = $scope.chklist[j].Status;
                    break;
                }
            }
        }
    }
   

    function disableClick() {
        $('#divAcceptPopup').css('display', 'block');
        document.getElementById("tab_camp").childNodes[0].onclick = function () { $('#divAcceptPopup').css('display', 'block'); return false; };
        //document.getElementById("tab_dashboard").childNodes[0].onclick = function () { $('#divAcceptPopup').css('display', 'block'); return false; };
        //document.getElementById("tab_report").childNodes[0].onclick = function () { $('#divAcceptPopup').css('display', 'block'); return false; };
        //document.getElementById("tab_sett").childNodes[0].onclick = function () { $('#divAcceptPopup').css('display', 'block'); return false; };
        //document.getElementById("tab_help").childNodes[0].onclick = function () { $('#divAcceptPopup').css('display', 'block'); return false; };
        //document.getElementById("tab_notify").onclick = function () { $('#divAcceptPopup').css('display', 'block'); return false; };
        //document.getElementById("tab_profile").childNodes[0].onclick = function () { $('#divAcceptPopup').css('display', 'block'); return false; };
        $('#selectformula').prop("disabled", "disabled");
        $('#SelectedName').prop("disabled", "disabled");
    }

    function enableClick() {
        if ($scope.accessformula == 0) {
            $scope.accessformula = 1;
            document.getElementById("tab_camp").childNodes[0].onclick = function () { return true; };
            //document.getElementById("tab_dashboard").childNodes[0].onclick = function () { return true; };
            //document.getElementById("tab_report").childNodes[0].onclick = function () { return true; };
            //document.getElementById("tab_sett").childNodes[0].onclick = function () { return true; };
            //document.getElementById("tab_help").childNodes[0].onclick = function () { return true; };
            //document.getElementById("tab_notify").onclick = function () { return true; };
            //document.getElementById("tab_profile").childNodes[0].onclick = function () { return true; };
            //$('.modal-backdrop').css('display', 'none');
            $('#selectformula').removeProp("disabled");
            $('#SelectedName').removeProp("disabled");
        }
    }

    function moveprogressbar() {
        $('.progressbar').css('display', 'block');
        var elem = document.getElementById("myBar");
        var width = 10;
        var id = setInterval(frame, 10);
        function frame() {
            if (width >= 100) {
                clearInterval(id);
            } else {
                width++;
                elem.style.width = width + '%';
                elem.innerHTML = width * 1 + '%';
            }
        }
    }
    //

    function hideprogressbar() {
        $('.progressbar').css('display', 'none')
    }
    // fetch new campaigns
    $scope.FetchNewKeys = function () {
        $scope.modalspinner = true;
        var campData = CampService.FetchNewKeys();
        campData.then(function (res) {
            var totalFound = parseInt(res.data.Records);
            var status = parseInt(res.data.Status);
            var msg = "";
            
            if (status===1 && totalFound > 0) {
                msg = "We will check your account for new campaigns, please check back in a little while";
                $scope.GetCampaigns();
            }
            else if(status===1 && totalFound===0){
				msg = "There are no new campaigns on your account";
               }
            else if (status===2){
            msg = "Unauthorised profile access.";
            }
            else if (status==3) {
                msg = "Error while processing.Please try again.";
            }
            //$scope.NewCampaignRespo = msg;

            $('#NewCampaignRespo').text(msg);
        $('#divFetchNewCampaignPopup').css('display', 'block');
        $scope.modalspinner = false;
    }, function (xhr) {
        $scope.modalspinner = false;
        console.log(xhr.error)
    });
    }

    $scope.SetDefaultBstValues = function (isSelected) {
        
        if (isSelected && (typeof ($scope.Formula.BestSearchACos) == 'undefined' || $scope.Formula.BestSearchACos == '')) {
            $scope.Formula.BestSearchACos = $scope.Formula.AcosRaise;
        }
        if (isSelected && (typeof($scope.Formula.BestSearchImpressons) == 'undefined' || $scope.Formula.BestSearchImpressons == '')) {
            $scope.Formula.BestSearchImpressons = $scope.Formula.CTRPause;
        }
        
    }
//Validation
$scope.keyPress_Validatefloat = function (input, type) {
    var strString = input;
    var strValidChars = "0123456789.";
    var strChar;
    for (i = 0; i < strString.length; i++) {
        strChar = strString.charAt(i);
        if (strValidChars.indexOf(strChar) == -1) {
            input.value = input.value.substring(0, i);
            return false;
        }
    }
}
}]);

campapp.$inject = ['$scope', '$filter'];