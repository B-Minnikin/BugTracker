USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Milestones_Insert]    Script Date: 12/02/2021 11:52:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Milestones_Insert]
	@ProjectId int,
	@Title nvarchar(max),
	@Description nvarchar(max),
	@CreationDate datetime2(7),
	@DueDate datetime2(7)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO dbo.Milestones(ProjectId, Title, Description, CreationDate, DueDate)
	OUTPUT inserted.MilestoneId
	VALUES (@ProjectId, @Title, @Description, @CreationDate, @DueDate)
END
GO
