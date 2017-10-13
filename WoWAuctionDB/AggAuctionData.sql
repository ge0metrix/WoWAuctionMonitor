CREATE PROCEDURE [dbo].[AggAuctionData]
AS
BEGIN 

SET NOCOUNT ON

DELETE FROM AuctionAgg WHERE LoadDate = (SELECT MAX(LoadDate) FROM [AuctionRaw]);

WITH stdevs AS (
	SELECT
	LoadDate, ItemID, 
	CAST((COALESCE(STDEV(BID/Quantity),0)) AS DECIMAL(18,0)) AS STDBID, 
	CAST((COALESCE(STDEV(BuyOut/Quantity),0)) AS DECIMAL(18,0))  AS STDBuyOut,
	COALESCE(AVG(BID/Quantity),0) AS AvgBid1, 
	COALESCE(AVG(BuyOut/Quantity),0) AS AvgBuyOut1,
	COUNT(*) AS AuctionCount
	FROM [dbo].[AuctionRaw]
	WHERE LoadDate = (SELECT MAX(LoadDate) FROM [AuctionRaw])
	GROUP BY LoadDate, ItemID
), CleanedValues  AS (
SELECT 
	A.*
	,A.BID/A.Quantity AS BidPerItem
	,A.BuyOut/A.Quantity AS BuyOutPerItem
	,STDBID
	,AvgBid1
	,STDBID + AvgBid1 AS MaxBidAcc
	,AvgBid1 - STDBID AS MinBidAcc
	,STDBuyOut
	,AvgBuyOut1
	,STDBuyOut + AvgBuyOut1 AS MaxBuyAcc
	,AvgBuyOut1 - STDBuyOut  AS MinBuyAcc
	,AuctionCount
	FROM dbo.[AuctionRaw] AS A
	INNER JOIN stdevs AS S ON A.ItemID = S.ItemID AND A.LoadDate = S.LoadDate
	WHERE A.LoadDate = (SELECT MAX(LoadDate) FROM [AuctionRaw])
), CalcClean AS (
	SELECT * 	
	,CASE WHEN BidPerItem >= MinBidAcc AND BidPerItem <= MaxBidAcc THEN BidPerItem END AS BidClean
	,CASE WHEN BuyOutPerItem >= MinBuyAcc AND BuyOutPerItem <= MaxBuyAcc THEN BuyOutPerItem END AS BuyClean
	FROM CleanedValues
)
INSERT INTO AuctionAgg
SELECT ItemID, LoadDate, CASE WHEN AuctionCount = 1 THEN AVGBid1 ELSE AVG(BIDClean) END AS AvgBid, CASE WHEN AuctionCount = 1 THEN AVGBuyOut1 ELSE AVG(BuyClean) END AS AvgBuy, MaxBuyAcc, MinBuyAcc, MaxBidAcc, MinBidAcc,STDBuyOut, STDBid, AVGBuyOut1, AVGBid1, AuctionCount
FROM CalcClean
WHERE LoadDate = (SELECT MAX(LoadDate) FROM [AuctionRaw])
GROUP BY ItemID, LoadDate, MaxBuyAcc, MinBuyAcc, MaxBidAcc, MinBidAcc,STDBuyOut, STDBid, AVGBuyOut1, AVGBid1, AuctionCount

END