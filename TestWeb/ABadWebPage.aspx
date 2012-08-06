<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ABadWebPage.aspx.cs" Inherits="TestWeb.ABadWebPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>A Bad Web Page With Lots of Errors</title>
    <style type="text/css">
        div.dataItemBorder{
			background-color: #F5F5F5;
			border: 1px solid #DDDDDD;
			border-radius: 3px 3px 3px 3px;
			box-shadow: 0 1px 0 #FFFFFF inset;
			list-style: none outside none;
			margin: 0 0 18px;
			min-height: 56px;
			padding: 7px 14px;
		}
    </style>
    <script type="text/javascript">
        var userSessionGuid = "<%= UserSessionGuid%>";
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div data-bind="foreach: employees">
        <div class="dataItemBorder">
            <span data-bind="text: FirstName"></span>
            <span data-bind="text: LastName"></span>
            <span data-bind="text: DepartmentId"></span>
            <span data-bind="text: StartDate"></span>
        </div>
    </div>
    <script type="text/javascript" src="Scripts/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="Scripts/knockout-2.0.0.js"></script>
    <script type="text/javascript" src="Scripts/abadwebpage.js"></script>
    </form>
</body>
</html>
