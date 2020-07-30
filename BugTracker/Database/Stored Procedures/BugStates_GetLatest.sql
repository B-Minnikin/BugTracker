USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[BugStates_GetLatest]    Script Date: 30/07/2020 09:57:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Gets the most recent BugState corresponding to
--		a BugReport.
-- =============================================
CREATE PROCEDURE [dbo].[BugStates_GetLatest]
	@BugReportId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.BugState
	WHERE BugReportId = @BugReportId
	ORDER BY Time DESC
END
GO

