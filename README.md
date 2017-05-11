# System Center Operations Manager Web API

Bringing SCOM in to the 21. century with a basic Web API (v0.1 Community Alpha)


### API endpoints included

 * /API/Alerts [Get]
 	* Gets all open alerts.
	* IncClosed = true will return open and closed alerts

 * /API/Alerts/{Id} [Get]
 	* Gets single alert from Guid.
	
 * /API/Alerts/{ComputerName} [Get]
 	* Get all open alerts by computername.
	* IncClosed = true will return open and closed alerts
	
 * /API/Alerts [Put]
 	* Update existing alert. Change resolution state etc.
 		* Monitor generated alerts will reset if resolution state is set to 255 (closed)
	* Required parameters, ResolutionState, Id
	* Additional parameters, TicketId
	
 * /API/Computers/Windows [Get]
 	* Gets all Windows Computer objects
	
 * /API/Computers/Windows/{ComputerName} [Get]
 	* Gets the specific Windows computer

 * /API/Computers/Linux [Get]
 	* Gets all Linux Computer objects

 * /API/Computers/Linux/{ComputerName} [Get]
 	* Gets all Linux Computer objects
 
 * /API/ComputerMaintenance [Post]
 	* Puts a single windows computer object in to maintenance mode
 	* Required body parameters: Name, Minutes, Comment
 	* Limitations: Hard coded reason for maintenance, does not utilize SCOM 2016 scheduling feature
	
 * /API/ObjectMaintenance [Post]
 	* Puts a single monitoring object in to maintenance mode regardless of class
 	* Required body parameters: Name, Minutes, Comment
 	* Limitations: Hard coded reason for maintenance, does not utilize SCOM 2016 scheduling feature

 * /API/MonitoringObject
 	* Get information about a specific monitoring object
 	* Required parameters, Id

#### Swagger

API Documentation can be found by using url/swagger Swagger allows you to test each endpoints directly in the browser.

### Installation

* Download and extract WebPackage.zip
	* Copy SCOM .dll files to the /bin folder where your extracted the files
	* Create a new web site and point this to the physical path of the extracted files
	* Change the ScomSdkServer property under application settings which is used for connection. If you deployed to a SCOM management server, localhost is fine

### Limitations

* User impersonation is enabled in code, but only confirmed to work when API is installed on a management server
* Microsoft SCOM .dll must be manually copied from your management server to the bin folder.
