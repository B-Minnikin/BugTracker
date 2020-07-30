USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Comments_GetById]    Script Date: 30/07/2020 09:58:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Get a specific comment by its ID.
-- =============================================
CREATE PROCEDURE [dbo].[Comments_GetById]
	@BugReportCommentId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.BugReportComment WHERE BugReportCommentId = @BugReportCommentId
END
GO

