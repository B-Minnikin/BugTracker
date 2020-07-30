USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Projects_GetAll]    Script Date: 30/07/2020 09:59:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Returns a collection of all projects.
-- =============================================
CREATE PROCEDURE [dbo].[Projects_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.Projects
END
GO

