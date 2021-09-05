USE [BugTrackerDB]
GO

/****** Object:  Table [dbo].[ActivityEvents]    Script Date: 15/08/2021 15:14:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ActivityEvents](
	[ActivityId] [int] IDENTITY(1,1) NOT NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[ProjectId] [int] NOT NULL,
	[MessageId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[BugReportId] [int] NULL,
	[AssigneeId] [int] NULL,
	[LinkedBugReportId] [int] NULL,
	[NewBugReportStateId] [int] NULL,
	[PreviousBugReportStateId] [int] NULL,
	[CommentId] [int] NULL,
	[MilestoneId] [int] NULL,
 CONSTRAINT [PK_ActivityEvents] PRIMARY KEY CLUSTERED 
(
	[ActivityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

