USE [BugTrackerDB]
GO

/****** Object:  Table [dbo].[MilestoneBugReports]    Script Date: 12/02/2021 12:02:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MilestoneBugReports](
	[MilestoneBugReportId] [int] IDENTITY(1,1) NOT NULL,
	[MilestoneId] [int] NOT NULL,
	[BugReportId] [int] NOT NULL,
 CONSTRAINT [PK_MilestoneBugReports] PRIMARY KEY CLUSTERED 
(
	[MilestoneBugReportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[MilestoneBugReports]  WITH CHECK ADD  CONSTRAINT [FK_MilestoneBugReports_BugReport] FOREIGN KEY([BugReportId])
REFERENCES [dbo].[BugReport] ([BugReportId])
GO

ALTER TABLE [dbo].[MilestoneBugReports] CHECK CONSTRAINT [FK_MilestoneBugReports_BugReport]
GO

ALTER TABLE [dbo].[MilestoneBugReports]  WITH CHECK ADD  CONSTRAINT [FK_MilestoneBugReports_Milestones] FOREIGN KEY([MilestoneId])
REFERENCES [dbo].[Milestones] ([MilestoneId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[MilestoneBugReports] CHECK CONSTRAINT [FK_MilestoneBugReports_Milestones]
GO
