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
using ICSharpCode.Reports.Core.Test.TestHelpers;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.BaseClasses
{
	[TestFixture]
	public class LayouterForSectionFixture:ConcernOf<Layouter>
	{
		Bitmap bitmap;
		Graphics graphics;
		BaseSection section;
		
		[Test]
		public void CanCreateSUT()
		{
			Assert.IsNotNull(Sut);
		}
		
		
		[Test]
		public void Layouter_Return_SectionSize_If_No_Item_Is_CanGrow_Shrink ()
		{
			Rectangle sectionRect = new Rectangle(section.Location,section.Size);
			BaseTextItem item = new BaseTextItem()
			{
				Location = new Point(5,5),
				Size = new Size (100,20),
				Text = "hi i'm the text"
			};
			
			section.Items.Add(item);
			Rectangle resultRec = Sut.Layout(this.graphics,section);
			BaseReportItem resultItem = section.Items[0];
			
			Assert.That(resultItem.Size.Height == item.Size.Height);
			Assert.AreEqual(sectionRect,resultRec);
		}
		
		
		[Test]
		public void Layouter_Return_SectionSize_If_Items_Fit_In_Section ()
		{
			Rectangle sectionRect = new Rectangle(section.Location,section.Size);
			BaseTextItem item = CreateSmallItem();
			section.Items.Add(item);
			Rectangle resultRec = Sut.Layout(this.graphics,section);
			BaseReportItem resultItem = section.Items[0];
			
			Assert.That(resultItem.Size.Height == item.Size.Height);	
			Assert.AreEqual(sectionRect,resultRec);
		}
		
		
		[Test]
		public void Layouter_Extend_SectionSize_If_Items_Not_Fit_In_Section ()
		{
			Rectangle sectionRect = new Rectangle(section.Location,section.Size);
			
			BaseTextItem item = CreateBigItem();
			
			section.Items.Add(item);
			
			Rectangle resultRec = Sut.Layout(this.graphics,section);
			BaseReportItem resultItem = section.Items[0];
			
			Assert.That(resultItem.Size.Height > CreateBigItem().Size.Height,"Result Rectangle should be extendend");
			Assert.That(sectionRect.Height < resultRec.Height,"result Rectangle should be higher than standard");
		}
		
		
		private BaseTextItem CreateSmallItem()
		{
			BaseTextItem item = new BaseTextItem()
			{
				CanGrow = true,
				Location = new Point(5,5),
				Size = new Size (100,20),
				Text = "hi i'm the text"
			};
			return item;
		}
		
		
		private BaseTextItem CreateBigItem()
		{
			BaseTextItem item = new BaseTextItem()
			{
				CanGrow = true,
				Location = new Point(5,5),
				Size = new Size (100,20),
				Text = "hi i'm the text, this text didn't fit in rectangle"
			};
			return item;
		}
		
		public override void Setup()
		{
			bitmap = new Bitmap(1,1);
			graphics = Graphics.FromImage(bitmap) ;
			section = new BaseSection()
			{
				Location = new Point (50,50),
				Size = new Size(110,30)
			};
			Sut = new Layouter();
		}
	}
}
