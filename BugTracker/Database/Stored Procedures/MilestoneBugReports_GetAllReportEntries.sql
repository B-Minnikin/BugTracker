USE [BugTrackerDB]
GO
/****** Object:  StoredProcedure [dbo].[MilestoneBugReports_GetAllReportEntries]    Script Date: 15/04/2021 22:01:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[MilestoneBugReports_GetAllReportEntries] 
	@MilestoneId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT dbo.BugReport.BugReportId, dbo.BugReport.LocalBugReportId, dbo.BugReport.Title
	FROM dbo.BugReport
	INNER JOIN dbo.MilestoneBugReports ON dbo.BugReport.BugReportId = dbo.MilestoneBugReports.BugReportId 
	WHERE dbo.MilestoneBugReports.MilestoneId = @MilestoneId
END
