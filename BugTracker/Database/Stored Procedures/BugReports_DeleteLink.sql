USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[BugReports_DeleteLink]    Script Date: 12/01/2021 13:35:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[BugReports_DeleteLink]
	@BugReportId int,
	@LinkToBugReportId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DELETE FROM dbo.BugReportLinks
	WHERE (BugReportId = @BugReportId AND LinkToBugReportId = @LinkToBugReportId) OR
	(BugReportId = @LinkToBugReportId AND LinkToBugReportId = @BugReportId)
END
GO


