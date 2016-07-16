using System;
using System.Collections;

namespace Server.Items
{
	public class CheckerBoard : BaseBoard
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		[Constructable]
		public CheckerBoard() : base( 0xFA6 )
		{
            Name = "checker board";
		}

		public override void CreatePieces()
		{
			for ( int i = 0; i < 4; i++ )
			{
				CreatePiece( new PieceWhiteChecker( this ), ( 50 * i ) + 45, 25 );
				CreatePiece( new PieceWhiteChecker( this ), ( 50 * i ) + 70, 50 );
				CreatePiece( new PieceWhiteChecker( this ), ( 50 * i ) + 45, 75 );
				CreatePiece( new PieceBlackChecker( this ), ( 50 * i ) + 70, 150 );
				CreatePiece( new PieceBlackChecker( this ), ( 50 * i ) + 45, 175 );
				CreatePiece( new PieceBlackChecker( this ), ( 50 * i ) + 70, 200 );
			}
		}

		public CheckerBoard( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}