app.controller("AccountCtrl", ['$scope', '$filter', '$window', '$interval', 'adminuserService', function ($scope, $filter, $window, $interval, adminuserService) {

    //var _adminuser = this; // Replace scope with this one later
    /* Scope Properties */
    $scope.ModalData = [];

    //Pagination
    $scope.gap = 5;
    $scope.filteredItems = [];
    $scope.groupedItems = [];
    $scope.pagedItems = [];
    $scope.currentPage = 0;
    $scope.filterParams = {
        nameSearch: ''
    };

    $scope.sort = {
        sortingOrder: 'FirstName',
        reverse: false
    };

    $scope.Options = {
        start: 0,
        length: 5
    };

    GetUsers();

    /* Get Admin Users */
    function GetUsers() {
        $scope.adminspinner = true;
        var UserData = adminuserService.getUsers();
        UserData.then(function (res) {
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

    //Loader
    function unloadLoader() {
        $("#divProgress").removeClass("showdivmsg");
    }

    $scope.submitText = "Invite";
    $scope.submitted = false;
    $scope.message = '';
    $scope.isFormValid = false;
    $scope.User = {
        Type:1,
        FirstName: '',
        LastName: '',
        Email: '',
        Password: ''
    };

    //Check form Validation 
    $scope.$watch('fmuser.$valid', function (newValue) {
        $scope.isFormValid = newValue;
        if ($scope.isFormValid) {
            $scope.message = '';
        }
    });

    //Save Data
    $scope.AddUser = function (data) {
        var msg;
        if ($scope.submitText == "Invite") {
            $scope.submitted = true;
            $scope.message = '';
            if ($scope.isFormValid) {
                $scope.User = data;
                $scope.submitText = 'Please Wait...';
                $scope.adminspinner = true;
                adminuserService.saveUser($scope.User).then(function (d) {
                    if (d.data == '') {
                        msg = "Invitation email has been sent!";
                        displayMsg(msg, 1);
                        $scope.Close();
                        GetUsers();
                    } else {
                        displayMsg(d.data, 2);
                    }
                    $scope.adminspinner = false;
                    $scope.submitText = "Invite";
                }, function (xhr) {
                    console.log(xhr.error);
                    $scope.adminspinner = false;
                    $scope.submitText = "Invite";
                    displayMsg(msg, 2);
                });
            }
            else {
                msg = 'Please fill the required fields';
                displayMsg(msg, 2);
            }
        }
    }

    //Remove User
    $scope.RemoveUser = function (userid) {
        var confrm = false;
        confrm = $window.confirm("Are you sure you want to delete a user? ");
        if (confrm) {
            $scope.adminspinner = true;
            adminuserService.removeUser(userid).then(function (d) {
                if (d.data == '') {
                    GetUsers();
                    displayMsg("Deleted Successfully!", 1);
                } else {
                    displayMsg(d.data, 2);
                }
                $scope.adminspinner = false;
            }, function (xhr) {
                console.log(xhr.error);
                $scope.adminspinner = false;
                displayMsg("Error!", 2);
            });
        }
    }

    //Update Status
    $scope.UpdateStatus = function (userid, status) {
        var confrm = false;
        var newstatus;
        if (status == true) {
            confrm = $window.confirm("Are you sure you don't want to give access? ");
            newstatus = 0;
        }
        else {
            confrm = $window.confirm("Are you sure you want to give access? ");
            newstatus = 1;
        }
        if (confrm) {
            $scope.adminspinner = true;
            adminuserService.updateStatus(userid, newstatus).then(function (d) {
                if (d.data == '') {
                    GetUsers();
                    displayMsg("Updated Successfully!", 1);
                } else {
                    document.getElementById('check' + userid).checked = status;
                    displayMsg(d.data, 2);
                }
                $scope.adminspinner = false;
            }, function (xhr) {
                console.log(xhr.error);
                $scope.adminspinner = false;
                document.getElementById('check' + userid).checked = status;
                displayMsg("Error!", 2);
            });
        } else {
            document.getElementById('check' + userid).checked = status;
        }
    }

    $scope.Close = function CloseModal() {
        $('.modal').modal('hide');
        $scope.ResetForm();
    }

    //Clear Form (reset)
    $scope.ResetForm = function ClearForm() {
        $scope.User = {
            Type: 1,
            FirstName: '',
            LastName: '',
            Email: '',
            Password: ''
        };
        $scope.fmuser.$setPristine(true); //here fmuser our form name
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
            setTimeout(function () { $('.errormsg').fadeOut(); }, 2000);
        }
    }

    $scope.$watchCollection('filterParams', function (newNames, oldNames) {
        $scope.search();
    });

    // init the filtered items
    $scope.search = function () {
        var filtered = [];
        filtered = $filter('filter')($scope.ModalData, function (item) {
            if (searchMatch(item['FirstName'], $scope.filterParams.nameSearch)) {
                return true;
            }
            return false;
        });

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

    var searchMatch = function (haystack, needle) {
        return haystack.toLowerCase().indexOf(needle.toLowerCase()) == 0;
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
        (size, start, end);

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
        (ret);
        return ret;
    };

    $scope.prevPage = function () {
        if ($scope.currentPage > 0) {
            $scope.campspinner = true;
            var size = parseInt($scope.Options.length);
            $scope.currentPage--;
            $scope.Options.start = parseInt($scope.Options.start) - size;
            $interval(unloadLoader, 500);
        }
    };

    $scope.nextPage = function () {
        if ($scope.currentPage < $scope.pagedItems.length - 1) {
            $scope.campspinner = true;
            $scope.currentPage++;
            $scope.Options.start = parseInt($scope.Options.start) + parseInt($scope.Options.length);
            $interval(unloadLoader, 500);
        }
    };

    $scope.setPage = function () {
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

    $scope.GetDataBySize = function () {
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
}])
