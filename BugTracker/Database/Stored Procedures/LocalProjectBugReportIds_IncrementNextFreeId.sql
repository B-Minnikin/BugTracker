USE [BugTrackerDB]
GO
/****** Object:  StoredProcedure [dbo].[LocalProjectBugReportIds_IncrementNextFreeId]    Script Date: 07/12/2020 11:33:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[LocalProjectBugReportIds_IncrementNextFreeId]
	@ProjectId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE LocalProjectBugReportIds
	SET NextFreeId = NextFreeId + 1
	WHERE LocalProjectBugReportIds.ProjectId = @ProjectId
END
