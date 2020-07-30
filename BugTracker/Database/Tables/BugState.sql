USE [BugTrackerDB]
GO

/****** Object:  Table [dbo].[BugState]    Script Date: 30/07/2020 09:50:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BugState](
	[BugStateId] [int] IDENTITY(1,1) NOT NULL,
	[Time] [datetime2](7) NOT NULL,
	[Author] [nvarchar](max) NULL,
	[StateType] [int] NOT NULL,
	[BugReportId] [int] NOT NULL,
 CONSTRAINT [PK_BugState] PRIMARY KEY CLUSTERED 
(
	[BugStateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[BugState]  WITH CHECK ADD  CONSTRAINT [FK_BugState_BugReport_BugReportId] FOREIGN KEY([BugReportId])
REFERENCES [dbo].[BugReport] ([BugReportId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[BugState] CHECK CONSTRAINT [FK_BugState_BugReport_BugReportId]
GO

