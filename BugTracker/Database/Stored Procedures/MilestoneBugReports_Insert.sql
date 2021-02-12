USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[MilestoneBugReports_Insert]    Script Date: 12/02/2021 12:05:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[MilestoneBugReports_Insert]
	@MilestoneId int,
	@BugReportId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF NOT EXISTS (SELECT * FROM dbo.MilestoneBugReports
		WHERE BugReportId = @BugReportId AND MilestoneId = @MilestoneId)
    -- Insert statements for procedure here
	BEGIN
		INSERT INTO dbo.MilestoneBugReports(MilestoneId, BugReportId)
		VALUES (@MilestoneId, @BugReportId)
	END
END
GO
