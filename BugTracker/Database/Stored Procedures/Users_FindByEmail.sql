USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Users_FindByEmail]    Script Date: 30/07/2020 10:02:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Return the User by email address.
-- =============================================
CREATE PROCEDURE [dbo].[Users_FindByEmail]
	@NormalizedEmail nvarchar(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here	
	SELECT *
	FROM dbo.Users
	WHERE NormalizedEmail = @NormalizedEmail
END
GO

