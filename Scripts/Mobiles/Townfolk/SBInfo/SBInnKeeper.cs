using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBInnKeeper : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBInnKeeper()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo("Vendor Rental Contract", typeof(VendorRentalContract), VendorRentalContract.GetSBPurchaseValue(), 25, 0x14F0, 0));

                Add(new GenericBuyInfo("Lantern", typeof(Lantern), Lantern.GetSBPurchaseValue(), 25, 0xA25, 0));
                Add(new GenericBuyInfo("Torch", typeof(Torch), Torch.GetSBPurchaseValue(), 25, 0xF6B, 0));
                Add(new GenericBuyInfo("Candle", typeof(Candle), Candle.GetSBPurchaseValue(), 25, 0xA28, 0));
                Add(new GenericBuyInfo("Backpack", typeof(Backpack), Backpack.GetSBPurchaseValue(), 25, 0x9B2, 0));
                Add(new GenericBuyInfo("Bag", typeof(Bag), Bag.GetSBPurchaseValue(), 25, 0xE76, 0));
                Add(new GenericBuyInfo("Pouch", typeof(Pouch), Pouch.GetSBPurchaseValue(), 25, 0xE79, 0));

                Add(new GenericBuyInfo("Chess Board", typeof(Chessboard), Chessboard.GetSBPurchaseValue(), 25, 0xFA6, 0));
                Add(new GenericBuyInfo("Checker Board", typeof(CheckerBoard), CheckerBoard.GetSBPurchaseValue(), 25, 0xFA6, 0));
                Add(new GenericBuyInfo("Backgammon", typeof(Backgammon), Backgammon.GetSBPurchaseValue(), 25, 0xE1C, 0));
                Add(new GenericBuyInfo("Dice", typeof(Dices), Dices.GetSBPurchaseValue(), 25, 0xFA7, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
                Add(typeof(VendorRentalContract), VendorRentalContract.GetSBSellValue());

                Add(typeof(Torch), Torch.GetSBSellValue());
                Add(typeof(Candle), Candle.GetSBSellValue());
                Add(typeof(Backpack), Backpack.GetSBSellValue());
                Add(typeof(Bag), Bag.GetSBSellValue());
                Add(typeof(Pouch), Pouch.GetSBSellValue());

                Add(typeof(Chessboard), Chessboard.GetSBSellValue());
                Add(typeof(CheckerBoard), CheckerBoard.GetSBSellValue());
                Add(typeof(Backgammon), Backgammon.GetSBSellValue());
                Add(typeof(Dices), Dices.GetSBSellValue());
			}
		}
	}
}