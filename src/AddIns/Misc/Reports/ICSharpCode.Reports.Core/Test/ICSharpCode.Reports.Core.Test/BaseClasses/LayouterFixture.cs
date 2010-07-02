/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 07.05.2010
 * Time: 20:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Test.TestHelpers;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.BaseClasses
{
	[TestFixture]
	public class LayouterForSectionFixture:ConcernOf<Layouter>
	{
		Bitmap bitmap;
		Graphics graphics;
		[Test]
		public void CanCreateSUT()
		{
			Assert.IsNotNull(Sut);
		}
		
		public override void Setup()
		{
			bitmap = new Bitmap(1,1);
			graphics = Graphics.FromImage(bitmap) ;
			BaseSection section = new BaseSection();
			Sut = new Layouter();
		}
	}
}
