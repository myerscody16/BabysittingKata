--CREATE DATABASE BabysittingDb
--use BabysittingDb

--CREATE TABLE FamilyPayRates(
--	Id INT PRIMARY KEY IDENTITY(1,1),
--	FamilyLetter NVARCHAR(1) NOT NULL,
--	PayRate INT NOT NULL,
--	StartTime TIME NOT NULL,
--	EndTime TIME NOT NULL);


--CREATE TABLE Appointments(
--	Id INT PRIMARY KEY IDENTITY(1,1),
--	FamilyId NVARCHAR(1) NOT NULL,
--	StartTime DATETIME NOT NULL,
--	EndTime DATETIME NOT NULL,
--	TotalCost INT);

INSERT INTO FamilyPayRates (FamilyLetter,PayRate, StartTime,EndTime)
VALUES('A',15,'05:00:00','11:00:00'),
('A',20,'11:00:00','04:00:00'),
('B',12,'05:00:00','10:00:00'),
('B',8,'10:00:00','12:00:00'),
('B',16,'12:00:00','04:00:00'),
('C',21,'05:00:00','09:00:00'),
('C',15,'09:00:00','04:00:00')