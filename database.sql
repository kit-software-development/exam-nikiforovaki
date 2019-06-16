USE [test_minesweeper_db]
GO
/****** Object:  Table [dbo].[Result]    Script Date: 16.06.2019 0:24:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Result](
	[Id] [numeric](18, 0) NOT NULL,
	[Time] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Result] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
