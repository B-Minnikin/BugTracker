USE [BugTrackerDB]
GO
/****** Object:  StoredProcedure [dbo].[LocalProjectBugReportIds_GetNextFreeId]    Script Date: 07/12/2020 11:33:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[LocalProjectBugReportIds_GetNextFreeId]
	@ProjectId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT NextFreeId FROM LocalProjectBugReportIds WHERE LocalProjectBugReportIds.ProjectId = @ProjectId
END
