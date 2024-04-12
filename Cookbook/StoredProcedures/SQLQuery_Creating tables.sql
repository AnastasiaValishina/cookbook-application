CREATE DATABASE CookbookAppDatabase
GO

USE CookbookAppDatabase
GO

CREATE SCHEMA CookbookAppSchema
GO

DROP TABLE IF EXISTS CookbookAppSchema.Ingredients
GO

DROP TABLE IF EXISTS CookbookAppSchema.Recipes
GO

DROP TABLE IF EXISTS CookbookAppSchema.Categories
GO

CREATE TABLE CookbookAppSchema.Recipes
(
    RecipeId INT IDENTITY(1, 1) PRIMARY KEY,
    UserId INT,
    Title NVARCHAR(MAX),
    Notes NVARCHAR(MAX),
    CategoryId INT,
    RecipeCreated DATETIME,
    RecipeUpdated DATETIME,
    Source NVARCHAR(MAX)
)

CREATE INDEX ix_Recipes_UserId ON CookbookAppSchema.Recipes(UserId)
GO

CREATE TABLE CookbookAppSchema.Ingredients (
    IngredientId INT IDENTITY(1, 1),
    RecipeId INT,
    Name NVARCHAR(MAX),
    Qty FLOAT,
    Unit NVARCHAR(50),
    FOREIGN KEY (RecipeId) REFERENCES CookbookAppSchema.Recipes(RecipeId) ON DELETE CASCADE
);

CREATE CLUSTERED INDEX cix_Ingredients_RecipeId_IngredientId ON CookbookAppSchema.Ingredients(RecipeId, IngredientId)
GO

CREATE TABLE CookbookAppSchema.Categories
(
    Id INT IDENTITY(1, 1) PRIMARY KEY,
    CategoryName NVARCHAR(50) NOT NULL
)

CREATE TABLE CookbookAppSchema.Users
(
    UserId INT IDENTITY(1, 1) PRIMARY KEY
    , UserName NVARCHAR(50)
    , Email NVARCHAR(50)
); 

CREATE TABLE CookbookAppSchema.Auth
(
	Email NVARCHAR(50) PRIMARY KEY,
	PasswordHash VARBINARY(MAX),
	PasswordSalt VARBINARY(MAX)
)

INSERT INTO CookbookAppSchema.Categories(
    [CategoryName]
    ) VALUES ('Soups');

INSERT INTO CookbookAppSchema.Categories(
    [CategoryName]
    ) VALUES ('Deserts');

INSERT INTO CookbookAppSchema.Categories(
    [CategoryName]
    ) VALUES ('Salads');


