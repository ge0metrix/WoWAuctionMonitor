CREATE TABLE [dbo].[AuctionRaw]
(
	[RowNum] [bigint] IDENTITY(1,1) NOT NULL,
	[Id] [int] NULL,
	[ItemID] [bigint] NULL,
	[Owner] [nvarchar](100) NULL,
	[Bid] [bigint] NULL,
	[BuyOut] [bigint] NULL,
	[Quantity] [int] NULL,
	[TimeLeft] [nvarchar](50) NULL,
	[LoadDate] [datetime2](7) NULL,
 CONSTRAINT [PK_Auctions] PRIMARY KEY CLUSTERED 
(
	[RowNum] ASC
)
)
GO

CREATE INDEX [IX_AuctionRaw_ItemID] ON [dbo].[AuctionRaw] ([ItemID])

GO

CREATE INDEX [IX_AuctionRaw_LoadDate] ON [dbo].[AuctionRaw] ([LoadDate])
