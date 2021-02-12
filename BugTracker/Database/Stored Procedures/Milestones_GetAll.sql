USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Milestones_GetAll]    Script Date: 12/02/2021 12:00:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Milestones_GetAll] 
	@ProjectId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.Milestones WHERE ProjectId = @ProjectId
END
GO
