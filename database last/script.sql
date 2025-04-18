USE [IsTakipDb]
GO
/****** Object:  Table [dbo].[Birimler]    Script Date: 16.03.2025 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Birimler](
	[birimId] [int] IDENTITY(1,1) NOT NULL,
	[birimAd] [nvarchar](50) NULL,
 CONSTRAINT [PK_Birimler] PRIMARY KEY CLUSTERED 
(
	[birimId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Durumlar]    Script Date: 16.03.2025 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Durumlar](
	[durumId] [int] IDENTITY(1,1) NOT NULL,
	[durumAd] [nvarchar](50) NULL,
 CONSTRAINT [PK_Durumlar] PRIMARY KEY CLUSTERED 
(
	[durumId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Isler]    Script Date: 16.03.2025 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Isler](
	[isId] [int] IDENTITY(1,1) NOT NULL,
	[isBaslik] [nvarchar](max) NULL,
	[isAciklama] [nvarchar](max) NULL,
	[isPersonelId] [int] NULL,
	[iletilenTarih] [datetime] NULL,
	[yapilanTarih] [datetime] NULL,
	[isDurumId] [int] NULL,
	[isYorum] [nvarchar](max) NULL,
	[isOkunma] [bit] NULL,
 CONSTRAINT [PK_Isler] PRIMARY KEY CLUSTERED 
(
	[isId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Personeller]    Script Date: 16.03.2025 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Personeller](
	[personelId] [int] IDENTITY(1,1) NOT NULL,
	[personelAdSoyad] [nvarchar](50) NULL,
	[personelKullaniciAd] [nvarchar](50) NULL,
	[personelParola] [nvarchar](50) NULL,
	[personelBirimId] [int] NULL,
	[personelYetkiTurId] [int] NULL,
	[personelTelefonNo] [nvarchar](10) NULL,
 CONSTRAINT [PK_Personeller] PRIMARY KEY CLUSTERED 
(
	[personelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[YetkiTurler]    Script Date: 16.03.2025 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[YetkiTurler](
	[yetkiTurId] [int] IDENTITY(1,1) NOT NULL,
	[yetkiTurAd] [nvarchar](50) NULL,
 CONSTRAINT [PK_YetkiTurler] PRIMARY KEY CLUSTERED 
(
	[yetkiTurId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Isler]  WITH CHECK ADD  CONSTRAINT [FK_Isler_Durumlar] FOREIGN KEY([isDurumId])
REFERENCES [dbo].[Durumlar] ([durumId])
GO
ALTER TABLE [dbo].[Isler] CHECK CONSTRAINT [FK_Isler_Durumlar]
GO
ALTER TABLE [dbo].[Isler]  WITH CHECK ADD  CONSTRAINT [FK_Isler_Personeller] FOREIGN KEY([isPersonelId])
REFERENCES [dbo].[Personeller] ([personelId])
GO
ALTER TABLE [dbo].[Isler] CHECK CONSTRAINT [FK_Isler_Personeller]
GO
ALTER TABLE [dbo].[Personeller]  WITH CHECK ADD  CONSTRAINT [FK_Personeller_Birimler] FOREIGN KEY([personelBirimId])
REFERENCES [dbo].[Birimler] ([birimId])
GO
ALTER TABLE [dbo].[Personeller] CHECK CONSTRAINT [FK_Personeller_Birimler]
GO
ALTER TABLE [dbo].[Personeller]  WITH CHECK ADD  CONSTRAINT [FK_Personeller_YetkiTurler] FOREIGN KEY([personelYetkiTurId])
REFERENCES [dbo].[YetkiTurler] ([yetkiTurId])
GO
ALTER TABLE [dbo].[Personeller] CHECK CONSTRAINT [FK_Personeller_YetkiTurler]
GO
