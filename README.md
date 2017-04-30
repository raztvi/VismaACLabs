# Visma AC Labs - Cloud Storage

![build status](https://emilcraciun.visualstudio.com/_apis/public/build/definitions/f6381654-59b1-4043-bd5f-3c9a9e740d98/1/badge)

This is the technical material for a course for students in Timisoara, Romania. The course is part of a students' organization project called [LigaAC LABS](https://labs.ligaac.ro/).

The purpose of this application is to make an introduction into ASP.NET Core, Microsoft Azure Blob Storage, SignalR, unit testing and the basic usage of version control system (git).

Additional references and resources can be found [here](/docs/References.md). 

Also, if it helps, the course outline can be found [here](/docs/CourseOutline.md).

Below you can read about some specifications and technical details and requirements.

## Functional requirements

### As a registered user I can
- [x] register an account on the application and associate it with an existing company
- [x] see a list of files uploaded by users in my company
- [x] download any file from the above list
- [x] edit the metadata of any file from above
- [ ] ~~edit the contents of any file from above (re-upload)~~
- [x] delete a file from the above list
- [x] upload a new file along with metadata
- [x] be notified in real time when someone from my company uploads a new file
- [x] search for files


### As an admin I can
- [x] see a list of all available companies
- [x] create a new company
- [x] edit an existing company
- [x] see all uploaded files
- [x] edit any file or metadata
- [x] delete any file 
- [x] upload a file

## Other requirements

### As a developer I will
- [x] use azure storage
- [x] use sql local db
- [x] use SignalR
- [x] write some unit tests
- [x] use logging


### As a respectable trainer and developer I must
- [x] **Ultra-mega-super-bonus:** Implement Continuous Integration pipeline *(see status badge above)*
- [x] Test and migrate 2015 project to 2017
- [x] extract constants
- [x] seed users, roles, claims
- [x] remove unused usings
- [x] refactor code where needed *// more or less*
- [x] maybe make sure all of the UI looks decent


## Technical details

### Prerequisites
- [Visual Studio 2017 Community](https://www.visualstudio.com/downloads/) and make sure you install the following workloads:
  - ASP.NET and web development
  - Azure development
  - Data storage and processing
  - .NET Core cross-platform development
- [Razor Language Services extension](https://marketplace.visualstudio.com/items?itemName=ms-madsk.RazorLanguageServices)

![Workloads part 1](/docs/images/vs1.png)
![Workloads part 2](/docs/images/vs2.png)

*FYI: [Known issues with .NET Core in Visual Studio 2017](https://github.com/aspnet/Tooling/blob/master/known-issues-vs2017.md)*

*More FYI: just for the fun of it, see below how many SEPARATE things you had to install if we would have continued with VS2015 :)*
- ~~Visual Studio 2015 Community or Professional Edition with Update 3 (https://go.microsoft.com/fwlink/?LinkId=691978&clcid=0x409)~~
- ~~Latest Azure SDK (2.9.6 current) (https://go.microsoft.com/fwlink/?LinkId=518003&clcid=0x409)~~
- ~~SQL Server Data Tools for VS2015 (https://msdn.microsoft.com/mt186501)~~
- ~~.NET Core tools preview for Visual Studio (1.0.1 tools Preview 2 current) (https://go.microsoft.com/fwlink/?LinkID=827546)~~
- ~~latest version of Nuget package manager (https://www.nuget.org/)~~

### Setup
- open solution in VS
- CTRL + Q and search for Package Manager Console
- once opened select the Data project as the default project (second dropdown)
- run the following command: 
```
Update-Database
```
- expand the src > CloudStorage > Dependencies nodes, right click on the Bower folder and click on Restore Packages
- *or as an alternative for the above step, open up the CloudStorage project folder in Command Prompt (make sure the bower.json file is in that folder) and type in the following command: bower install*
- that's it, just make sure the CloudStorage project is selected as the default one, hit F5 and enjoy! (VS should restore all missing packages automatically)

**Note: this will NOT be deployable to Linux because of it's dependency to WindowsAzure.Storage nuget package**
