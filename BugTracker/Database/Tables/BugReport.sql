USE [BugTrackerDB]
GO

/****** Object:  Table [dbo].[BugReport]    Script Date: 30/07/2020 09:49:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BugReport](
	[BugReportId] [int] IDENTITY(1,1) NOT NULL,
	[Hidden] [bit] NOT NULL,
	[CreationTime] [datetime2](7) NOT NULL,
	[Severity] [int] NOT NULL,
	[Importance] [int] NOT NULL,
	[Title] [nvarchar](max) NULL,
	[ProgramBehaviour] [nvarchar](max) NULL,
	[DetailsToReproduce] [nvarchar](max) NULL,
	[PersonReporting] [nvarchar](max) NULL,
	[ProjectId] [int] NULL,
 CONSTRAINT [PK_BugReport] PRIMARY KEY CLUSTERED 
(
	[BugReportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[BugReport]  WITH CHECK ADD  CONSTRAINT [FK_BugReport_Projects_ProjectId] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Projects] ([ProjectId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[BugReport] CHECK CONSTRAINT [FK_BugReport_Projects_ProjectId]
GO

