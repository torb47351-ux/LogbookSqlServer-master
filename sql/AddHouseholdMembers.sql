/* Run once in SSMS after selecting/connecting to LogbookDb.
   Creates a table for each family member; existing family records remain unchanged.
   The head of household is already represented by dbo.Beneficiaries. */
USE [LogbookDb];
GO
IF OBJECT_ID(N'dbo.HouseholdMembers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HouseholdMembers
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_HouseholdMembers PRIMARY KEY,
        BeneficiaryId INT NOT NULL,
        FullName NVARCHAR(200) NOT NULL,
        Relationship NVARCHAR(50) NULL,
        Age INT NULL,
        Occupation NVARCHAR(150) NULL,
        MonthlyIncome DECIMAL(18,2) NULL,
        Notes NVARCHAR(500) NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_HouseholdMembers_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_HouseholdMembers_Beneficiaries_BeneficiaryId FOREIGN KEY (BeneficiaryId)
            REFERENCES dbo.Beneficiaries(Id) ON DELETE CASCADE,
        CONSTRAINT CK_HouseholdMembers_Age CHECK (Age IS NULL OR (Age >= 0 AND Age <= 120)),
        CONSTRAINT CK_HouseholdMembers_MonthlyIncome CHECK (MonthlyIncome IS NULL OR MonthlyIncome >= 0)
    );
    CREATE INDEX IX_HouseholdMembers_BeneficiaryId ON dbo.HouseholdMembers(BeneficiaryId);
END;
GO
