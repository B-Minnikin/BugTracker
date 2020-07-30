USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[BugReports_Insert]    Script Date: 30/07/2020 09:56:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Creates a BugReport entry and returns the result.
-- =============================================
CREATE PROCEDURE [dbo].[BugReports_Insert]
	@Title nvarchar(max),
	@DetailsToReproduce nvarchar(max),
	@ProgramBehaviour nvarchar(max),
	@CreationTime datetime2(7),
	@Severity int,
	@Importance int,
	@PersonReporting nvarchar(max),
	@Hidden bit,
	@ProjectId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO dbo.BugReport (Title, DetailsToReproduce, ProgramBehaviour, CreationTime, Severity, Importance, PersonReporting, Hidden, ProjectId)
	OUTPUT inserted.BugReportId
	VALUES (@Title, @DetailsToReproduce, @ProgramBehaviour, @CreationTime, @Severity, @Importance, @PersonReporting, @Hidden, @ProjectId)
END
GO

