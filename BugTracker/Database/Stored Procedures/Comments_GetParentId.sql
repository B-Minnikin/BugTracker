USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Comments_GetParentId]    Script Date: 30/07/2020 09:58:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Takes a comment's ID and returns the BugReport
--		ID to which it belongs.
-- =============================================
CREATE PROCEDURE [dbo].[Comments_GetParentId]
	@CommentId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT BugReportId FROM dbo.Comment WHERE CommentId = @CommentId
END
GO

