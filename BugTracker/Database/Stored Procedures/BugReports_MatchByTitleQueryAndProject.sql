USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[BugReports_MatchByTitleQueryAndProject]    Script Date: 30/12/2020 09:22:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[BugReports_MatchByTitleQueryAndProject]
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
	WHERE BugReport.Title LIKE ('%' + @Query + '%') AND
		ProjectId = @ProjectId
END
GO
