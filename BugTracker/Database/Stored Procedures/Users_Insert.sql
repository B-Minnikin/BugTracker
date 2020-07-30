USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Users_Insert]    Script Date: 30/07/2020 10:02:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Create a new User and insert into the Users table.
-- =============================================
CREATE PROCEDURE [dbo].[Users_Insert]
	@UserName nvarchar(MAX),
	@NormalizedUserName nvarchar(MAX),
	@Email nvarchar(MAX),
	@NormalizedEmail nvarchar(MAX),
	@PasswordHash nvarchar(MAX),
	@PhoneNumber nvarchar(MAX),
	@EmailConfirmed bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO dbo.Users (UserName, NormalizedUserName, Email, NormalizedEmail, PasswordHash, PhoneNumber, EmailConfirmed)
	VALUES (@UserName, @NormalizedUserName, @Email, @NormalizedEmail, @PasswordHash, @PhoneNumber, @EmailConfirmed)
END
GO

