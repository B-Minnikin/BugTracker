USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[BugReports_DeleteById]    Script Date: 30/07/2020 09:56:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Takes a BugReport ID and deletes the specified
--		BugReport.
-- =============================================
CREATE PROCEDURE [dbo].[BugReports_DeleteById]
	@BugReportId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM dbo.BugReport
	WHERE BugReportId = @BugReportId
END
GO

