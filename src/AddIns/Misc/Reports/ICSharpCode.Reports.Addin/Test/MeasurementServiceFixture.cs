/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 07.09.2008
 * Zeit: 18:32
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Addin.Test
{
	[TestFixture]
	public class MeasurementServiceFixture
	{
		Graphics graphics;
		BaseSection section;
		
		[Test]
		public void Constructor()
		{
			MeasurementService s = new MeasurementService(this.graphics);
			Assert.IsNotNull(s);
		}
		
		[Test]
		public void ReportItemFitInSection ()
		{
			MeasurementService s = new MeasurementService(this.graphics);
			Rectangle r = new Rectangle(section.Location,section.Size);
			this.section.Items.Clear();
			BaseReportItem item = new BaseReportItem();
			item.Location = new Point(10,10);
			item.Size = new Size(50,50);
			this.section.Items.Add(item);
				s.FitSectionToItems(this.section);
			Assert.AreEqual(r.Location,s.Rectangle.Location);
		}
		
		[Test]
		public void CheckPlainSection ()
		{
			MeasurementService s = new MeasurementService(this.graphics);
			Rectangle r = new Rectangle(section.Location,section.Size);
			s.FitSectionToItems(this.section);
			Assert.AreEqual(r.Location,s.Rectangle.Location);
		}
	
		
		[TestFixtureSetUp]
		public void Init()
		{
			Label l = new Label();
			this.graphics = l.CreateGraphics();
			section = new BaseSection();
			section.Location = new Point (50,50);
			section.Size = new Size(400,200);
		}
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add tear down code.
		}
	}
}
