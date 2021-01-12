USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[BugReports_GetByLocalId]    Script Date: 12/01/2021 13:37:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[BugReports_GetByLocalId]
	@LocalBugReportId int,
	@ProjectId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.BugReport WHERE LocalBugReportId = @LocalBugReportId AND
		ProjectId = @ProjectId
END
GO


