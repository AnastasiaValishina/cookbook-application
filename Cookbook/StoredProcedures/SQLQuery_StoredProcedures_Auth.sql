
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
    FROM CookbookAppSchema.Auth AS Auth 
        WHERE Auth.Email = @Email
END;
GO

CREATE OR ALTER PROCEDURE CookbookAppSchema.spEmailExists_Get
    @Email NVARCHAR(50)
AS
BEGIN
    SELECT [Auth].[Email] 
    FROM CookbookAppSchema.Auth AS Auth 
        WHERE Auth.Email = @Email
END;
GO

-- EXEC CookbookAppSchema.spEmailExists_Get @Email = ''

CREATE OR ALTER PROCEDURE CookbookAppSchema.spUserId_Get
    @Email NVARCHAR(50) = NULL,
    @UserId INT = NULL
AS
BEGIN
    SELECT [Users].[UserId] 
    FROM CookbookAppSchema.Users AS Users
        WHERE Users.Email = ISNULL (@Email, Users.Email)
            AND Users.UserId = ISNULL (@UserId, Users.UserId)
END;
GO

CREATE OR ALTER PROCEDURE CookbookAppSchema.spUser_Get
    @UserId INT = NULL
AS
BEGIN
    SELECT [Users].[UserId],
        [Users].[UserName],
        [Users].[Email] 
            FROM CookbookAppSchema.Users AS Users
                WHERE Users.UserId = @UserId
END;
GO

-- EXEC CookbookAppSchema.spUser_Get @UserId = 6




