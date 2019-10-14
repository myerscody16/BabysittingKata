This program has been created using the ASP.NET framework in conjunction with the C# (.NET core) language. 

This program allows for a babysitter to create a list of appointments with their nightly charges based upon the prices set by the families. The program makes use of an SQL database (I use Microsoft SQL server management studio 2018, which is free), which I have added the SQL queries for in this file (please check below).

On the index page, a user will be able to see any appointment that is within the next two weeks along with their nightly charge. I also have another page that shows every appointment in the database, broken up by which family it is for. 

On the backend of this, I have included validation for inavlid times, so a user cannot put in a start time that is at or after the end time. For further validation, there is a confirm appointment page that makes sure the user is satisfied with their choices. 



CREATE DATABASE BabysittingDb
use BabysittingDb

CREATE TABLE FamilyPayRates(
	Id INT PRIMARY KEY IDENTITY(1,1),
	FamilyLetter NVARCHAR(1) NOT NULL,
	PayRate INT NOT NULL,
	StartTime TIME NOT NULL,
	EndTime TIME NOT NULL);


CREATE TABLE Appointments(
	Id INT PRIMARY KEY IDENTITY(1,1),
	FamilyId NVARCHAR(1) NOT NULL,
	StartDate DATE NOT NULL,
	StartTime TIME NOT NULL,
	EndTime TIME NOT NULL,
	TotalCost INT);

INSERT INTO FamilyPayRates (FamilyLetter,PayRate, StartTime,EndTime)
VALUES('A',15,'05:00:00','11:00:00'),
('A',20,'11:00:00','04:00:00'),
('B',12,'05:00:00','10:00:00'),
('B',8,'10:00:00','12:00:00'),
('B',16,'12:00:00','04:00:00'),
('C',21,'05:00:00','09:00:00'),
('C',15,'09:00:00','04:00:00')
