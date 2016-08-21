using System;
using System.Collections;
using System.Collections.Generic;
using Server;

namespace Server.Gumps
{
	public class GumpCollectionObject
	{
        public enum ObjectType
        {
            Item,
            Image            
        }       

        public int m_OffsetX = 0;
        public int m_OffsetY = 0;
        public int m_ItemID = 2500;
        public int m_Hue = 0;

        public ObjectType m_ObjectType = ObjectType.Item;

        public GumpCollectionObject(int offsetX, int offsetY, int itemID, int hue, ObjectType objectType)
		{
            m_OffsetX = offsetX;
            m_OffsetY = offsetY;
            m_ItemID = itemID;
            m_Hue = hue;

            m_ObjectType = objectType;
		}
	}   
}