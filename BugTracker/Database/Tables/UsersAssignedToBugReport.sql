USE [BugTrackerDB]
GO

/****** Object:  Table [dbo].[UsersAssignedToBugReport]    Script Date: 21/12/2020 07:06:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UsersAssignedToBugReport](
	[UserId] [int] NOT NULL,
	[BugReportId] [int] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UsersAssignedToBugReport]  WITH CHECK ADD  CONSTRAINT [FK_UsersAssignedToBugReport_BugReport] FOREIGN KEY([BugReportId])
REFERENCES [dbo].[BugReport] ([BugReportId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[UsersAssignedToBugReport] CHECK CONSTRAINT [FK_UsersAssignedToBugReport_BugReport]
GO

ALTER TABLE [dbo].[UsersAssignedToBugReport]  WITH CHECK ADD  CONSTRAINT [FK_UsersAssignedToBugReport_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[UsersAssignedToBugReport] CHECK CONSTRAINT [FK_UsersAssignedToBugReport_Users]
GO


