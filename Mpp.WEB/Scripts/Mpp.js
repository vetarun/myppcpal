//validate Email
function validateEmail(strInput) {
    var pattern = /^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/;
    if (!pattern.test(strInput)) {
        return false;
    }
    return true;
}

//validate Pwd
function validatePassword(fldObj) {
    var IsValid = true;
    var strInput = fldObj.value;
    var re = /^(?=.*\d).{4,8}$/;
    for (i = 0; i < strInput.length; i++) {
        if (strInput.charCodeAt(i) == 60 || strInput.charCodeAt(i) == 62) {
            alert("Symbols like '>' and '<'  are not allowed in password field");
            fldObj.value = "";
            IsValid = false;
        }
    }
    return IsValid;
}

function keyPress_Validateinteger(text) {
    var strString = text.value
    var strValidChars = "0123456789";
    var strChar;
    for (i = 0; i < strString.length; i++) {
        strChar = strString.charAt(i);
        if (strValidChars.indexOf(strChar) == -1) {
            text.value = text.value.substring(0, i);
            return false;
        }
    }
}

//validate numbers
function keyPress_Validatefloat(text) {
    
    var strString = text.value
    var strValidChars = "0123456789.";
    var strChar;
    for (i = 0; i < strString.length; i++) {
        strChar = strString.charAt(i);
        if (strValidChars.indexOf(strChar) == -1) {
            text.value = text.value.substring(0, i);
            $(event.currentTarget).removeClass('ng-valid-parse ng-valid ng-valid-required');
            $(event.currentTarget).addClass('ng-invalid ng-invalid-required');
            return false;
        }
        else {
            $(event.currentTarget).removeClass('ng-invalid ng-invalid-required');
            $(event.currentTarget).addClass('ng-valid-parse ng-valid ng-valid-required');
        }
    }
   
}

function checkpassword(fldObj) {
    var strInput = fldObj.value;
    var errormsg = "";
    if (strInput.match('^(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).*$')) {
        errormsg = "";
    }
    else {
        errormsg = ("Password must contain at least one letter, one number and one special character(#?!@$%^&*-)");
    }
    if (strInput.length < 6) {
        errormsg = ("The Password must be at least 6 characters long.");
    }
    if (/\s/.test(strInput)) {
        errormsg = ("Spaces are not allowed in password.");
    }
    return errormsg;
}

//Date format (MM/DD/YYY)
function getNewDate(date) {
    var today = new Date();
    var dd = date.getDate();
    var mm = date.getMonth() + 1; //January is 0!
    var yyyy = date.getFullYear();

    if (dd < 10) {
        dd = '0' + dd
    }

    if (mm < 10) {
        mm = '0' + mm
    }

    return today = mm + '/' + dd + '/' + yyyy;
}

//Date format (DD Month YYYY)
function getMonthDate(date) {
    var today = new Date();
    var dd = date.getDate();
    var mm = date.getMonth();//January is 0!
    var monthNames = ['Jan', 'Feb', 'March', 'April', 'May', 'June', 'July', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    var yyyy = date.getFullYear();

    if (dd < 10) {
        dd = '0' + dd
    }

    return today = monthNames[mm] + ',\xa0' + dd + '\xa0' +yyyy;
}

//Add days
Date.prototype.addDays = function (days) {
    var dat = new Date(this.valueOf())
    dat.setDate(dat.getDate() + days);
    return dat;
}

//Add months
Date.prototype.addMonths = function (months) {
    return new Date(this.setMonth(this.getMonth() + months));
}

////Get week no of the year
//Date.prototype.getWeek = function () {
//    var onejan = new Date(this.getFullYear(), 0, 1);
//    return Math.ceil((((this - onejan) / 86400000) + onejan.getDay() + 1) / 7);
//}

//var weekNumber = (new Date()).getWeek();

//var dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
//var now = new Date();
//document.write(dayNames[now.getDay()] + " (" + weekNumber + ").");

function setInitialColor(alpha) {
    var res;
    var alphabet = "abcdefghijklmnopqrstuvwxyz".split("");
    // Defining Colors
    var colors = ["#1abc9c", "#16a085", "#f1c40f", "#f39c12", "#2ecc71", "#27ae60", "#e67e22", "#d35400", "#3498db", "#2980b9", "#e74c3c", "#c0392b", "#9b59b6", "#8e44ad", "#bdc3c7", "#34495e", "#2c3e50", "#95a5a6", "#7f8c8d", "#ec87bf", "#d870ad", "#f69785", "#9ba37e", "#b49255", "#b49255", "#a94136"];
    for (i = 0; i < alphabet.length; i++) {
        if (alphabet[i] == alpha.toLowerCase()) {
            res = colors[i];
        }
    }
    return res;
}

function disableClick(id, type) {
    $(id).css('display', 'block');
    document.getElementById("tab_dashboard").childNodes[0].onclick = function () { $(id).show(); return false; };
    document.getElementById("tab_camp").childNodes[0].onclick = function () { $(id).css('display', 'block'); return false; };
    document.getElementById("tab_report").childNodes[0].onclick = function () { $(id).css('display', 'block'); return false; };
    document.getElementById("tab_sett").childNodes[0].onclick = function () { $(id).css('display', 'block'); return false; };
    document.getElementById("tab_help").childNodes[0].onclick = function () { $(id).css('display', 'block'); return false; };
    document.getElementById("tab_notify").onclick = function () { $(id).css('display', 'block'); return false; };
    document.getElementById("tab_profile").onclick = function () { $(id).css('display', 'block'); return false; };
    if (type == 'settings') {
        document.getElementById("tab_billing").childNodes[1].onclick = function () { $(id).css('display', 'block'); return false; };
        document.getElementById("tab_sprofile").childNodes[1].onclick = function () { $(id).css('display', 'block'); return false; };
        document.getElementById("tab_account").childNodes[1].onclick = function () { $(id).css('display', 'block'); return false; };
        document.getElementById("tab_amazon").childNodes[0].onclick = function () { $(id).css('display', 'block'); return false; };
    }
}

function NavigateToCard() {
    location.href = "../Settings/Cards";
}

function NavigateToPlan() {
    location.href = "../Settings/Plan";
}

//Used to convert YYYY-MM-DD format
function ConvertToDate(UDate) {
    var ConvertedDate, CheckSlash, nextItemStart, digit;
    var SeprationMonth, SeprationYear, SeprationDay;

    SeprationYear = UDate.substring(0, 4);
    CheckSlash = UDate.substring(6, 7);
    if ((CheckSlash == "/") || (CheckSlash == "-")) {
        SeprationMonth = UDate.substring(5, 6);
        nextItemStart = 7;
        digit = 1;
    }
    else {
        SeprationMonth = UDate.substring(5, 7);
        nextItemStart = 8;
        digit = 2;
    }
    SeprationDay = UDate.substring(nextItemStart, nextItemStart + digit);
    ConvertedDate = SeprationMonth + '/' + SeprationDay + '/' + SeprationYear;
    return ConvertedDate;
}