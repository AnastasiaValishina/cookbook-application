CREATE DATABASE CookbookAppDatabase
GO

USE CookbookAppDatabase
GO

CREATE SCHEMA CookbookAppSchema
GO

DROP TABLE CookbookAppSchema.Ingredients
GO

DROP TABLE CookbookAppSchema.Recipes
GO

DROP TABLE CookbookAppSchema.Categories
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
GO

CREATE TABLE CookbookAppSchema.Ingredients (
    IngredientId INT IDENTITY(1, 1) PRIMARY KEY,
    RecipeId INT,
    Name NVARCHAR(MAX),
    Qty FLOAT,
    Unit NVARCHAR(50),
    FOREIGN KEY (RecipeId) REFERENCES CookbookAppSchema.Recipes(RecipeId)
);

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


INSERT INTO CookbookAppSchema.Categories(
    [CategoryName]
    ) VALUES ('Soups');

INSERT INTO CookbookAppSchema.Categories(
    [CategoryName]
    ) VALUES ('Deserts');

INSERT INTO CookbookAppSchema.Categories(
    [CategoryName]
    ) VALUES ('Salads');


