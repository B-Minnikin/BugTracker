USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[UserSubscriptions_Delete]    Script Date: 30/07/2020 10:03:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Delete the specified User ID from the list of
--		subscriptions for a BugReport.
-- =============================================
CREATE PROCEDURE [dbo].[UserSubscriptions_Delete]
	@UserId int,
	@BugReportId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM dbo.UserSubscriptions
	WHERE UserId = @UserId AND BugReportId = @BugReportId
END
GO

