USE [BugTrackerDB]
GO
/****** Object:  StoredProcedure [dbo].[ProjectInvitations_Delete]    Script Date: 14/11/2020 09:16:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[ProjectInvitations_Delete]
	@EmailAddress nvarchar(MAX),
	@ProjectId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM dbo.ProjectInvitations
	WHERE EmailAddress = @EmailAddress AND ProjectId = @ProjectId
END
