USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Projects_Delete]    Script Date: 30/07/2020 09:59:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Delete the project from the Projects table 
--		specified by the Project ID.
-- =============================================
CREATE PROCEDURE [dbo].[Projects_Delete]
	@ProjectId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM dbo.Projects
	WHERE ProjectId = @ProjectId
END
GO

