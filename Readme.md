# ToDoAPI_NG (ToDo List API Next Generation)

**Author**: Joshua Taylor
**Version**: 1.0.0

## Overview

ToDoAPI_NG is a REST API that allows users to create, read, update, and
delete (CRUD) both ToDo items and lists of ToDo items. These lists and
items can be retrieved via HTTP GET. Adding an id parameter will retrieve
a specific list or item along with the entities that they are referencing.
ToDo items can be assigned to a single ToDo list, and deleting ToDo lists
will cascade to every ToDo item referencing those lists.

## Getting Started

ToDoAPI_NG targets the .NET Core 2.0 platform, ASP.NET Core, Entity
Framework Core and the MVC Framework. The .NET Core 2.0 SDK can be downloaded 
from the following URL for Windows, Linux, and macOS:

https://www.microsoft.com/net/download/

Additionally, the Entity Framework tools will need to be installed via the
NuGet Package Manager Console in order to create a migration for the local,
development database (run from the solution root):

    Install-Package Microsoft.EntityFrameworkCore.Tools
	Add-Migration Initial
	Update-Database

The dotnet CLI utility would then be used to build and run the application:

    cd TodoAPI_Ng
    dotnet build
    dotnet run

The _dotnet run_ command will start an HTTP server on localhost using Kestrel
which can be accessed by HTTP requests to the defined API endpoints.

Additionally, users can build and run ToDoAPI_NG using Visual Studio
2017 or greater by opening the solution file at the root of this repository.
All dependencies are referenced via NuGet and should be brought in during
the restore process. If this does not occur, the following will download all
needed dependencies (other than the Entity Framework tools):

    dotnet restore

## Data Model

To-do list items and lists are represented in state transfers in JSON format.
Relationships between ToDo items and ToDoList's are represented solely
through the listId field in the ToDo item JSON objects.

### ToDo

Represents a single ToDo item along with a reference to a single ToDo list
of which that item is a member.

#### id (int)

Primary key for each to-do list item.

#### message (string)

A JSON string containing the message of each to-do list item.

#### isDone (bool)

A boolean JSON value representing whether the task has been completed.

#### listId (int)

The ID of the list of which this item is a member

#### list (ToDoList)

JSON representation of the ToDoList of which this item is a member

### ToDoList

Represents a single ToDo list. 

#### id (int)

Primary key for each to-do list.

#### name (string)

A JSON string containing the name of each to-do list.

## End-Points

### ToDo Items

#### GET /api/ToDo

Retrieves all to-do list items stored within the backend database in JSON
format as an array. Does not resolve the list object in the resultant
array items. The list can be retrieved either by sending a GET request with
a specific ToDo item's ID in the routing or by sending a GET request to the
/api/ToDoList/{id:int} endpoint with a particular item's listId included in
the routing.

#### GET /api/ToDo/{id:int}

Retrieves the specified to-do list item by its numeric id primary key. A
successful response can be identified with code 200 with the requested item
in the response body in JSON format.

#### POST /api/ToDo

Adds a new to-do list item using the JSON provided in the request body. Will
return code 201 on successful item creation along with the new item in the
response body in JSON format. Specifying a non-existant listId will result
in a 409 status code.

#### PUT /api/ToDo/{id:int}

Updates the to-do list item with a numeric id primary key matching the id
passed from routing using the JSON representational state provided in the
request body. This method will update __all__ fields of the existing to-do
list item specified. Returns an empty 204 response on success per _RFC 2616 Section 10.2.5_.

#### DELETE /api/ToDo/{id:int}

Deletes the existing to-do list item with the numeric id primary key matching
the id provided in routing. Provides an empty 204 response on success.

### ToDoList

#### GET /api/ToDoList

Retrieves all to-do lists stored within the backend database in JSON
format as an array. Does not resolve the ToDo items in the resultant
array. The items can be retrieved either by sending a GET request with
a specific ToDoList's ID in the routing.

#### GET /api/ToDoList/{id:int}

Retrieves the specified to-do list by its numeric id primary key. A
successful response can be identified with a 200 response code. The returned
JSON object contains two fields. "list" contains the actual ToDoList object
state in JSON representation. "items" is an array of all ToDo entities
associated with this specific list through their listId values.

#### POST /api/ToDoList

Adds a new to-do list using the JSON provided in the request body. Will
return code 201 on successful list creation along with the new item in the
response body in JSON format.

#### PUT /api/ToDo/{id:int}

Updates the to-do list with a numeric id primary key matching the id
passed from routing using the JSON representational state provided in the
request body. This method will update __all__ fields of the existing to-do
list item specified. Returns an empty 204 response on success per _RFC 2616 Section 10.2.5_.

#### DELETE /api/ToDo/{id:int}

Deletes the existing to-do list with the numeric id primary key matching
the id provided in routing. Will cascade to all ToDo items which reference
this list via their listId values. Provides an empty 204 response on success.

## Architecture

### ToDoController

ToDoController provides actions for all of the aforementioned API endpoints
at /api/ToDo. These actions operate as described in the _Endpoints_ section
of this document.

### ToDoListController

ToDoListController provides actions for all of the aforementioned API endpoints
at /api/ToDoList. These actions operate as described in the _Endpoints_ section
of this document.

### Models

All C# code-side classes for the JSON objects mentioned earlier in this
document are located within the Models/ subdirectory of the TodoAPI_Ng
project. C# naming conventions are used for the code-side counterparts
of the models used in this project.

## Change Log

* 4.11.2018 [Joshua Taylor](mailto:taylor.joshua88@gmail.com) - Initial
release. All tests passing.