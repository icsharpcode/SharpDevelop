/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 20.10.2004
 * Time: 22:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;

namespace Ruler
{
	/// <summary>
	/// Description of GdiHelper.
	/// </summary>
	public class GdiHelper
	{
		public GdiHelper()
		{
		}
		
		public static SizeF CalcReciprocals(Graphics g)	
		{
			switch(g.PageUnit)	
			{	
				case GraphicsUnit.World:	
				case GraphicsUnit.Pixel:
					
					return new SizeF(1f,1f);
					
				case GraphicsUnit.Inch:
					
					return new SizeF(1f/g.DpiX,1f/g.DpiY);
					
				case GraphicsUnit.Millimeter:
					
					return new SizeF(25.4f/g.DpiX,25.4f/g.DpiY);
					
				case GraphicsUnit.Point:
					
					return new SizeF(72f/g.DpiX,72f/g.DpiY);
					
				case GraphicsUnit.Display:
					
					return new SizeF(75f/g.DpiX,75f/g.DpiY);
					
				case GraphicsUnit.Document:
					
					return new SizeF(300f/g.DpiX,300f/g.DpiY);
					
			}
			return new SizeF(10,10);//never gets here...
		}
	}
}
