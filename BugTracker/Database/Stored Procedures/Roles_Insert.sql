USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Roles_Insert]    Script Date: 30/07/2020 10:00:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Create a role and insert into Role table.
-- =============================================
CREATE PROCEDURE [dbo].[Roles_Insert]
	@Name nvarchar(MAX),
	@NormalizedName nvarchar(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO dbo.Roles (Name, NormalizedName)
	VALUES (@Name, @NormalizedName)
END
GO

