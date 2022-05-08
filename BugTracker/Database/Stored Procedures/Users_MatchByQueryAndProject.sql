USE [BugTrackerDB]
GO
/****** Object:  StoredProcedure [dbo].[Users_MatchByQueryAndProject]    Script Date: 15/12/2020 12:53:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Users_MatchByQueryAndProject]
	@Query nvarchar(max),
	@ProjectId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT Users.UserName, Users.Email
	FROM Users
	INNER JOIN UserRoles ON UserRoles.UserId = Users.Id
	WHERE Users.UserName LIKE ('%' + @Query + '%') AND
		ProjectId = @ProjectId
END
