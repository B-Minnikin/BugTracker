USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[BugReports_GetById]    Script Date: 30/07/2020 09:56:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Takes a BugReport ID and returns BugReports
--		matching that ID.
-- =============================================
CREATE PROCEDURE [dbo].[BugReports_GetById]
	@BugReportId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.BugReport WHERE BugReportId = @BugReportId
END
GO

