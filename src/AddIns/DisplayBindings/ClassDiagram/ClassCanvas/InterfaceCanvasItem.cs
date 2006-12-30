/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 28/09/2006
 * Time: 19:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;

using System.Drawing;
using System.Drawing.Drawing2D;

//using System.Reflection;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ClassDiagram
{
	public class InterfaceCanvasItem : ClassCanvasItem
	{
		static Brush innerTitlesBG = new SolidBrush(Color.FromArgb(255, 224, 255, 224));
		static Color titlesBG = Color.FromArgb(255, 192, 224, 192);
	
		public InterfaceCanvasItem (IClass ct) : base (ct) {}
	
		protected override Color TitleBackground
		{
			get { return titlesBG;}
		}
	
		protected override Brush InnerTitlesBackground
		{
			get { return innerTitlesBG; }
		}
	}

}
