/* Run once in SQL Server Management Studio (SSMS), connected to LogbookDb.
   Existing Beneficiaries data is preserved. Photo metadata is stored in SQL Server;
   actual image files are stored under wwwroot/uploads/beneficiaries/<id>/. */
USE [LogbookDb];
GO
IF OBJECT_ID(N'dbo.BeneficiaryMedia', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BeneficiaryMedia
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_BeneficiaryMedia PRIMARY KEY,
        BeneficiaryId INT NOT NULL,
        Category NVARCHAR(20) NOT NULL,
        Agency NVARCHAR(200) NULL,
        Description NVARCHAR(1000) NULL,
        AssistanceDate DATETIME2 NULL,
        ImagePath NVARCHAR(500) NOT NULL,
        UploadedAt DATETIME2 NOT NULL CONSTRAINT DF_BeneficiaryMedia_UploadedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_BeneficiaryMedia_Beneficiaries_BeneficiaryId FOREIGN KEY (BeneficiaryId)
            REFERENCES dbo.Beneficiaries(Id) ON DELETE CASCADE
    );
    CREATE INDEX IX_BeneficiaryMedia_BeneficiaryId ON dbo.BeneficiaryMedia(BeneficiaryId);
END;
GO
