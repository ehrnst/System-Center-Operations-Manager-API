# System Center Operations Manager API


Bringing SCOM in to the 21. century with a Restful Web API.

  - Easy integration
  - Common used function as enpoints
  - Swagger Documented

# API Endpoints

### Agents

| Route | Description |
| ------ | ------ |
| [GET] API/Agents | Gets all agents |
| [GET] API/Agents/{id} | Get a single agent |

### Alerts

| Route | Description |
| ------ | ------ |
| [GET] API/Alerts | Gets all open alerts |
| [GET] API/Alert/{id} | Get a single alert |
| [PUT] API/Alert | Update the specified alert with resolution state, TicketId |
| [GET] API/Alert/{ComputerName} | Get all alert from specific computer, use IncClosed=true to include open and closed alerts |

### Computer

| Route | Description |
| ------ | ------ |
| [GET] API/Computer/Windows | Get all windows computers wit basic properties |
| [GET] API/Computer/Windows/{ComputerName} | Get A single windows computers with basic properties |
| [GET] API/Computer/Windows/{ComputerName}/Detailed | Get A single windows computers with hosted child objects |
| ------ | ------ |
| [GET] API/Computer/Linux | Get all Linux computers wit basic properties |
| [GET] API/Computer/Linux/{ComputerName} | Get A single Linux computer with basic properties |
| [GET] API/Computer/Linux/{ComputerName}/Detailed | Get A single Linux computers with hosted child objects |

### Maintenance

| Route | Description |
| ------ | ------ |
| [POST] API/ComputerMaintenance | Put the specific computer object and all child in maintenance mode |
| [POST] API/ObjectMaintenance | Put the specific monitoring object and all child in maintenance mode |
| [POST] API/MaintenanceSchedule | Create a new maintenance schedule. SCOM 2016 ONLY |

### Object

| Route | Description |
| ------ | ------ |
| [GET] API/MonitoringObject/{id} | Get a monitoring object and all child object |


### Installation

- Download the zip and extract to your SCOM management server running IIS or download the whole source to make your own customizations
- Create a new web site and application pool
- Set your application pool to use Network Service
- Enable windows authentication (basic if needed)
- Copy SCOM specific .dll's to the BIN folder where you extracted the .Zip
