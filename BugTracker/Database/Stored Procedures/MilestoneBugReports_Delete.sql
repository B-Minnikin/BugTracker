USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[MilestoneBugReports_Delete]    Script Date: 12/02/2021 12:06:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[MilestoneBugReports_Delete]
	@MilestoneId int,
	@BugReportId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM dbo.MilestoneBugReports
	WHERE MilestoneId = @MilestoneId AND BugReportId = @BugReportId
END
GO
