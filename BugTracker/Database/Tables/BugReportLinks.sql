USE [BugTrackerDB]
GO

/****** Object:  Table [dbo].[BugReportLinks]    Script Date: 12/01/2021 13:31:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BugReportLinks](
	[BugReportId] [int] NOT NULL,
	[LinkToBugReportId] [int] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[BugReportLinks]  WITH CHECK ADD  CONSTRAINT [FK_BugReportLinks_BugReport] FOREIGN KEY([BugReportId])
REFERENCES [dbo].[BugReport] ([BugReportId])
GO

ALTER TABLE [dbo].[BugReportLinks] CHECK CONSTRAINT [FK_BugReportLinks_BugReport]
GO

ALTER TABLE [dbo].[BugReportLinks]  WITH CHECK ADD  CONSTRAINT [FK_BugReportLinks_BugReport1] FOREIGN KEY([LinkToBugReportId])
REFERENCES [dbo].[BugReport] ([BugReportId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[BugReportLinks] CHECK CONSTRAINT [FK_BugReportLinks_BugReport1]
GO


