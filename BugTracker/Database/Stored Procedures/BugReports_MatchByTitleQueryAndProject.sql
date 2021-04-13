USE [BugTrackerDB]
GO
/****** Object:  StoredProcedure [dbo].[BugReports_MatchByTitleQueryAndProject]    Script Date: 13/04/2021 18:56:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[BugReports_MatchByTitleQueryAndProject]
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
	WHERE BugReport.Title LIKE ('%' + @Query + '%') AND
		ProjectId = @ProjectId
END
