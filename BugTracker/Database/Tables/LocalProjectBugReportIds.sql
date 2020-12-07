USE [BugTrackerDB]
GO

/****** Object:  Table [dbo].[LocalProjectBugReportIds]    Script Date: 07/12/2020 00:08:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LocalProjectBugReportIds](
	[ProjectId] [int] NOT NULL,
	[NextFreeId] [int] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[LocalProjectBugReportIds] ADD  CONSTRAINT [DF_ProjectsNextFreeBugReportId_NextFreeId]  DEFAULT ((1)) FOR [NextFreeId]
GO

ALTER TABLE [dbo].[LocalProjectBugReportIds]  WITH CHECK ADD  CONSTRAINT [FK_ProjectsNextFreeBugReportId_Projects] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Projects] ([ProjectId])
GO

ALTER TABLE [dbo].[LocalProjectBugReportIds] CHECK CONSTRAINT [FK_ProjectsNextFreeBugReportId_Projects]
GO