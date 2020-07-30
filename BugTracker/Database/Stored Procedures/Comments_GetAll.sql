USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Comments_GetAll]    Script Date: 30/07/2020 09:58:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Get all comments for the specified BugReport.
-- =============================================
CREATE PROCEDURE [dbo].[Comments_GetAll] 
	@BugReportId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.BugReportComment WHERE BugReportId = @BugReportId
END
GO

