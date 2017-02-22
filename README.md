# VismaACLabs - Cloud Storage
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
- [ ] write some unit tests


## As a respectable trainer and developer I must
- [ ] extract constants
- [ ] seed users, roles, claims
- [ ] remove unused usings
- [ ] refactor code where needed
- [ ] not let quickly thrown onto the wall code exist just because it works and it was fast
- [ ] maybe make sure all of the UI looks decent (default / basic style the shit out of everything)

**Note: this will NOT be deployable to Linux because of it's dependency to WindowsAzure.Storage nuget package**