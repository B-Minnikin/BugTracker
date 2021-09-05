USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Comments_Update]    Script Date: 30/07/2020 09:58:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Update the comment specified by the comment ID.
-- =============================================
CREATE PROCEDURE [dbo].[Comments_Update]
	@CommentId int,
	@Author nvarchar(MAX),
	@MainText nvarchar(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE dbo.Comment
	SET Author = @Author, MainText = @MainText
	WHERE CommentId = @CommentId
END
GO

