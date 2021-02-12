USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Milestones_Update]    Script Date: 12/02/2021 11:58:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Milestones_Update]
	@MilestoneId int,
	@Title nvarchar(MAX),
	@Description nvarchar(MAX),
	@CreationDate datetime2(7),
	@DueDate datetime2(7)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE dbo.Milestones
	SET Title = @Title, Description = @Description, CreationDate = @CreationDate, DueDate = @DueDate
	OUTPUT inserted.MilestoneId
	WHERE MilestoneId = @MilestoneId
END
GO
