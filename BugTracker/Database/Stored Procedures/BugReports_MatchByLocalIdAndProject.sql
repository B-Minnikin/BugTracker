USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[BugReports_MatchByLocalIdAndProject]    Script Date: 30/12/2020 09:19:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[BugReports_MatchByLocalIdAndProject]
	@Query nvarchar(max),
	@ProjectId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT BugReport.LocalBugReportId, BugReport.Title
	FROM BugReport
	WHERE CONVERT(nvarchar(max), BugReport.LocalBugReportId) LIKE ('%' + @Query + '%') AND
		ProjectId = @ProjectId
END
GO
