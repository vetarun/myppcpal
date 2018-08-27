app.controller("dashboardModal", ['$scope', '$filter', '$window', 'dashboardService', function ($scope, $filter, $window, dashboardService) {

	//Pagination
	$scope.gap = 4;
	$scope.Custom = 0;
	$scope.Range = 60;
	$scope.Type = 0; /* 0-Camp, 1-Keywords */
	$scope.cid = 0;
	$scope.cname = "";
	$scope.FilterArg = 0;
	$scope.sort = {
		sortingOrder: 'ACoS',
		reverse: false
	};
	$scope.DataFrom = '';

	$scope.filterParams = {
		nameSearch: ''
	};

	$scope.pagedItems1 = {
		records: [],
		currentPage: 0,
		recordsTotal: 0,
		lastpage: 0
	};

	$scope.pagedItems2 = {
		records: []
	};

	$scope.Options = {
		start: 0,
		length: 5,
		columnname: $scope.sort.sortingOrder,
		direction: $scope.sort.reverse,
        searchname: '',
        IsIgnoreZero:true
	};

	$window.onload = function () {
		$scope.LoadDashboard(0);
	}

	$scope.LoadDashboard = function (type) {
		$scope.Type = type;
		$scope.SetDefault('ACoS', false, 5, 60);
		$scope.GetDashboardData();
	}
	
	$scope.getKeyLog = function (cid, cname, FilterArg) {
		if ($scope.accessformula != 0) {
			$scope.modalspinner = true;
			$scope.cname = cname;
			$scope.cid = cid;
			$scope.Options.length = 10;

			var KeylogData = dashboardService.GetKeyLog($scope.Options, cid, FilterArg);
			KeylogData.then(function (res) {
				var clog = res.data.data;
				if (clog.length > 0) {
					$scope.pagedItems1.records = clog;
					$scope.pagedItems1.recordsTotal = res.data.recordsTotal;
					$scope.pagedItems1.lastpage = Math.ceil(res.data.recordsTotal / $scope.Options.length) - 1;
					$scope.Type = 3;
				}
				else {
					$scope.pagedItems1.records = null;
					$scope.pagedItems1.recordsTotal = null;
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

	
	/*Dashboard data */
	$scope.GetDashboardData = function () {
		var dashbdData;
	
		$scope._tabspinner = true;
		if ($scope.Type < 3) {
			if ($scope.Type == 0) {
				dashbdData = dashboardService.getCampaignsTest($scope.Options, $scope.Range)

			} else if ($scope.Type == 1) {

				dashbdData = dashboardService.getKeywordsTest($scope.Options, $scope.Range)

			} else {
				dashbdData = dashboardService.getAdGroupsTest($scope.Options, $scope.Range)
			}
			dashbdData.then(function (res) {
				var data = res.data.data;

				if (data.length > 0) {
					$scope.pagedItems2.records = data;
					$scope.showmsg = false;
				}
				else {
					$scope.pagedItems2.records = null;
					$scope.showmsg = true;
				}
				$scope._tabspinner = false;
			}, function (xhr) {
				console.log(xhr.error);
				$scope._tabspinner = false;
			});
		}
		else {
			$scope.SetDefault('CampaignName', false, 5, 60);
			$scope.CampaignOptimization($scope.Options, $scope.Range, 0, 0, "");
		}
	}
	$scope.CampaignOptimization = function (options, Range, type, id, KeyName) {
		

		dashbdData = dashboardService.getOptimization(options, Range, type, id, KeyName)
		dashbdData.then(function (res) {
			var data = res.data.data;
			if (data.length > 0) {
				$scope.pagedItems2.records = data;
				$scope.showmsg = false;
			}
			else {
				$scope.pagedItems2.records = null;
				$scope.showmsg = true;
			}
			$scope._tabspinner = false;
		}, function (xhr) {
			console.log(xhr.error);
			$scope._tabspinner = false;
		});
	}
	/*Modal loading */
	$scope.LoadModal = function (type) {
		$scope.SetDefault('ACoS', false, 10, 60, '');
		$scope.Type = type;
		$scope.GetModalData();
		var date = ConvertToDate(document.getElementById('hdnstartdate').value);
		$scope.signUpDate = new Date(date);
		$scope.signUpDate.setDate($scope.signUpDate.getDate() - 59);
	}

	/*Modal data by range selection */
	$scope.GetDataByRange = function (range) {
	
		$scope.SetDefault('ACoS', false, 10, range);
		$scope.showdata = false;
		$scope.showmsg1 = false;
		$scope.pagedItems1 = {
			records: [],
			currentPage: 0,
			recordsTotal: 0,
			lastpage: 0
		}
		if (range != 1) {
			$("#divcustomkey").removeClass("showdivmsg");
			$("#divcustomcamp").removeClass("showdivmsg");
			$("#divcustomad").removeClass("showdivmsg");
			$scope.Custom = 0;
			$scope.c_startdate = null;
			$scope.c_enddate = null;
			$scope.k_startdate = null;
			$scope.k_enddate = null;
			$scope.ClearCustom();
			if (range == 7 || range == 90) {
				if (range == "90") {
					var today = new Date();
					var showDataOn = new Date($scope.signUpDate.getFullYear(), $scope.signUpDate.getMonth(), $scope.signUpDate.getDate() + 90);
					if (today <= showDataOn) {
						$scope.ShowDataOn = 'Data is not avialable. Please come back on ' + showDataOn.toDateString();
						$scope.showdata = true;
					}
					else {
						var date = ConvertToDate(document.getElementById('hdnstartdate').value);
						$scope.ShowDataFrom = new Date(date);
						$scope.ShowDataFrom.setDate($scope.ShowDataFrom.getDate() - 89);
						$scope.DataFrom = range == 0 ? '(Data is from: ' + $scope.ShowDataFrom.toDateString() + ')' : '';
						$scope.GetModalData();
					}
				} else {
					$scope.GetModalData();
				}
			} else {
				var date = ConvertToDate(document.getElementById('hdnstartdate').value);
				$scope.ShowDataFrom = new Date(date);
				$scope.ShowDataFrom.setDate($scope.ShowDataFrom.getDate() - 59);
				$scope.DataFrom = range == 0 ? '(Data is from: ' + $scope.ShowDataFrom.toDateString() + ')' : '';
				$scope.GetModalData();
			}
		} else {
			$scope.DataFrom = '';
			if ($scope.Type == 0) {
				$scope.Custom = 1;
				$("#divcustomcamp").addClass("showdivmsg");
			}
			else if ($scope.Type == 1) {
				$scope.Custom = 1;
				$("#divcustomkey").addClass("showdivmsg");
			} else {
				$scope.Custom = 1;
				$("#divcustomad").addClass("showdivmsg");
			}
		}
	}

	/*Modal data by page size selection */
	$scope.GetDataBySize = function () {
		var size = $scope.Options.length;
		var range = $scope.Range;
		var order = $scope.sort.sortingOrder;
		var dir = $scope.sort.direction;
		var search = $scope.filterParams.nameSearch;
		$scope.SetDefault(order, dir, size, range, search);
		$scope.GetModalData();
	}

	/* Get Model data */
	$scope.GetModalData = function () {
		var CmpData;

		$scope.modalspinner = true;
		if ($scope.Custom == 1) {
			if ($scope.Type == 0) {
				CmpData = dashboardService.getCustomCampTest($scope.Options, $scope.c_startdate, $scope.c_enddate)
			} else if ($scope.Type == 1) {
				CmpData = dashboardService.getCustomKeyTest($scope.Options, $scope.k_startdate, $scope.k_enddate)
			} else {
				CmpData = dashboardService.getCustomAdTest($scope.Options, $scope.a_startdate, $scope.a_enddate)
			}

		}
		else {
			if ($scope.Type == 0) {
				CmpData = dashboardService.getCampaignsTest($scope.Options, $scope.Range)
			} else if ($scope.Type == 1) {
				CmpData = dashboardService.getKeywordsTest($scope.Options, $scope.Range)
			} else if ($scope.Type == 2) {
				CmpData = dashboardService.getKeywordsTest($scope.Options, $scope.Range)
			}
			else {
				

				CmpData = dashboardService.GetKeyLog($scope.Options, $scope.cid, $scope.FilterArg);
				
			}

		}
		CmpData.then(function (res) {
			var data = res.data.data;
			if (data.length > 0) {
		
				$scope.pagedItems1.records = res.data.data;
				$scope.pagedItems1.recordsTotal = res.data.recordsTotal;
				$scope.FilterArg = 0;
				$scope.pagedItems1.lastpage = Math.ceil(res.data.recordsTotal / $scope.Options.length) - 1;
				$scope.showmsg1 = false;
				$scope.modalspinner = false;
			}
			else {
				$scope.pagedItems1.records = null;
				$scope.pagedItems1.recordsTotal = null;
				$scope.showmsg1 = true;
				$scope.modalspinner = false;
			}
		}, function () {
			msg = "Error!";
			alert(msg);
			$scope.modalspinner = false;
		});
	}

	$scope.LoadCustomData = function () {
		$scope.SetDefault('ACoS', false, 10, 1);
		$scope.GetCustomData();
	}

	$scope.GetCustomData = function () {
		var CmpData, sdate, edate;
		var isValid = false;
		if ($scope.Type == 0) {
			sdate = $scope.c_startdate;
			edate = $scope.c_enddate;
			isValid = $scope.ValidateCampaign(sdate, edate);
			isValid ? CmpData = dashboardService.getCustomCampTest($scope.Options, sdate, edate) : $('#divcamperror').show();
		} else if ($scope.Type == 1) {
			sdate = $scope.k_startdate;
			edate = $scope.k_enddate;
			isValid = $scope.ValidateKeyword(sdate, edate);
			isValid ? CmpData = dashboardService.getCustomKeyTest($scope.Options, sdate, edate) : $('#divkeyerror').show();
		} else {
			sdate = $scope.a_startdate;
			edate = $scope.a_enddate;
			isValid = $scope.ValidateAdgroup(sdate, edate);
			isValid ? CmpData = dashboardService.getCustomAdTest($scope.Options, sdate, edate) : $('#divaderror').show();
		}

		if (isValid) {
			$scope.modalspinner = true;
			$scope.ClearCustom();
			CmpData.then(function (res) {
				var data = res.data.data;
				if (data.length > 0) {
					$scope.pagedItems1.records = res.data.data;
					$scope.pagedItems1.recordsTotal = res.data.recordsTotal;
					$scope.pagedItems1.lastpage = Math.floor(res.data.recordsTotal / $scope.Options.length) - 1;
					$scope.showmsg1 = false;
					$scope.modalspinner = false;
				}
				else {
					$scope.pagedItems1 = {
						records: [],
						currentPage: 0,
						recordsTotal: 0,
						lastpage: 0
					}
					$scope.showmsg1 = true;
					$scope.modalspinner = false;
				}
			}, function (xhr) {
				console.log(xhr.error);
				$scope.modalspinner = false;
			});
		}
	}

	//Campaigns custom validation
	$scope.ValidateCampaign = function (startDate, endDate) {
		var _startDate = new Date(startDate);
		var _endDate = new Date(endDate);

		var eDate = new Date();
		eDate.setDate(eDate.getDate() - 3);

		var isValid = true;
		if (startDate == null || startDate == "") {
			$('#divcamperror').html("FromDate must not be empty!");
			$('#txtcampfrom').addClass('input-validation-error');
			isValid = false;
		} else if (endDate == null || endDate == "") {
			$('#divcamperror').html("ToDate must not be empty!");
			$('#txtcampto').addClass('input-validation-error');
			isValid = false;
		} else if (_startDate < $scope.signUpDate) {
			$('#divcamperror').html("FromDate should not be less than " + $scope.signUpDate.toDateString());
			$('#txtcampfrom').addClass('input-validation-error');
			isValid = false;
		}
		else if (_endDate < _startDate) {
			$('#divcamperror').html("FromDate should not be greater than ToDate!");
			$('#txtcampfrom').addClass('input-validation-error');
			isValid = false;
		} else if (eDate < _endDate) {
			$('#divcamperror').html("ToDate should not be greater than " + eDate.toDateString());
			$('#txtcampto').addClass('input-validation-error');
			isValid = false;
		}
		return isValid;
	}

	//Keywordss custom validation
	$scope.ValidateKeyword = function (startDate, endDate) {
		var eDate = new Date();
		eDate.setDate(eDate.getDate() - 2);
		var isValid = true;
		if (startDate == null || endDate == "") {
			$('#divkeyerror').html("FromDate must not be empty!");
			$('#txtkeyfrom').addClass('input-validation-error');
			isValid = false;
		} else if (endDate == null || endDate == "") {
			$('#divkeyerror').html("ToDate must not be empty!");
			$('#txtkeyto').addClass('input-validation-error');
			isValid = false;
		} else if (startDate < getNewDate($scope.signUpDate)) {
			$('#divkeyerror').html("FromDate should not be less than " + $scope.signUpDate.toDateString());
			$('#txtkeyfrom').addClass('input-validation-error');
			isValid = false;
		} else if (endDate < startDate) {
			$('#divkeyerror').html("FromDate should not be greater than ToDate!");
			$('#txtkeyfrom').addClass('input-validation-error');
			isValid = false;
		} else if (getNewDate(eDate) < endDate) {
			$('#divkeyerror').html("ToDate should not be greater than " + eDate.toDateString());
			$('#txtkeyto').addClass('input-validation-error');
			isValid = false;
		}
		return isValid;
	}

	//AdGroups custom validation
	$scope.ValidateAdgroup = function (startDate, endDate) {
		var eDate = new Date();
		eDate.setDate(eDate.getDate() - 2);
		var isValid = true;
		if (startDate == null || endDate == "") {
			$('#divaderror').html("FromDate must not be empty!");
			$('#txtadfrom').addClass('input-validation-error');
			isValid = false;
		} else if (endDate == null || endDate == "") {
			$('#divaderror').html("ToDate must not be empty!");
			$('#txtadto').addClass('input-validation-error');
			isValid = false;
		} else if (startDate < getNewDate($scope.signUpDate)) {
			$('#divaderror').html("FromDate should not be less than " + $scope.signUpDate.toDateString());
			$('#txtadfrom').addClass('input-validation-error');
			isValid = false;
		} else if (endDate < startDate) {
			$('#divaderror').html("FromDate should not be greater than ToDate!");
			$('#txtadfrom').addClass('input-validation-error');
			isValid = false;
		} else if (getNewDate(eDate) < endDate) {
			$('#divaderror').html("ToDate should not be greater than " + eDate.toDateString());
			$('#txtadto').addClass('input-validation-error');
			isValid = false;
		}
		return isValid;
	}

	// Reset prop
	$scope.ClearData = function () {
		$("#divcustomkey").removeClass("showdivmsg");
		$("#divcustomcamp").removeClass("showdivmsg");
		$("#divcustomad").removeClass("showdivmsg");
		$scope.c_startdate = null;
		$scope.c_enddate = null;
		$scope.k_startdate = null;
		$scope.k_enddate = null;
		$scope.a_startdate = null;
		$scope.a_enddate = null;
		$scope.ClearCustom();
		$scope.Custom = 0;
		$scope.showdata = false;
		$scope.SetDefault('ACoS', false, 5, 60);
		$scope.pagedItems1 = {
			records: [],
			currentPage: 0,
			recordsTotal: 0,
			lastpage: 0
		}
		$scope.pagedItems2 = {
			records: []
		}
	}

	// Clear Custom div
	$scope.ClearCustom = function () {
		$('#divcamperror').html("");
		$('#divcamperror').hide();
		$('#txtcampfrom').removeClass('input-validation-error');
		$('#txtcampto').removeClass('input-validation-error');
		$('#divkeyerror').html("");
		$('#divkeyerror').hide();
		$('#txtkeyfrom').removeClass('input-validation-error');
		$('#txtkeyto').removeClass('input-validation-error');
		$('#divaderror').html("");
		$('#divaderror').hide();
		$('#txtadfrom').removeClass('input-validation-error');
		$('#txtadto').removeClass('input-validation-error');
		$scope.ShowDataOn = '';
		$scope.DataFrom = '';
	}

	//Loader
	$scope.unloadLoader = function () {
		$("#divProgress").removeClass("showdivmsg");
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
			$scope.GetModalData();
		}
	};

	$scope.nextPage = function () {
		if ($scope.pagedItems1.currentPage < $scope.pagedItems1.lastpage) {
			$scope.pagedItems1.currentPage++;
			$scope.Options.start = parseInt($scope.Options.start) + parseInt($scope.Options.length);
			$scope.GetModalData();
		}
	};

	$scope.setPage = function () {
		var pageno = this.n;
		if ($scope.pagedItems1.currentPage != pageno) {
			$scope.pagedItems1.currentPage = pageno;
			$scope.Options.start = pageno * $scope.Options.length;
			$scope.GetModalData();
		}

	};
	
	$scope.filter_by = function () {
		$scope.FilterArg = 1;
		$scope.pagedItems1.currentPage = 0;
		$scope.Options.searchname = $scope.filterParams.nameSearch;
		$scope.Options.start = 0;
		$scope.Range != 1 ? $scope.GetModalData() : $scope.GetCustomData();
		
	};


	// change sorting order on model
    $scope.sort_by = function (newSortingOrder) {
        debugger;
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
            searchname: $scope.filterParams.nameSearch,
            IsIgnoreZero: $('#isIgnoreZero').is(':checked')
		};
		if ($scope.pagedItems1.records.length > 0) {
			$scope.Range != 1 ? $scope.GetModalData() : $scope.GetCustomData();
		}
	};

	$scope.dbsortOptimize = function (newSortingOrder) {

		if ($scope.sort.sortingOrder == newSortingOrder)
			$scope.sort.reverse = !$scope.sort.reverse;
		$scope.sort.sortingOrder = newSortingOrder;
		$scope.Options = {
			start: 0,
			length: 5,
			columnname: newSortingOrder,
			direction: $scope.sort.reverse
		};
		$scope.SortOptimizedData();
	};
	$scope.SortOptimizedData = function () {
		
		$scope.CampaignOptimization($scope.Options, $scope.Range, 0, 0, "");


	}
	// change sorting order on dashboard
	$scope.dbsort_by = function (newSortingOrder) {
        debugger;

		if ($scope.sort.sortingOrder == newSortingOrder)
			$scope.sort.reverse = !$scope.sort.reverse;
		$scope.sort.sortingOrder = newSortingOrder;
		$scope.Options = {
			start: 0,
			length: 5,
			columnname: newSortingOrder,
            direction: $scope.sort.reverse,
            IsIgnoreZero:$('#isIgnoreZero').is(':checked')
		};
		$scope.GetDashboardData();
	};

	$scope.selectedCls = function (column) {
	
		if (column == $scope.sort.sortingOrder)
			return ((($scope.sort.reverse) ? 'header headerSortUp' : 'header headerSortDown'));
		else
			return 'header';
	};

	//Set Modal sorting default values
	$scope.SetDefault = function (name, rev, len, range, search) {
        debugger;
		$scope.sort.sortingOrder = name;
		$scope.sort.reverse = rev;
		$scope.Range = range;
		$scope.filterParams.nameSearch = search;
		$scope.Options = {
			start: 0,
			length: len,
			columnname: $scope.sort.sortingOrder,
			direction: $scope.sort.reverse,
            searchname: $scope.filterParams.nameSearch,
            IsIgnoreZero: $('#isIgnoreZero').is(':checked')
		};
		$scope.pagedItems1.currentPage = 0;
	}

	$scope.Close = function CloseModal() {
		
		$scope.ClearData();

		$scope.GetDashboardData();
		$('.modal').modal('hide');
	}
}])

app.$inject = ['$scope', '$filter'];