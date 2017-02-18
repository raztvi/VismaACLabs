# VismaACLabs - Cloud Storage
Hands on lab application for a course for students 


### As a registered user I can
- [ ] register an account on the application and associate it with an existing company
- [ ] see a list of files uploaded by users in my company
- [ ] download any file from the above list
- [ ] edit the metadata of any file from above
- [ ] edit the contents of any file from above (re-upload)
- [ ] delete a file from the above list
- [x] upload a new file along with metadata
- [ ] be notified in real time when someone from my company uploads a new file


### As an admin I can
- [x] see a list of all available companies
- [ ] create a new company
- [ ] edit an existing company
- [x] see all uploaded files
- [x] edit any file or metadata
- [x] delete any file 
- [x] upload a file


### As a developer I will
- [x] use azure storage
- [x] use sql local db
- [ ] write some unit tests


**Note: this will NOT be deployable to Linux because of it's dependency to WindowsAzure.Storage nuget package**