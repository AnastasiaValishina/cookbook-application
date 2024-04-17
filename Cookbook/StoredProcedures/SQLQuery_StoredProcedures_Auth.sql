
USE CookbookAppDatabase
GO

CREATE OR ALTER PROCEDURE CookbookAppSchema.spRegistration_Upsert
    @Email NVARCHAR(50),
    @PasswordHash VARBINARY(MAX),
    @PasswordSalt VARBINARY(MAX)
AS 
BEGIN
    IF NOT EXISTS (SELECT * FROM CookbookAppSchema.Auth WHERE Email = @Email)
        BEGIN
            INSERT INTO CookbookAppSchema.Auth(
                [Email],
                [PasswordHash],
                [PasswordSalt]
            ) VALUES (
                @Email,
                @PasswordHash,
                @PasswordSalt
            )
        END
    ELSE
        BEGIN
            UPDATE CookbookAppSchema.Auth 
                SET PasswordHash = @PasswordHash,
                    PasswordSalt = @PasswordSalt
                WHERE Email = @Email
        END
END
GO

CREATE OR ALTER PROCEDURE CookbookAppSchema.spLoginConfirmation_Get
    @Email NVARCHAR(50)
AS
BEGIN
    SELECT [Auth].[PasswordHash],
        [Auth].[PasswordSalt] 
    FROM TutorialAppSchema.Auth AS Auth 
        WHERE Auth.Email = @Email
END;
GO