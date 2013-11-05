/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 25.04.2013
 * Time: 19:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.ReportItems
{
	[TestFixture]
	public class TextItemFixture
	{
		[Test]
		public void IsNameSetOnInitialize() {
			var ti = new BaseTextItem();
			Assert.That (ti.Name,Is.EqualTo("BaseTextItem"));
		}
		
		
		[Test]
		public void ChangeName() {
			var newName = "changed";
			var ti = new BaseTextItem();
			ti.Name = newName;
			Assert.That(ti.Name,Is.EqualTo(newName));
		}
		
		
		[Test]
		public void DefaultFontOnInitialize() {
			var ti = new BaseTextItem();
			Assert.That(ti.Font,Is.EqualTo(GlobalValues.DefaultFont));
		}
		
		[Test]
		public void CreateExportText() {
			var ti = new BaseTextItem();
			var exportText = (ExportText)ti.CreateExportColumn();
			Assert.That(exportText.Name,Is.EqualTo(ti.Name));
			Assert.That(exportText.Location,Is.EqualTo(ti.Location));
			Assert.That(exportText.Size,Is.EqualTo(ti.Size));
			Assert.That(exportText.Font , Is.EqualTo(GlobalValues.DefaultFont));
		}
			
	}
}
