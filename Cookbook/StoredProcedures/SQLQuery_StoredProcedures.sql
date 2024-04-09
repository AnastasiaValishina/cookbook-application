
USE CookbookAppDatabase
GO

CREATE OR ALTER PROCEDURE CookbookAppSchema.spRecipes_Get
    @UserId INT = NULL
    , @RecipeId INT = NULL
AS
BEGIN
    SELECT [Categories].[Id],
    [Categories].[CategoryName],
    [Recipes].[RecipeId],
    [Recipes].[UserId],
    [Recipes].[Title],
    [Recipes].[Notes],
    [Recipes].[CategoryId],
    [Recipes].[RecipeCreated],
    [Recipes].[RecipeUpdated],
    [Recipes].[Source] 
        FROM CookbookAppSchema.Recipes as Recipes
		INNER JOIN CookbookAppSchema.Categories as Categories
		ON Recipes.CategoryId = Categories.Id
            WHERE Recipes.UserId = ISNULL(@UserId, Recipes.UserId)
                AND Recipes.RecipeId = ISNULL(@RecipeId, Recipes.RecipeId)
END
GO
-- EXEC CookbookAppSchema.spRecipes_Get @UserId = 101
-- EXEC CookbookAppSchema.spRecipes_Get @RecipeId = 2023


CREATE OR ALTER PROCEDURE CookbookAppSchema.spIngredients_Get
    @RecipeId INT = NULL
    , @IngredientId INT = NULL
AS 
BEGIN
    SELECT [Ingredients].[IngredientId],
    [Ingredients].[RecipeId],
    [Ingredients].[Name],
    [Ingredients].[Qty],
    [Ingredients].[Unit] 
        FROM CookbookAppSchema.Ingredients AS Ingredients
            WHERE Ingredients.RecipeId = ISNULL (@RecipeId, Ingredients.RecipeId)
                AND Ingredients.IngredientId = ISNULL (@IngredientId, Ingredients.IngredientId)
END
GO
-- EXEC CookbookAppSchema.spIngredients_Get @RecipeId = 2023, @IngredientId = 


CREATE OR ALTER PROCEDURE CookbookAppSchema.spRecipe_Add
    @UserId INT
    , @Title NVARCHAR(MAX)
    , @Notes NVARCHAR(MAX)
    , @CategoryId INT = NULL
    , @Source NVARCHAR(MAX)
AS
BEGIN
    INSERT INTO CookbookAppSchema.Recipes (
        [UserId],
        [Title],
        [Notes],
        [CategoryId],
        [RecipeCreated],
        [RecipeUpdated],
        [Source]) 
		OUTPUT INSERTED.RecipeId
		VALUES (
            @UserId,
            @Title, 
            @Notes,
            @CategoryId,
            GETDATE(),
            GETDATE(),
            @Source)
END
GO
-- EXEC CookbookAppSchema.spRecipes_Add @UserId = 101, @Title = 'sp test', @Notes = 'sp test', @CategoryId = 1, @Source = 'sp test';


CREATE OR ALTER PROCEDURE CookbookAppSchema.spIngredient_Add
    @RecipeId INT
    , @Name NVARCHAR(MAX)
    , @Qty FLOAT
    , @Unit NVARCHAR(50)
AS
BEGIN
	INSERT INTO CookbookAppSchema.Ingredients (
		[RecipeId],
		[Name],
		[Qty],
		[Unit]
	) VALUES (
        @RecipeId,
        @Name,
        @Qty,
        @Unit)
END
GO
-- EXEC CookbookAppSchema.spIngredient_Add @RecipeId = ,@Name = '', @Qty = '', @Unit = ''   

CREATE OR ALTER PROCEDURE CookbookAppSchema.spIngredient_Upsert
    @IngredientId INT
    , @RecipeId INT
    , @Name NVARCHAR(MAX)
    , @Qty FLOAT
    , @Unit NVARCHAR(50)
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM CookbookAppSchema.Ingredients WHERE IngredientId = @IngredientId)
        BEGIN
	        INSERT INTO CookbookAppSchema.Ingredients (
		        [RecipeId],
		        [Name],
		        [Qty],
		        [Unit]
	        ) VALUES (
                @RecipeId,
                @Name,
                @Qty,
                @Unit)
        END
    ELSE
        BEGIN
            UPDATE CookbookAppSchema.Ingredients

        END
END
GO
-- EXEC CookbookAppSchema.spIngredient_Add @RecipeId = ,@Name = '', @Qty = '', @Unit = ''   


CREATE OR ALTER PROCEDURE CookbookAppSchema.spRecipes_GetBySearch
    @SearchParam NVARCHAR(MAX) = NULL
AS
BEGIN
    SELECT DISTINCT 
        [Recipes].[RecipeId],
        [Recipes].[UserId],
		[Recipes].[Title],
		[Recipes].[Notes],
		[Recipes].[CategoryId],
		[Recipes].[RecipeCreated],
		[Recipes].[RecipeUpdated],
		[Recipes].[Source],
		[Categories].[CategoryName]
    FROM CookbookAppSchema.Recipes Recipes
    LEFT JOIN CookbookAppSchema.Categories Categories 
        ON Recipes.CategoryId = Categories.Id
    LEFT JOIN CookbookAppSchema.Ingredients Ingredients 
        ON Recipes.RecipeId = Ingredients.RecipeId
            WHERE Recipes.Title LIKE '%' + @SearchParam + '%'
                OR Recipes.Notes LIKE '%' + @SearchParam + '%'
                OR Categories.CategoryName LIKE '%' + @SearchParam + '%'
                OR EXISTS (
                    SELECT 1
                    FROM CookbookAppSchema.Ingredients
                    WHERE RecipeId = Recipes.RecipeId
                    AND Ingredients.Name LIKE '%' + @SearchParam + '%'
            );
END
GO
-- EXEC CookbookAppSchema.spRecipes_GetBySearch @SearchParam = 'tomato'


CREATE OR ALTER PROCEDURE CookbookAppSchema.spRecipes_Delete
    @RecipeId INT
    , @UserId INT 
AS
BEGIN
    DELETE FROM CookbookAppSchema.Recipes 
        WHERE RecipeId = @RecipeId
            AND UserId = @UserId
END
GO
-- EXEC CookbookAppSchema.spRecipes_Delete @RecipeId = , @UserId = 

CREATE OR ALTER PROCEDURE CookbookAppSchema.spRecipes_Update
    @RecipeId INT 
    , @Title NVARCHAR(MAX)
    , @Notes NVARCHAR(MAX)
    , @CategoryId INT
    , @Source NVARCHAR(MAX)
AS
BEGIN
    UPDATE CookbookAppSchema.Recipes
    SET Title = @Title,
        Notes = @Notes,
        CategoryId = @CategoryId,
        RecipeUpdated = GETDATE(),
        Source = @Source
            WHERE RecipeId = @RecipeId
END
GO
-- EXEC CookbookAppSchema.spRecipes_Update @RecipeId = , @Title '', @Notes = '', @CategoryId = '', @Source = '' 

