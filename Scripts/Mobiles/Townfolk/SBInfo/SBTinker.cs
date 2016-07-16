using System; 
using System.Collections.Generic; 
using Server.Items; 

namespace Server.Mobiles 
{ 
	public class SBTinker: SBInfo 
	{ 
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBTinker() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{
                Add(new GenericBuyInfo("Pickaxe", typeof(Pickaxe), Pickaxe.GetSBPurchaseValue(), 50, 0xE86, 0));
                Add(new GenericBuyInfo("Shovel", typeof(Shovel), Shovel.GetSBPurchaseValue(), 50, 0xF39, 0));
                Add(new GenericBuyInfo("Hatchet", typeof(Hatchet), Hatchet.GetSBPurchaseValue(), 50, 0xF43, 0));

                Add(new GenericBuyInfo("Lockpick", typeof(Lockpick), Lockpick.GetSBPurchaseValue(), 250, 0x14FC, 0));
                Add(new GenericBuyInfo("Scissors", typeof(Scissors), Scissors.GetSBPurchaseValue(), 50, 0xF9F, 0));

                Add(new GenericBuyInfo("Tongs", typeof(Tongs), Tongs.GetSBPurchaseValue(), 50, 0xFBB, 0));
                Add(new GenericBuyInfo("SledgeHammer", typeof(SledgeHammer), SledgeHammer.GetSBPurchaseValue(), 50, 0xFB5, 0));
                Add(new GenericBuyInfo("SmithHammer", typeof(SmithHammer), SmithHammer.GetSBPurchaseValue(), 50, 0x13E3, 0));
                Add(new GenericBuyInfo("MortarPestle", typeof(MortarPestle), MortarPestle.GetSBPurchaseValue(), 50, 0xE9B, 0));
                Add(new GenericBuyInfo("TinkerTools", typeof(TinkerTools), TinkerTools.GetSBPurchaseValue(), 50, 0x1EB8, 0));
                Add(new GenericBuyInfo("SewingKit", typeof(SewingKit), SewingKit.GetSBPurchaseValue(), 50, 0xF9D, 0));
                Add(new GenericBuyInfo("Skillet", typeof(Skillet), Skillet.GetSBPurchaseValue(), 50, 0x97F, 0));
                Add(new GenericBuyInfo("RollingPin", typeof(RollingPin), RollingPin.GetSBPurchaseValue(), 50, 0x1043, 0));
                Add(new GenericBuyInfo("MapmakersPen", typeof(MapmakersPen), MapmakersPen.GetSBPurchaseValue(), 50, 0x0FBF, 0));
                Add(new GenericBuyInfo("ScribesPen", typeof(ScribesPen), ScribesPen.GetSBPurchaseValue(), 50, 0x0FBF, 0));
                Add(new GenericBuyInfo("Scorp", typeof(Scorp), Scorp.GetSBPurchaseValue(), 50, 0x10E7, 0));
                Add(new GenericBuyInfo("DrawKnife", typeof(DrawKnife), DrawKnife.GetSBPurchaseValue(), 50, 0x10E4, 0));
                Add(new GenericBuyInfo("Saw", typeof(Saw), Saw.GetSBPurchaseValue(), 50, 0x1034, 0));
                Add(new GenericBuyInfo("DovetailSaw", typeof(DovetailSaw), DovetailSaw.GetSBPurchaseValue(), 50, 0x1028, 0));
                Add(new GenericBuyInfo("Froe", typeof(Froe), Froe.GetSBPurchaseValue(), 50, 0x10E5, 0));
                Add(new GenericBuyInfo("Hammer", typeof(Hammer), Hammer.GetSBPurchaseValue(), 50, 0x102A, 0));
                Add(new GenericBuyInfo("Inshave", typeof(Inshave), Inshave.GetSBPurchaseValue(), 50, 0x10E6, 0));
                Add(new GenericBuyInfo("JointingPlane", typeof(JointingPlane), JointingPlane.GetSBPurchaseValue(), 50, 0x1030, 0));
                Add(new GenericBuyInfo("WoodenPlane", typeof(WoodenPlane), WoodenPlane.GetSBPurchaseValue(), 50, 0x102C, 0));
                Add(new GenericBuyInfo("SmoothingPlane", typeof(SmoothingPlane), SmoothingPlane.GetSBPurchaseValue(), 50, 0x1032, 0));
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo()
            {
                Add(typeof(Pickaxe), Pickaxe.GetSBSellValue());
                Add(typeof(Shovel), Shovel.GetSBSellValue());

                Add(typeof(Lockpick), Lockpick.GetSBSellValue());
                Add(typeof(Scissors), Scissors.GetSBSellValue());

                Add(typeof(Tongs), Tongs.GetSBSellValue());
                Add(typeof(SledgeHammer), SledgeHammer.GetSBSellValue());
                Add(typeof(SmithHammer), SmithHammer.GetSBSellValue());
                Add(typeof(MortarPestle), MortarPestle.GetSBSellValue());
                Add(typeof(TinkerTools), TinkerTools.GetSBSellValue());
                Add(typeof(SewingKit), SewingKit.GetSBSellValue());
                Add(typeof(Skillet), Skillet.GetSBSellValue());
                Add(typeof(RollingPin), RollingPin.GetSBSellValue());
                Add(typeof(MapmakersPen), MapmakersPen.GetSBSellValue());
                Add(typeof(ScribesPen), ScribesPen.GetSBSellValue());
                Add(typeof(Scorp), Scorp.GetSBSellValue());
                Add(typeof(DrawKnife), DrawKnife.GetSBSellValue());
                Add(typeof(Saw), Saw.GetSBSellValue());
                Add(typeof(DovetailSaw), DovetailSaw.GetSBSellValue());
                Add(typeof(Froe), Froe.GetSBSellValue());
                Add(typeof(Hammer), Hammer.GetSBSellValue());
                Add(typeof(Inshave), Inshave.GetSBSellValue());
                Add(typeof(JointingPlane), JointingPlane.GetSBSellValue());
                Add(typeof(WoodenPlane), WoodenPlane.GetSBSellValue());
                Add(typeof(SmoothingPlane), SmoothingPlane.GetSBSellValue());
			} 
		} 
	} 
}