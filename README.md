
# Course Pilot application ReadMe

This readme describes how to compile and execute the Time Management Application, 
a standalone desktop application built using C# and Windows Presentation Foundation (WPF). 
Users may use this programme to organise their study courses for an entire semester and track their self-study hours. 
The processes for compiling and running the programme are outlined below.

## Compilation steps

1. Open the Project in Visual Studio:
Launch Visual Studio.
Click on "Open a project or solution."
Navigate to the directory where you cloned or extracted the project files and select the solution file
(usually with a .sln extension).

2.Build the Project: Right-click on the class library ModuleManagerCL and choose "Build" to compile the application.

3. Run the Application:
Once the build process is complete without any errors, you can run the application by clicking the "Start" or "Debug" button in Visual Studio.

4. Use the Application:
The application should open with the user interface, allowing you to add modules for the semester, specify the number of weeks, start dates, and record study hours.

5. View Modules and Study Progress:
You can view the list of modules and their respective self-study hour calculations.
The application will display how many hours of self-study remain for each module for the current week based on recorded study hours.

## Part 2 changes

1. The application now allows the you to create an account and login in to your account. You will only be able to see your own data and 
no any other user

2. The application now uses a database to persist data, becuase of this, the applcation now uses multi-threading to ensure that the user interface does
not become unresponsive when retrieving data from the database.

3. Included a sql file that must be run to create the database and necessary tables

## Important Reminders
The application does not save user data between runs; instead, data is saved in memory while the programme is operating.
The code adheres to widely recognised coding standards and includes detailed comments that explain the names of variables, methods, and logic.
To modify data, LINQ is used.
According to the project requirements, a custom class library with classes relevant to data and calculations is added.

### how to connect to the database
1. Open the sql file called CoursePilotQueries in SSMS and execute all the queries, you should then have a database called CoursePilotDB

2. Right click the database and go to properties and copy the SERVER name

3. Open Visual studio and in the app.config file, find the connectionStrings tag and replace the value of the Data Source to the server
name you just copied, it should look like this:

```data source=yourservername;```

4.In the ModuleManagerCL file, open the Class1 file and in the "moduleData" class replace the data source in the connection
string to the same server name you copied

5. Finally, rebuild the ModuleManagerCL file

6. Your are now ready to run the application

