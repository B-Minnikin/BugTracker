USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Projects_Update]    Script Date: 30/07/2020 09:59:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Update the project speicified by the Project ID.
-- =============================================
CREATE PROCEDURE [dbo].[Projects_Update]
	@ProjectId int,
	@Name nvarchar(MAX),
	@Description nvarchar(MAX),
	@CreationTime datetime2(7),
	@LastUpdateTime datetime2(7),
	@Hidden bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE dbo.Projects
	SET Name = @Name, Description = @Description, CreationTime = @CreationTime, LastUpdateTime = @LastUpdateTime, Hidden = @Hidden
	WHERE ProjectId = @ProjectId
END
GO

