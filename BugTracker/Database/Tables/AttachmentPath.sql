USE [BugTrackerDB]
GO

/****** Object:  Table [dbo].[AttachmentPath]    Script Date: 30/07/2020 09:48:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AttachmentPath](
	[AttachmentPathId] [int] IDENTITY(1,1) NOT NULL,
	[Path] [nvarchar](max) NULL,
	[CommentId] [int] NULL,
 CONSTRAINT [PK_AttachmentPath] PRIMARY KEY CLUSTERED 
(
	[AttachmentPathId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[AttachmentPath]  WITH CHECK ADD  CONSTRAINT [FK_AttachmentPath_Comment_CommentId] FOREIGN KEY([CommentId])
REFERENCES [dbo].[Comment] ([CommentId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[AttachmentPath] CHECK CONSTRAINT [FK_AttachmentPath_Comment_CommentId]
GO

