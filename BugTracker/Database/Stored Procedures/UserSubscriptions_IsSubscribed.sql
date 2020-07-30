USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[UserSubscriptions_IsSubscribed]    Script Date: 30/07/2020 10:04:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Return boolean value checking whether the
--		specified User ID is subscribed to a BugReport.
-- =============================================
CREATE PROCEDURE [dbo].[UserSubscriptions_IsSubscribed]
	@UserId int,
	@BugReportId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT
		CASE WHEN EXISTS
			(SELECT Id
			FROM UserSubscriptions as us
			WHERE us.UserId = @UserId AND us.BugReportId = @BugReportId)
		THEN 1
		ELSE 0
		END
	FROM dbo.UserSubscriptions
END
GO

