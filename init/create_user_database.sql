USE [master];
GO

-- Création de l'utilisateur
CREATE LOGIN [projet_manager_owner] WITH PASSWORD = 'ProjetctManager2024//**', CHECK_POLICY = OFF;
GO

-- Création de la base de données si elle n'existe pas
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'project_manager')
BEGIN
    CREATE DATABASE [project_manager]
END
GO

-- Utilisation de la base de données
USE [project_manager];
GO

-- Création de l'utilisateur dans la base de données
CREATE USER [projet_manager_owner] FOR LOGIN [projet_manager_owner];
GO

-- Attribution des droits
EXEC sp_addrolemember N'db_owner', N'projet_manager_owner';
GO
