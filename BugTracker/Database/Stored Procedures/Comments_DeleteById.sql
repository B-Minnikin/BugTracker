USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Comments_DeleteById]    Script Date: 30/07/2020 09:57:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Deletes the comment specified by ID.
-- =============================================
CREATE PROCEDURE [dbo].[Comments_DeleteById]
	@BugReportCommentId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM dbo.BugReportComment
	WHERE BugReportCommentId = @BugReportCommentId
END
GO

