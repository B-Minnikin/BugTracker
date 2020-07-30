USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[BugReports_GetAll]    Script Date: 30/07/2020 09:56:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description: Takes a Project ID and returns a collection of
--		associated BugReports.
-- =============================================
CREATE PROCEDURE [dbo].[BugReports_GetAll]
	@ProjectId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.BugReport WHERE ProjectId = @ProjectId
END
GO

