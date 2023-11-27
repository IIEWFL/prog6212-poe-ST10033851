
# Course Pilot application ReadMe

This readme describes how to compile and execute the Time Management Application, 
a standalone desktop application built using C# and .net Core MVC and sql server as the backend database.
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

6. New Feature: Graphical Representation of Study Hours Over Time:

The web application now includes a feature that visually represents the number of hours spent on a module per week in the format of a graph.
Additionally, the ideal calculated number of hours is displayed on the graph for reference.

## Part 2 changes

1. The application now allows the you to create an account and login in to your account. You will only be able to see your own data and 
no any other user

2. The application now uses a database to persist data, becuase of this, the applcation now uses multi-threading to ensure that the user interface does
not become unresponsive when retrieving data from the database.

3. Included a sql file that must be run to create the database and necessary tables

## Part 3 Changes

1. The application now uses .net Core MVC instead of a wpf application.

2. Graphical Representation of Study Hours Over Time:
The web application now provides a graphical representation of the number of hours spent on each module per week.
A graph displays the study hours over time, offering a visual understanding of study patterns throughout the semester.
The ideal calculated number of hours is incorporated into the graph for easy comparison and tracking of study goals.

3. The application now uses entity frameowork core instead of ADO .net

## Important Reminders
The application now saves data in a database
The code adheres to widely recognised coding standards and includes detailed comments that explain the names of variables, methods, and logic.
To modify data, LINQ is used.
According to the project requirements, a custom class library with classes relevant to data and calculations is added.

NB: After the video was made, i did fix the issue where the self study hours left would go below 0 and have a negative value.

### how to connect to the database
1. Instead of creating the tables from a sql file, the application makes use of migrations.

2. Open sql server and copy the server name that you use to connect.

3. Open Visual studio and in the ```appsettings.json``` file, you should see placeholder called ```ConnString```

4. Replace the value of the Server with the server name you just copied, it should look like this:

```Server=yourservername;```

5. Now open the package manager console, copy and run this command: ```Update-Database -Migration '20231122152843_Initial_Migration'```

6. You are now ready to run the application.

