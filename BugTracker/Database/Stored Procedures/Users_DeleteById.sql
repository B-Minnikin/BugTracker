USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Users_DeleteById]    Script Date: 30/07/2020 10:02:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Delete the specified User from the Users table.
-- =============================================
CREATE PROCEDURE [dbo].[Users_DeleteById]
	@UserId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM dbo.Users
	WHERE Id = @UserId
END
GO

