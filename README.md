# Visma AC Labs - Cloud Storage
Hands on lab application for a course for students 


## As a registered user I can
- [x] register an account on the application and associate it with an existing company
- [ ] see a list of files uploaded by users in my company
- [ ] download any file from the above list
- [ ] edit the metadata of any file from above
- [ ] edit the contents of any file from above (re-upload)
- [ ] delete a file from the above list
- [x] upload a new file along with metadata
- [ ] be notified in real time when someone from my company uploads a new file


## As an admin I can
- [x] see a list of all available companies
- [x] create a new company
- [x] edit an existing company
- [x] see all uploaded files
- [x] edit any file or metadata
- [x] delete any file 
- [x] upload a file


## As a developer I will
- [x] use azure storage
- [x] use sql local db
- [x] use SignalR
- [ ] write some unit tests


## As a respectable trainer and developer I must
- [ ] extract constants
- [ ] seed users, roles, claims
- [ ] remove unused usings
- [ ] refactor code where needed
- [ ] not let quickly thrown onto the wall code exist just because it works and it was fast
- [ ] maybe make sure all of the UI looks decent (default / basic style the shit out of everything)


## Prerequisites
- Visual Studio 2015 Community or Professional Edition with Update 3 (https://go.microsoft.com/fwlink/?LinkId=691978&clcid=0x409)
- Latest Azure SDK (2.9.6 current) (https://go.microsoft.com/fwlink/?LinkId=518003&clcid=0x409)
- SQL Server Data Tools for VS2015 (https://msdn.microsoft.com/mt186501)
- .NET Core tools preview for Visual Studio (1.0.1 tools Preview 2 current) (https://go.microsoft.com/fwlink/?LinkID=827546)
- latest version of Nuget package manager (https://www.nuget.org/)

## Setup
- open solution in VS
- CTRL + Q and search for Package Manager Console
- once opened select the Data project as the default project (second dropdown)
- run the following command: Update-Database
- that's it, just make sure the CloudStorage project is selected as the default one, hit F5 and enjoy! (VS should restore all missing packages automatically)

**Note: this will NOT be deployable to Linux because of it's dependency to WindowsAzure.Storage nuget package**