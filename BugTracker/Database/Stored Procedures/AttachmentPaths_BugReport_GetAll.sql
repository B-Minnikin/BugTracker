USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[AttachmentPaths_BugReport_GetAll]    Script Date: 30/07/2020 09:51:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:
-- =============================================
CREATE PROCEDURE [dbo].[AttachmentPaths_BugReport_GetAll]
	@ParentId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.AttachmentPath WHERE BugReportId = @ParentId
END
GO

