/*
Error Handler Object
*/
var Incident = function (title, pageName, userSessionGuid, currentDOM, notes, clientData, originalErrorMessage) {
    this.Title = title;
    this.PageName = pageName;
    this.UserSessionGuid = userSessionGuid;
    this.CurrentDOM = currentDOM;
    this.Notes = notes;
    this.ClientData = clientData;
    this.OriginalErrorMessage = originalErrorMessage;
}

/*
    Domain Solution
*/
var Employee = function (id, firstName, lastName, departmentId, startDate) {
    this.Id = ko.observable(id);
    this.FirstName = ko.observable(firstName);
    this.LastName = ko.observable(lastName);
    this.DepartmentId = ko.observable(departmentId);
    this.StartDate = ko.observable(startDate);
}

var ViewModel = function () {
    var self = this;

    self.selectedEmployee = ko.observable();
    self.employees = ko.observableArray([]);

    self.getData = function () {
        $.ajax({
            url: "Services/APoorWebService.asmx/GetAllEmployees",
            async: true,
            type: "POST",
            data: ko.toJSON(data),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(XMLHttpRequest.status);
                alert(XMLHttpRequest.responseText);
            },
            success: function (msg) {
                var serverEmployees = ko.utils.parseJson(msg.d);
                self.employees.removeAll();

                self.employees = ko.utils.arrayMap(serverEmployees, function (emp) {
                    return new Employee(emp.Id, emp.FirstName, emp.LastName, emp.DepartmentId, emp.StartDate);
                });
            }
        });
    }

    self.newEmployee = function () {
        self.selectedEmployee(new Employee());

        $("#newEmployee").show();
        $("#newEmployeeFirstName").focus();
    }

    self.createEmployee = function () {
        $.ajax({
            url: "Services/APoorWebService.asmx/CreateEmployee",
            async: true,
            type: "POST",
            //  Generate an error by sending no data.  Throws on server
            data: "{}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(XMLHttpRequest.status);
                alert(XMLHttpRequest.responseText);
            },
            success: function (msg) {
                alert("Save was successful");
            }
        });
    }


}

/***
Taken from A List Apart: http://www.alistapart.com/articles/modern-debugging-tips-and-tricks/
***/
function sendError() {
    var o, xhr, data, msg = {}, argtype = typeof (arguments[0]);

    // if it is an error object, just use it.
    if (argtype === 'object') {
        msg = arguments[0];
    }

    // if it is a string, check whether we have 3 arguments...
    else if (argtype === 'string') {
        // if we have 3 arguments, assume this is an onerror event.
        if (arguments.length == 3) {
            msg.message = arguments[0];
            msg.fileName = arguments[1];
            msg.lineNumber = arguments[2];
        }
        // otherwise, post the first argument
        else {
            msg.message = arguments[0];
        }
    }

    // include the user agent
    msg.userAgent = navigator.userAgent;

    // Client side data.  In this case, include Employees from server, current employee
    var clientData = {};
    clientData.selectedEmployee = vm.selectedEmployee;
    clientData.employees = vm.employee;

    // Create incident for logging
    var incident = new Incident("", document.URL, userSessionGuid, "{body: " + document.body.innerHTML +
                    "}", "", ko.toJSON(clientData), ko.toJSON(msg));

    //    Parse Title from message, remove 'Error: '
    var endTitlePos = msg.message.indexOf(".");
    incident.Title = msg.message.substr(7, endTitlePos - 7);

    // convert to JSON string
    data = { "jsonError": ko.toJSON(incident) };

    //  If jQuery can't load we are in worse shape than what we think.
    $.ajax({
        url: "Services/Reducio.asmx/LogError",
        async: true,
        type: "POST",
        data: ko.toJSON(data),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.status);
            alert(XMLHttpRequest.responseText);
        },
        success: function (msg) {
        }
    });

    // To hide error message from user in supporting browsers return true;
    return false;
}

window.onerror = sendError;

var vm = new ViewModel();
vm.getData();

ko.applyBindings(vm);
