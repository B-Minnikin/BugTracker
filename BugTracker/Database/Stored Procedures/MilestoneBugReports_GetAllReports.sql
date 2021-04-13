USE [BugTrackerDB]
GO
/****** Object:  StoredProcedure [dbo].[MilestoneBugReports_GetAllReports]    Script Date: 13/04/2021 18:21:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[MilestoneBugReports_GetAllReports] 
	@MilestoneId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
	FROM dbo.BugReport
	INNER JOIN dbo.MilestoneBugReports ON dbo.BugReport.BugReportId = dbo.MilestoneBugReports.BugReportId 
	WHERE dbo.MilestoneBugReports.MilestoneId = @MilestoneId
END
