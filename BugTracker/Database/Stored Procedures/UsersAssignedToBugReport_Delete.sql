USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[UsersAssignedToBugReport_Delete]    Script Date: 21/12/2020 07:09:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UsersAssignedToBugReport_Delete]
	@UserId int,
	@BugReportId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM dbo.UsersAssignedToBugReport
	WHERE UserId = @UserId AND BugReportId = @BugReportId
END
GO
