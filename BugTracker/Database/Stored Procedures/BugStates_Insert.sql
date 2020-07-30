USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[BugStates_Insert]    Script Date: 30/07/2020 09:57:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Create a new BugState and insert it into the
--		BugState table. Returns the created entry.
-- =============================================
CREATE PROCEDURE [dbo].[BugStates_Insert]
	@Time datetime2(7),
	@StateType int,
	@Author nvarchar(max),
	@BugReportId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO dbo.BugState (Author, StateType, Time, BugReportId)
	OUTPUT inserted.BugStateId
	VALUES (@Author, @StateType, @Time, @BugReportId)
END
GO

