# PhotoContest
This is a school project, we have to build a photogallery where you can (obviously) upload photo and vote other photos!

Not the ones uploaded by you anyway.. everyone already know you like it ;)


This project is fully tested with **Amazon Web Services** and partially tested with **Azure**.

This project needed to be easily extensible; the division of every functionality in a different library, the abstraction of the implementation behind an interface and a single point in which is specified the implementation to use (thanks to dependency injection) is easy to extend the application to use different tecnology and providers.


In the project there are many reference to thumbnails, so you need to know that currently these are generated by a Node.Js script that you can find on GitHub [here](https://github.com/sebejbaldin/photocontest-consumer).

This script get the image url from a RabbitMQ queue, make the thumbnail and upload it on AWS S3.

The process actually is:
1.    You upload a photo on the photogallery
2.    The image is uploaded on AWS S3
3.    In the database is added a record for the foto
4.    An AWS Lambda is triggered by the upload to insert the photo url in the queue
5.    The photocontest-consumer get the url from the queue and process the image to make the thumbnail
6.    The thumbnail is uploaded on AWS S3
7.    The record in the database is updated with the thumbnail url

## Features
* Upload your photo
* Vote the photo you like
* Leave comments under the photos
* Search photo
  * By username
  * By user email
  * By photo name 
* View a rank of the best photos
* Delete the photo you have uploaded
* Client-side and server-side pagination

For the most basic usage you only need a database (such as PostgreSQL or SQLServer), but with this setup the search feature is not available.
If you do not have a cloud service available there is an implementation that save the uploaded photo on a local folder, this is the default if not otherwise specified.
To test all the functionalities you also need ElasticSearch.
Redis is not needed to test, is only supposed to speed up performance.

## Installation

### Install the .NET Core environment
You can download the SDK for every platform [here](https://dotnet.microsoft.com/download).
Make sure to download the **.NET Core 2.2** version of the framework.
Now follow the setup and install it.

To check if you installed it correctly you can submit this command from cmd or bash(where *x* is anything, only matters major version is 2.2):
```bash
~$  dotnet --version
    2.2.x
```


### Clone the project
Now you have to get the sources.

If you have git installed you can do this:
```bash
~$  git clone https://github.com/sebejbaldin/photogallery.git 
~$  cd photogallery/
```

If you don't have git installed you can download a zip file containing the sources from the *Clone or download* green button


### Configure the project
You have to add configurations to say the application where to find the database and so on.
The minimum configuration needed is a connection string to a database, and a value for the storage field.

To do this you have to replace the *appsettings.json* file with a schema like the following:
```json
{
  "ConnectionStrings": { //these are connection strings that are used for Identity and custom data (pictures, votes, comments) 
    "PgSQLConnOnline": "",
    "SQLServerConn": "",
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Storage": "Amazon", //this parameter value must be equal to one child of the CloudStorage object because is used to determine what storage to use, if it doesn't match any the application will default to use a local folder
  "CloudStorage": {
    //this contains the configuration values for the storage in the cloud
    "Amazon": {
      "Account": "",
      "Secret": "",
      "Storage": "",
      "Bucket": "",
      "Folder": ""
    },
    "Azure": {
      "Account": "",
      "Secret": "",
      "Storage": "",
      "Folder": ""
    }
  },
  "Caching": {
    //This section will contain the configurations for the cache providers
    "Redis": {
      "Host": "",
      "Password": ""
    }
  },
  "Search": {
    //This section will contain the configurations for the search providers
    "Elasticsearch": {
      "ConnectionString": "",
      "Host": "",
      "Port": ""
    }
  }
}
```

### Configure the environment
To run the project you have to get the dependency and build it
```bash
~$  dotnet restore //this download the project dependency needed to work
~$  dotnet build 
```

Now you need to prepare the database to use Identity(to make this you need to have specified a connection string in the appsetting.json file).
From the root folder of the project you should submit these commands: 
```bash
~$  dotnet ef migrations add firstSchema
~$  dotnet ef database update
```
This will create all the tables needed by identity in the database.
About the schema for the other resources (images, comments and votes) I will upload a dump of the database, or you can view the models used in the code to get the fields and the query for the table names (and the database column names as well :)).

## Run the project

To run the project you have to use this command:
```bash
~$  dotnet Baldin.SebEJ.Gallery.Web.dll
```
