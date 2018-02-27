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
| [PUT] API/Alerts/{id} | Update the specified alert with common properties and resolution state |
| [GET] API/Alert/Computer/{ComputerName} | Get all alert from specific computer, use IncClosed=true to include open and closed alerts |
| [GET] API/Alert/MonitoringObject/{MonitoringObjectId} | Get all alert from specific monitoring object, use IncClosed=true to include open and closed alerts |

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
| [POST] API/MaintenanceSchedule | Create a new maintenance schedule. Supports multiple object guids in an array. SCOM 2016 ONLY |

### Object

| Route | Description |
| ------ | ------ |
| [GET] API/MonitoringObject/{id} | Get a monitoring object and all child object |
| [GET] API/MonitoringObject/class/{classId} | Get all objects of a class. Limited properties returned. |

### Performance

| Route | Description | Parameters |
| ------ | ------ | ------ |
| [GET] API/Perf/{managedEntityId} | Get RAW performance data from a specific managedEntity and metric | managedEntityId, counterName, startDate, endDate |
| [GET] API/Perf/hourly/{managedEntityId} | Get hourly aggregated performance data from a specific managedEntity and metric | managedEntityId, counterName, startDate, endDate |
| [GET] API/Perf/daily/{managedEntityId} | Get daily aggregated performance data from a specific managedEntity and metric | managedEntityId, counterName, startDate, endDate |

### Installation

Follow the (very limited) guide here https://github.com/ehrnst/System-Center-Operations-Manager-API/wiki/Installation-and-configuration
