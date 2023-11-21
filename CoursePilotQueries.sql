IF EXISTS(SELECT * FROM Sys.databases WHERE name = 'CoursePilotDB')
DROP DATABASE CoursePilotDB
CREATE DATABASE CoursePilotDB
USE CoursePilotDB

CREATE TABLE Users (
    ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    Username VARCHAR(50) NOT NULL,
	[PasswordHash] VARCHAR(128) NOT NULL
);

CREATE INDEX IX_Username ON Users (Username);

CREATE TABLE Modules (
    ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	UserID INT NOT NULL,
	ModuleCode VARCHAR(10) NOT NULL,
	[moduleName] VARCHAR(50) NOT NULL,
	credits INT NOT NULL,
	weeklyClassHours Decimal(18,2) NOT NULL,
	weeksInSem INT NOT NULL,
	startDate Date NOT NULL,
	HoursStudied Decimal(18,2) NOT NULL,
	selfStudyHoursleft Decimal(18,2) NOT NULL,
	LastStudyDate DateTime NOT NULL,
	selfStudyHoursPerWeek Decimal(18,2) NOT NULL,
	FOREIGN KEY (UserID) REFERENCES Users(ID) 
);
