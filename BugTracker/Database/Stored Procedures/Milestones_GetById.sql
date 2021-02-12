USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Milestones_GetById]    Script Date: 12/02/2021 11:54:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Milestones_GetById]
	@MilestoneId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.Milestones WHERE MilestoneId = @MilestoneId
END
GO
