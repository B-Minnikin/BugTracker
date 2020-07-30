USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Projects_GetById]    Script Date: 30/07/2020 09:59:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Return all projects which correspond to the
--		specified ID.
-- =============================================
CREATE PROCEDURE [dbo].[Projects_GetById]
	@ProjectId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.Projects WHERE ProjectId = @ProjectId
END
GO

