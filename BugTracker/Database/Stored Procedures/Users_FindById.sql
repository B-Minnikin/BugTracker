USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Users_FindById]    Script Date: 30/07/2020 10:02:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Find a User by the specified User ID.
-- =============================================
CREATE PROCEDURE [dbo].[Users_FindById]
	@UserId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.Users WHERE Id = @UserId
END
GO

