CREATE TABLE Teachers
(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	FullName NVARCHAR(50) NOT NULL,
	Age INT,
	Gender INT,
    Email NVARCHAR(50) NOT NULL,
	MobileNumber NVARCHAR(50) NOT NULL, 
	SchoolName NVARCHAR(50),
	Department NVARCHAR(50),
	Salary INT
	CONSTRAINT UQ_Teachers_Email UNIQUE(Email),
	CONSTRAINT UQ_Teachers_MobileNumber UNIQUE(MobileNumber)
)