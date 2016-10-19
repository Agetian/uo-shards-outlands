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
            Image,
            Background
        }       

        public int m_OffsetX = 0;
        public int m_OffsetY = 0;       
        public int m_ItemID = 2500;
        public int m_Hue = 0;
        public int m_Width = 0;
        public int m_Height = 0;

        public ObjectType m_ObjectType = ObjectType.Item;

        public GumpCollectionObject(int offsetX, int offsetY, int itemID, int hue, int width, int height, ObjectType objectType)
		{
            m_OffsetX = offsetX;
            m_OffsetY = offsetY;          
            m_ItemID = itemID;
            m_Hue = hue;
            m_Width = width;
            m_Height = height;

            m_ObjectType = objectType;
		}
	}   
}