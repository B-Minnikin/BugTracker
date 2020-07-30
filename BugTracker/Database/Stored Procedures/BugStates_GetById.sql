USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[BugStates_GetById]    Script Date: 30/07/2020 09:57:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Get a BugState by its ID.
-- =============================================
CREATE PROCEDURE [dbo].[BugStates_GetById]
	@BugStateId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM BugState WHERE BugStateId = @BugStateId
END
GO

