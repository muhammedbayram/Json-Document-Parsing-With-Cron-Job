# Json Document Parsing With Cron Job And Send a notification E-Mail
The application allows the JSON document selected by the user to be deserialized and uploaded to the database. The user can see the list of documents saved in the database, and detailed information can be accessed by selecting the documents from the list.

##Cron Job
With Cron Expression, json documents are read from the desired path at certain time intervals, the Id is checked, and if there is an unsaved document, it is deserialized and saved to the database.

##Send a notification e-mail
For documents saved in the database with Cron Job, an information e-mail is sent to the user along with the document content. An information e-mail is also sent to the user for the documents uploaded by the user. Receiver Email address is specified in the appsettings.json file.

##Logging
Logging using Serilog.

##UnitTest
xUnit

![index](https://github.com/muhammedbayram/Json-Document-Parsing-With-Cron-Job/assets/99541575/b0126ed2-b60a-4a71-ae4a-02f0f3075584)
