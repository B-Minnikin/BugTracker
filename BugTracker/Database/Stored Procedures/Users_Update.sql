USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Users_Update]    Script Date: 30/07/2020 10:03:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Update the specified User by ID.
-- =============================================
CREATE PROCEDURE [dbo].[Users_Update]
	@UserId int,
	@UserName nvarchar(MAX),
	@NormalizedUserName nvarchar(MAX),
	@Email nvarchar(MAX),
	@NormalizedEmail nvarchar(MAX),
	@PhoneNumber nvarchar(MAX),
	@PasswordHash nvarchar(MAX),
	@EmailConfirmed bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE dbo.Users
	SET UserName = @UserName, NormalizedUserName = @NormalizedUserName, Email = @Email, NormalizedEmail = @NormalizedEmail, EmailConfirmed = @EmailConfirmed, PhoneNumber = @PhoneNumber, PasswordHash = @PasswordHash
	WHERE Id = @UserId
END
GO

