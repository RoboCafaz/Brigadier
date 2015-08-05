GO
SET IDENTITY_INSERT [dbo].[LinkType] ON 
INSERT [dbo].[LinkType] ([Id], [Type]) VALUES (1, N'Direct')
INSERT [dbo].[LinkType] ([Id], [Type]) VALUES (2, N'NP')
INSERT [dbo].[LinkType] ([Id], [Type]) VALUES (3, N'Archive')
INSERT [dbo].[LinkType] ([Id], [Type]) VALUES (4, N'Other')
SET IDENTITY_INSERT [dbo].[LinkType] OFF
GO
SET IDENTITY_INSERT [dbo].[WatchedSub] ON 
INSERT [dbo].[WatchedSub] ([Id], [Url]) VALUES (1, N'shitredditsays')
INSERT [dbo].[WatchedSub] ([Id], [Url]) VALUES (2, N'SubredditDrama')
INSERT [dbo].[WatchedSub] ([Id], [Url]) VALUES (3, N'kotakuinaction')
INSERT [dbo].[WatchedSub] ([Id], [Url]) VALUES (4, N'kiachatroom')
INSERT [dbo].[WatchedSub] ([Id], [Url]) VALUES (5, N'gamerghazi')
INSERT [dbo].[WatchedSub] ([Id], [Url]) VALUES (6, N'bestof')
INSERT [dbo].[WatchedSub] ([Id], [Url]) VALUES (7, N'worstof')
INSERT [dbo].[WatchedSub] ([Id], [Url]) VALUES (8, N'CircleBroke')
INSERT [dbo].[WatchedSub] ([Id], [Url]) VALUES (9, N'SubredditCancer')
SET IDENTITY_INSERT [dbo].[WatchedSub] OFF
GO
