USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[BugReports_CommentCount]    Script Date: 30/07/2020 09:55:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Takes a BugReport ID and returns the number of
--		associated comments.
-- =============================================
CREATE PROCEDURE [dbo].[BugReports_CommentCount] 
	@BugReportId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT COUNT (*)
	FROM dbo.Comment
	WHERE BugReportId = @BugReportId
END
GO

