USE CookbookAppDatabase;
GO

CREATE OR ALTER PROCEDURE CookbookAppSchema.spUser_Upsert
	@UserName NVARCHAR(50),
	@Email NVARCHAR(50),
	@UserId INT = NULL
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM CookbookAppSchema.Users WHERE UserId = @UserId)
        BEGIN
        IF NOT EXISTS (SELECT * FROM CookbookAppSchema.Users WHERE Email = @Email)
            BEGIN
                INSERT INTO CookbookAppSchema.Users(
                    [UserName],
                    [Email]
                ) OUTPUT INSERTED.UserId
                VALUES (
                    @UserName,
                    @Email
                )
            END
        END
    ELSE 
        BEGIN
            UPDATE CookbookAppSchema.Users 
                SET UserName = @UserName,
                    Email = @Email
                WHERE UserId = @UserId
        END
END;
GO

CREATE OR ALTER PROCEDURE CookbookAppSchema.spUser_Delete
    @UserId INT
AS
BEGIN
    DECLARE @Email NVARCHAR(50);

    SELECT  @Email = Users.Email
      FROM  CookbookAppSchema.Users
     WHERE  Users.UserId = @UserId;

    DELETE  FROM CookbookAppSchema.Recipes
     WHERE  Recipes.UserId = @UserId;

    DELETE  FROM CookbookAppSchema.Users
     WHERE  Users.UserId = @UserId;

    DELETE  FROM CookbookAppSchema.Auth
     WHERE  Auth.Email = @Email;
END;
GO