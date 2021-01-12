USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[BugReports_InsertLink]    Script Date: 12/01/2021 13:33:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[BugReports_InsertLink]
	@BugReportId int,
	@LinkToBugReportId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF NOT EXISTS (SELECT * FROM dbo.BugReportLinks
		WHERE (BugReportId = @BugReportId AND LinkToBugReportId = @LinkToBugReportId) OR
		(BugReportId = @LinkToBugReportId AND LinkToBugReportId = @BugReportId))
    -- Insert statements for procedure here
	BEGIN
		INSERT INTO dbo.BugReportLinks (BugReportId, LinkToBugReportId)
		VALUES (@BugReportId, @LinkToBugReportId)
	END
END
GO


