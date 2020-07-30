USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Projects_Insert]    Script Date: 30/07/2020 09:59:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Create and insert a new Project into the Projects
--		table.
-- =============================================
CREATE PROCEDURE [dbo].[Projects_Insert]
	@Name nvarchar(max),
	@Description nvarchar(max),
	@CreationTime datetime2(7),
	@LastUpdateTime datetime2(7),
	@Hidden bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO dbo.Projects (Name, Description, CreationTime, LastUpdateTime, Hidden)
	OUTPUT inserted.ProjectId
	VALUES (@Name, @Description, @CreationTime, @LastUpdateTime, @Hidden)
END
GO

