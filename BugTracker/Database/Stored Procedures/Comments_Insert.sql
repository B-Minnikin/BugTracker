USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Comments_Insert]    Script Date: 30/07/2020 09:58:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Create a new comment, insert into 
--		BugReportComment table, and then return the newly created
--		comment.
-- =============================================
CREATE PROCEDURE [dbo].[Comments_Insert]
	@Author nvarchar(max),
	@MainText nvarchar(max),
	@Date datetime2(7),
	@BugReportId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO dbo.BugReportComment (Author, MainText, Date, BugReportId)
	OUTPUT inserted.BugReportCommentId
	VALUES (@Author, @MainText, @Date, @BugReportId)
END
GO

