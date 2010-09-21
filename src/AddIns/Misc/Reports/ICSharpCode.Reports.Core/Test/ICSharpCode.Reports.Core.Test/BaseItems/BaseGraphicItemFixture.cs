// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;
using ICSharpCode.Reports.Core.Exporter;

namespace ICSharpCode.Reports.Core.Test.BaseItems
{
	[TestFixture]
	public class BaseGraphicItemFixture
	{
		[Test]
		public void DefaultConstructur()
		{
			BaseGraphicItem bgi = new BaseGraphicItem();
			Assert.IsNotNull(bgi,"BaseGraphicItem should not be null");
		}
		
		[Test]
		public void DefaultSettings()
		{
			BaseGraphicItem bgi = new BaseGraphicItem();
			Assert.AreEqual(1,bgi.Thickness,"Thickness should be '1'");
			Assert.AreEqual(System.Drawing.Drawing2D.DashStyle.Solid,bgi.DashStyle,
			                "dashStyle should be 'Solid'");
			Assert.IsFalse(bgi.DrawBorder,"DrawBorder should be 'false'");
		}
		
		[Test] 
		public void ToStringOverride ()
		{
			BaseGraphicItem bgi = new BaseGraphicItem();
			Assert.AreEqual ("BaseGraphicItem",bgi.ToString(),"ToString override");
		}
		
	}
}
