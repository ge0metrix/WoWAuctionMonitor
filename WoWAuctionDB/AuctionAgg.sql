CREATE TABLE [dbo].[AuctionAgg](
	[ItemID] [bigint] NOT NULL,
	[LoadDate] [datetime2](7) NOT NULL,
	[AvgBid] [bigint] NULL,
	[AvgBuy] [bigint] NULL,
	[MaxBuyAcc] [decimal](20, 0) NULL,
	[MinBuyAcc] [decimal](20, 0) NULL,
	[MaxBidAcc] [decimal](20, 0) NULL,
	[MinBidAcc] [decimal](20, 0) NULL,
	[STDBuyOut] [decimal](18, 0) NULL,
	[STDBid] [decimal](18, 0) NULL,
	[AVGBuyOut1] [bigint] NULL,
	[AVGBid1] [bigint] NULL,
	[AuctionCount] [int] NULL,
 CONSTRAINT [pk_AuctionAgg] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC,
	[LoadDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO

