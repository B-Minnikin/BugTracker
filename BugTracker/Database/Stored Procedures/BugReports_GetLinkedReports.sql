USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[BugReports_GetLinkedReports]    Script Date: 12/01/2021 13:39:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[BugReports_GetLinkedReports]
	@BugReportId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT report.* FROM BugReport AS report
	INNER JOIN BugReportLinks AS links
	ON (report.BugReportId = links.BugReportId AND links.LinkToBugReportId = @BugReportId)
		OR (report.BugReportId = links.LinkToBugReportId AND links.BugReportId = @BugReportId)
END
GO


