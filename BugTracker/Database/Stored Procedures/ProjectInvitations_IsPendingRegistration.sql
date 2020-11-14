USE [BugTrackerDB]
GO
/****** Object:  StoredProcedure [dbo].[ProjectInvitations_IsPendingRegistration]    Script Date: 14/11/2020 09:21:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[ProjectInvitations_IsPendingRegistration]
	@EmailAddress nvarchar(MAX),
	@ProjectId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT
		CASE WHEN EXISTS
			(SELECT ProjectId
			FROM ProjectInvitations as pInvite
			WHERE pInvite.EmailAddress = @EmailAddress AND pInvite.ProjectId = @ProjectId)
		THEN 1
		ELSE 0
		END
	FROM dbo.ProjectInvitations
END
