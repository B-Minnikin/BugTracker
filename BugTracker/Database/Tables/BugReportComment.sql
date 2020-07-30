USE [BugTrackerDB]
GO

/****** Object:  Table [dbo].[BugReportComment]    Script Date: 30/07/2020 09:50:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BugReportComment](
	[BugReportCommentId] [int] IDENTITY(1,1) NOT NULL,
	[Author] [nvarchar](max) NULL,
	[Date] [datetime2](7) NOT NULL,
	[MainText] [nvarchar](max) NULL,
	[BugReportId] [int] NULL,
 CONSTRAINT [PK_BugReportComment] PRIMARY KEY CLUSTERED 
(
	[BugReportCommentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[BugReportComment]  WITH CHECK ADD  CONSTRAINT [FK_BugReportComment_BugReport_BugReportId] FOREIGN KEY([BugReportId])
REFERENCES [dbo].[BugReport] ([BugReportId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[BugReportComment] CHECK CONSTRAINT [FK_BugReportComment_BugReport_BugReportId]
GO

