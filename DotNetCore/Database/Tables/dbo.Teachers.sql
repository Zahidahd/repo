CREATE TABLE Teachers
(
	Id INT IDENTITY(1,1),
	FullName NVARCHAR(50) NOT NULL,
	Email NVARCHAR(50) NOT NULL,
	Age INT,
	Gender INT,
	SchoolName NVARCHAR(50),
	Department NVARCHAR(50),
	Salary INT
	CONSTRAINT UniqueEmail UNIQUE(Email)
)