Reducio - A web application error logging framework for client and server logging
=================================================================================
###Premise
Reducio will help record Javascript errors that occur during client side testing, and will attempt to:

Classify each error based on message thrown
Record an exception from the client and associate secondary instances of that 
	exception to the primary error description
Assign an identifier to the user session for error corrrelation
Provide a method for mapping client side errors to any exceptions thrown in ther server environment	

Just add this function to you web pages:

```javascript
<script type="text/javascript">
		var userSessionGuid = "<%= UserSessionGuid%>";
	</script>
```
And this function to your .js file
```javascript
function sendError(){
     var o, xhr, data, msg = {}, argtype = typeof( arguments[0] );

     // if it is an error object, just use it.
     if( argtype === 'object' ){
     	  msg = arguments[0];
     }

     // if it is a string, check whether we have 3 arguments...
     else if( argtype === 'string') {
     // if we have 3 arguments, assume this is an onerror event.
          if( arguments.length == 3 ){
              msg.message    = arguments[0];
              msg.fileName   = arguments[1];
              msg.lineNumber = arguments[2];
          }
        // otherwise, post the first argument
          else {
              msg.message    = arguments[0];
          }
      }

      // include the user agent
      msg.userAgent = navigator.userAgent;

      // client data - workflows, form Fields, etc.
      var clientData = {};
      clientData.Workflow = ko.toJSON(vm.selectedWorkflow());
      clientData.FormFields = ko.toJSON(vm.formFields());

    // Create incident for logging
    var incident = new Incident("", document.URL, userSessionGuid, "{body: " + document.body.innerHTML +
                    "}", "", ko.toJSON(clientData), ko.toJSON(msg));

    //    Parse Title from message, remove 'Error: '
    var endTitlePos = msg.message.indexOf(".");
    incident.Title = msg.message.substr(7, endTitlePos - 7);

	// convert to JSON string
	data = {"jsonError" : ko.toJSON(incident) };

     //  If jQuery can't load we are in worse shape than what we think.
	 $.ajax({
	     url: "Services/ErrorLogging.asmx/LogError",
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

     // hide error message from user in supporting browsers
     return false;
 }

 window.onerror = sendError;


```

###Technology
Reducio uses RavenDB, jQuery, KnockoutJS

