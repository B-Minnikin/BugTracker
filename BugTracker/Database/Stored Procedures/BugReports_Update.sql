USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[BugReports_Update]    Script Date: 30/07/2020 09:56:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Updates the specified BugReport.
-- =============================================
CREATE PROCEDURE [dbo].[BugReports_Update]
	@BugReportId int,
	@Title nvarchar(MAX),
	@ProgramBehaviour nvarchar(MAX),
	@DetailsToReproduce nvarchar(MAX),
	@Severity int,
	@Importance int,
	@Hidden bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE dbo.BugReport
	SET Title = @Title, ProgramBehaviour = @ProgramBehaviour, DetailsToReproduce = @DetailsToReproduce, Severity = @Severity, Importance = @Importance, Hidden = @Hidden
	WHERE BugReportId = @BugReportId
END
GO

