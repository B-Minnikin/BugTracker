USE [BugTrackerDB]
GO
/****** Object:  StoredProcedure [dbo].[ProjectInvitations_Insert]    Script Date: 14/11/2020 09:20:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[ProjectInvitations_Insert]
	@EmailAddress nvarchar(MAX),
	@ProjectId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO dbo.ProjectInvitations(EmailAddress, ProjectId)
	VALUES (@EmailAddress, @ProjectId)
END
