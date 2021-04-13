USE [BugTrackerDB]
GO
/****** Object:  StoredProcedure [dbo].[BugReports_MatchByLocalIdAndProject]    Script Date: 13/04/2021 18:52:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[BugReports_MatchByLocalIdAndProject]
	@Query nvarchar(max),
	@ProjectId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT BugReport.BugReportId, BugReport.LocalBugReportId, BugReport.Title
	FROM BugReport
	WHERE CONVERT(nvarchar(max), BugReport.LocalBugReportId) LIKE ('%' + @Query + '%') AND
		ProjectId = @ProjectId
END
