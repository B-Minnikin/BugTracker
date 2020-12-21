USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[UsersAssignedToBugReport_Insert]    Script Date: 21/12/2020 08:14:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UsersAssignedToBugReport_Insert]
	@UserId int,
	@BugReportId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF NOT EXISTS (SELECT * FROM dbo.UsersAssignedToBugReport
		WHERE UserId = @UserId AND BugReportId = @BugReportId)
	BEGIN
		-- Insert statements for procedure here
		INSERT INTO dbo.UsersAssignedToBugReport (UserId, BugReportId)
		VALUES (@UserId, @BugReportId)
	END
END
GO
