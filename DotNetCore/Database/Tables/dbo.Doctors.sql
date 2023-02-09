CREATE TABLE Doctors
(
	Id INT IDENTITY(1,1),
	RegistrationNumber INT,
	Name NVARCHAR(50) NOT NULL,
	Email NVARCHAR(50) NOT NULL,
	Gender INT NOT NULL,
	Department NVARCHAR(50),
	City NVARCHAR(50)
    CONSTRAINT UniqueEmail UNIQUE(Email),
    CONSTRAINT UniqueRegistrationNumber UNIQUE(RegistrationNumber)
 )

