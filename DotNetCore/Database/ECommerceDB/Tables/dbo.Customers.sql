CREATE TABLE Customers
(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	Name NVARCHAR(50),
	Email NVarchar(50),
	Gender INT,
	Age INT,
	Country NVARCHAR(50),
	CONSTRAINT UQ_Customers_Email UNIQUE(Email)
)
