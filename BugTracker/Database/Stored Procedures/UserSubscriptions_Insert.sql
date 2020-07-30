USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[UserSubscriptions_Insert]    Script Date: 30/07/2020 10:04:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Create a new user subscription to a specific 
--		BugReport for the User ID and insert into the
--		UserSubscriptions table.
-- =============================================
CREATE PROCEDURE [dbo].[UserSubscriptions_Insert]
	@UserId int,
	@BugReportId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO dbo.UserSubscriptions (UserId, BugReportId)
	VALUES (@UserId, @BugReportId)
END
GO

