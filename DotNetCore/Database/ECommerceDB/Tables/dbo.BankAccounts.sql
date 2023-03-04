CREATE TABLE BankAccounts
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
	BankName NVARCHAR(100),
    BranchName NVARCHAR(100) NOT NULL,
    IfscCode NVARCHAR(100)  NOT NULL,
    AccountNumber INT  NOT NULL,
    AccountType INT NOT NULL,
    AccountHolder1Name NVARCHAR(100) NOT NULL,
    AccountHolder2Name NVARCHAR(100) NULL,
    Holder1Email NVARCHAR(100) NOT NULL,
    Holder2Email NVARCHAR(100) NULL,
	Holder1Address NVARCHAR(100) NOT NULL,
	Holder2Address NVARCHAR(100) NULL,
    CompanyName NVARCHAR(100) NULL,
	GSTNo NVARCHAR(100) NULL,
	AccountBalance DECIMAL(10,3)  NOT NULL
   CONSTRAINT UQ_BankAccounts_AccountNumber UNIQUE(AccountNumber)
)