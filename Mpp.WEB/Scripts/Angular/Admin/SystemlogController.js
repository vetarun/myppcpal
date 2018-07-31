app.controller("SystemlogController", ['$scope', '$filter', '$window', 'settingService', function ($scope, $filter, $window, settingService) {


    $scope.gap = 4;
    $scope.clients = [];
    $scope.filterParam = {};
    $scope.filterParam.Type = 1;
    $scope.PageOptions = {
        Start: 1,
        Length: 10,
        ColumnName: "",
        Direction: true,
        Date: getTodayFormated(),
        Date2: getTodayFormated(),
        Client: 0
    }

    $scope.pagedItems1 = {
        records: [],
        currentPage: 0,
        recordsTotal: 0,
        lastpage: 0
    };
    // GetAllClients();
    $scope
    $scope.logTableData = null;
    $scope.NoOfPages = [];
    $scope.selectedClient = 0;
    $scope.validDate = true;

    $("#srchClient").autocomplete({
        minLength:0 ,
        select: function (event, ui) {
            event.preventDefault();
            $scope.PageOptions.Length = 10;
            $scope.PageOptions.Start = 1;
            $scope.pagedItems1 = {
                records: [],
                currentPage: 0,
                recordsTotal: 0,
                lastpage: 0
            };
            $("#srchClient").val(ui.item.label);
            $scope.PageOptions.Client = ui.item.value;
            $scope.LoadLogData();
        },
        source: function (req, respo) {
            var arr = [];
            settingService.getClients(req.term).then(function (res) {
                if (res.data != null && res.data.length > 0)
                    respo($.map(res.data, function (item) {
                        return  {
                            label: item.Name,
                            value: item.Id
                        };
                         
                }))
            })   
        },

    }).bind('focus', function () {
        $(this).autocomplete("search");
        });
    $scope.selectedCls = function (column) {
        if (column == $scope.PageOptions.ColumnName) {
            return ((($scope.PageOptions.Direction) ? 'headerSort headerSortUp' : 'headerSort headerSortDown'));
        }
        else {
            return 'headerSort';
        }
    };

    
    //function GetAllClients(srchTerm) {
    //   
    //    $scope.adminspinner = true;
    //    var dataList = null;
    //    var clientsRespo = settingService.getClients(srchTerm);
    //    return cli
    //    //clientsRespo.then(function (res) {
    //    //    if (res.data.length > 0) {
    //    //        dataList = res.data;
    //    //    } 
    //    //    return dataList;
    //    //    $scope.adminspinner = false;
    //    //},
    //    //    function (xhr) {
    //    //        $scope.adminspinner = false;
    //    //        console.log(xhr.error);
    //    //        return dataList;
    //    //    });
       
    //}

    $scope.LoadLogData = function () {       
            $scope.adminspinner = true;
           
            var logData = null;
            if (typeof $scope.PageOptions.Length == 'string') {
                $scope.PageOptions.Length = parseInt($scope.PageOptions.Length);
                $scope.PageOptions.Start = 1;
            }
            if ($scope.filterParam.Type == '1') {
                logData = settingService.getReportLog($scope.PageOptions);
            }
            else if ($scope.filterParam.Type == '2') {
                logData = settingService.getEmailLog($scope.PageOptions);
            }
            if (logData != null) {
                logData.then(function (res) {

                    if (res.data != null && res.data.Total > 0) {
                        $scope.logTableData = null;
                        $scope.logTableData = [];
                        $scope.logTableData.Total = 0;
                        $scope.NoOfPages = GetNumberOfPages(res.data.Total, $scope.PageOptions.Length);
                        
                        $scope.logTableData = res.data;
                        $scope.pagedItems1.records = res.data;
                        $scope.pagedItems1.recordsTotal = res.data.Total;
                        $scope.pagedItems1.lastpage = Math.ceil(res.data.Total / $scope.PageOptions.Length) - 1;
                    }
                    else {
                        $scope.logTableData = null;
                    }
                    $scope.adminspinner = false;
                }
                    , function (xhr) {
                        $scope.adminspinner = false;
                        console.log(xhr.error);
                    });
            }
            $scope.adminspinner = false;
      
    }
    $scope.OnDateChangeLoadLogData = function () {
        event.preventDefault();
        $scope.PageOptions.Length = 10;
        $scope.PageOptions.Start = 1;
        $scope.pagedItems1 = {
            records: [],
            currentPage: 0,
            recordsTotal: 0,
            lastpage: 0
        };
        $scope.valaidateDatesFilter();
      
        
    }
    $scope.valaidateDatesFilter = function () {
        if ($scope.filterParam.Type == '1') {
            
            if (Date.parse($scope.PageOptions.Date) <= Date.parse($scope.PageOptions.Date2)) {

                $scope.LoadLogData();
                $('#dateError').css('display', 'none');
            }
            else {
                $('#dateError').css('display', 'block');
            }
        }
        else {
            $('#dateError').css('display', 'none');
        }
    }
   $scope.LoadLogData();
    $scope.sort_by = function (name) {
       
        $scope.PageOptions.ColumnName = name;
        $scope.PageOptions.Direction = $scope.PageOptions.Direction == true ? false : true;
        $scope.LoadLogData();
    }

    $scope.setPage = function (n) {
        $scope.PageOptions.Start = n;
       
    }
    //function changeSortClass(dir, elem) {
    //    if (dir)
    //        $(elem).addClass('headerSortUp')
    //    else
    //        $(elem).addClass('headerSortDown');
    //}

    function GetNumberOfPages(total, length) {
        var arr = [];
        var devide = total / length;
        var pages = Math.ceil(devide);
        for (var i = 1; i <= pages; i++) {
            arr[i - 1] = i;
        }
        return arr;
    }

    function getTodayFormated() {
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1;
        var yyyy = today.getFullYear();
        if (dd < 10) {
            dd = '0' + dd;
        }
        if (mm < 10) {
            mm = '0' + mm;
        }
        today = mm + '/' + dd + '/' + yyyy;
        return today;
    }

    $scope.ResetPageOptions = function () {
        
        $scope.PageOptions = {
            Start: 1,
            Length: 10,
            ColumnName: "",
            Direction: true,
            Date: getTodayFormated(),
            Date2: getTodayFormated(),
            Client: 0
        }
        $("#srchClient").val('');
    }

    $("#srchClient").blur(function () {
        if ($(this).val() === '')
            $scope.PageOptions.Client = 0;
        $scope.LoadLogData();
    });

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
            var size = parseInt($scope.PageOptions.Length);
            $scope.pagedItems1.currentPage--;
            $scope.PageOptions.Start = $scope.pagedItems1.currentPage == 0 ?1:$scope.pagedItems1.currentPage;
            $scope.LoadLogData();
        }
    };

    $scope.nextPage = function () {
        
        if ($scope.pagedItems1.currentPage < $scope.pagedItems1.lastpage) {
            $scope.pagedItems1.currentPage++;
            $scope.PageOptions.Start = $scope.pagedItems1.currentPage;
            $scope.LoadLogData();
        }
    };

    $scope.setPage = function () {
        var pageno = this.n;
        if ($scope.pagedItems1.currentPage != pageno) {
            $scope.pagedItems1.currentPage = pageno;
            $scope.PageOptions.Start = pageno+1;
            $scope.LoadLogData();
        }

    };
}]);