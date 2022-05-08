USE [BugTrackerDB]
GO
/****** Object:  StoredProcedure [dbo].[ProjectInvitations_GetInvitationsForEmailAddress]    Script Date: 14/11/2020 09:19:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ProjectInvitations_GetInvitationsForEmailAddress]
	@EmailAddress nvarchar(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ProjectInvitations.ProjectId
	FROM ProjectInvitations WHERE ProjectInvitations.EmailAddress = @EmailAddress
END
