CREATE TABLE Customers
(
	Id INT IDENTITY(1,1),
	Name NVARCHAR(50),
	Email NVarchar(50),
	Gender INT,
	Age INT,
	Country NVARCHAR(50),
	CONSTRAINT CustomersUniqueEmail UNIQUE(Email)
)
