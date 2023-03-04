CREATE TABLE Employees
(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	FullName NVARCHAR(50) NOT NULL,
	Gender INT NOT NULL,
	Email NVARCHAR(50),
	Password NVARCHAR(50),
	MobileNumber NVARCHAR(50),
	DateOfJoining DATETIME,
	Salary DECIMAL,
	LastFailedLoginDate DATETIME,
	LastSuccessfulLoginDate DATETIME,
	LoginFailedCount INT,
	IsLocked BIT
	CONSTRAINT UQ_Employees_Email UNIQUE(Email),
	CONSTRAINT UQ_Employees_MobileNumber UNIQUE(MobileNumber)
)