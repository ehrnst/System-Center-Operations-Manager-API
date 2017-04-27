# System Center Operations Manager Web API

Bringing SCOM in to the 21. century with a basic Web API
I have released my source code and included a 'web deploy' package which quickly letyou deploy and use the API


### API endpoints included

 * /API/Alerts [Get]
 	* Gets all open alerts.
	* Additional parameters: ComputerName, Id, IncDeleted
 * /API/Alerts [Put]
 	* Update existing alert. Change resolution state etc.
 		* Monitor generated alerts will reset if resolution state is set to 255 (closed)
	* Required parameters, ResolutionState, Id
	* Additional parameters, TicketId
	
 * /API/WindowsComputers
 	* Gets all Windows Computer objects
 	* Additional parameters: ComputerName
 
 * /API/ComputerMaintenance [Post]
 	* Puts a single windows computer object in to maintenance mode
 	* Required body parameters: Name, Minutes, Comment
 	* Limitations: Hard coded reason for maintenance, does not utilize SCOM 2016 scheduling feature

 * /API/MonitoringObject
 	* Get information about a specific monitoring object
 	* Required parameters, Id

#### Swagger

The api is swagger enabled, but not all functions are documented. (Multiple get parameters)

### Installation

* Download and extract WebPackage.zip
	* Copy SCOM .dll files to the /bin folder where your extracted the files
	* Create a new web site and point this to the physical path of the extracted files
	* Change the ScomSdkServer property under application settings which is used for connection. If you deployed to a SCOM management server, localhost is fine

### Limitations

* SCOM RBAC is NOT used. This means that authentication against the SCOM SDK will be from the application pool/server
* Microsoft SCOM .dll must be manually copied from your management server to the bin folder.
