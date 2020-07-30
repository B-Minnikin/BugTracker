USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Roles_Update]    Script Date: 30/07/2020 10:00:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Update a Role by the specified ID.
-- =============================================
CREATE PROCEDURE [dbo].[Roles_Update]
	@RoleId int,
	@Name nvarchar(MAX),
	@NormalizedName nvarchar(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE dbo.Roles
	SET RoleId = @RoleId, Name = @Name, NormalizedName = @NormalizedName
	WHERE RoleId = @RoleId
END
GO

