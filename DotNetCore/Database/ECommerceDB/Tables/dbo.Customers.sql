CREATE TABLE Customers
(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	Name NVARCHAR(50),
	Gender INT,
	Age INT,
    Email NVarchar(50),
	Password NVARCHAR(50),
	Country NVARCHAR(50),
	MobileNumber NVarchar(50),
	LastFailedLoginDate DateTime,
	LastSucccessfulLoginDate DateTime,
	LoginFailedCout Int,
	IsLocked Bit,
	CONSTRAINT UQ_Customers_Email UNIQUE(Email),
	CONSTRAINT UQ_Customers_MobileNumber UNIQUE(MobileNumber)
)
