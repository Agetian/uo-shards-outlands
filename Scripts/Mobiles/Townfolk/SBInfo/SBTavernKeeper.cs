using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBTavernKeeper : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBTavernKeeper()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo("Barkeep Contract", typeof(BarkeepContract), 6250, 20, 0x14F0, 0));

                Add(new BeverageBuyInfo("Jug of Cider", typeof(Jug), BeverageType.Cider, Jug.GetSBPurchaseValue(), 25, 0x9C8, 0));
                Add(new BeverageBuyInfo("Bottle of Ale", typeof(BeverageBottle), BeverageType.Ale, BeverageBottle.GetSBPurchaseValue(), 25, 0x99F, 0));
                Add(new BeverageBuyInfo("Bottle of Wine", typeof(BeverageBottle), BeverageType.Wine, BeverageBottle.GetSBPurchaseValue(), 25, 0x9C7, 0));
                Add(new BeverageBuyInfo("Bottle of Liquor", typeof(BeverageBottle), BeverageType.Liquor, BeverageBottle.GetSBPurchaseValue(), 25, 0x99B, 0));
                Add(new BeverageBuyInfo("Pitcher of Milk", typeof(Pitcher), BeverageType.Milk, Pitcher.GetSBPurchaseValue(), 25, 0x9F0, 0));
                Add(new BeverageBuyInfo("Pitcher of Ale", typeof(Pitcher), BeverageType.Ale, Pitcher.GetSBPurchaseValue(), 25, 0x1F95, 0));
                Add(new BeverageBuyInfo("Pitcher of Cider", typeof(Pitcher), BeverageType.Cider, Pitcher.GetSBPurchaseValue(), 25, 0x1F97, 0));
                Add(new BeverageBuyInfo("Pitcher of Liquor", typeof(Pitcher), BeverageType.Liquor, Pitcher.GetSBPurchaseValue(), 25, 0x1F99, 0));
                Add(new BeverageBuyInfo("Pitcher of Wine", typeof(Pitcher), BeverageType.Wine, Pitcher.GetSBPurchaseValue(), 25, 0x1F9B, 0));
                Add(new BeverageBuyInfo("Pitcher of Water", typeof(Pitcher), BeverageType.Water, Pitcher.GetSBPurchaseValue(), 25, 0x1F9D, 0));

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
                Add(typeof(BarkeepContract), BarkeepContract.GetSBSellValue());

                Add(typeof(Jug), Jug.GetSBSellValue());
                Add(typeof(BeverageBottle), BeverageBottle.GetSBSellValue());
                Add(typeof(Pitcher), Pitcher.GetSBSellValue());
                Add(typeof(GlassMug), GlassMug.GetSBSellValue());

                Add(typeof(Chessboard), Chessboard.GetSBSellValue());
                Add(typeof(CheckerBoard), CheckerBoard.GetSBSellValue());
                Add(typeof(Backgammon), Backgammon.GetSBSellValue());
                Add(typeof(Dices), Dices.GetSBSellValue());
            }
        }
	}
}