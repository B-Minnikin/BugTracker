USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[BugStates_GetAll]    Script Date: 30/07/2020 09:57:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Get all BugStates of a specified BugReport ID.
-- =============================================
CREATE PROCEDURE [dbo].[BugStates_GetAll] 
	@BugReportId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.BugState WHERE BugReportId = @BugReportId
END
GO

