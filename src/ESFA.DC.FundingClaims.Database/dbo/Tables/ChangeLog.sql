
CREATE TABLE [dbo].[ChangeLog](
	[Id] [int] NOT NULL IDENTITY,
	[SubmissionId] [uniqueidentifier] NOT NULL,
	[UserEmailAddress] [nvarchar](320) NOT NULL,
	[UpdatedDateTimeUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_ChangeLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ChangeLog]  WITH CHECK ADD  CONSTRAINT [FK_ChangeLog_Submission] FOREIGN KEY([SubmissionId])
REFERENCES [dbo].[Submission] ([SubmissionId])
GO

ALTER TABLE [dbo].[ChangeLog] CHECK CONSTRAINT [FK_ChangeLog_Submission]
GO

