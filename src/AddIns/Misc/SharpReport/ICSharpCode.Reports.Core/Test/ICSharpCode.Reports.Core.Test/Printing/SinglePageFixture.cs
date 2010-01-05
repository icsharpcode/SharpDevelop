/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 07.07.2009
 * Zeit: 19:59
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.Printing
{
	[TestFixture]
	public class SinglePageFixture
	{
		
		private SectionBounds sectionBounds;
		
		[Test]
		public void Can_Create_SinglePage()
		{
			SinglePage p = new SinglePage(sectionBounds,15);
			Assert.IsNotNull(p);
		}
		
		
		[Test]
		[ExpectedExceptionAttribute(typeof(ArgumentNullException))]
		public void ConstrThrowIfNullSectionBounds()
		{
			SinglePage p = new SinglePage(null,15);
		}
		
		
		[Test]
		[ExpectedExceptionAttribute(typeof(ArgumentNullException))]
		public void ConstrThrowIfPageNumberLessThenZero()
		{
			SinglePage p = new SinglePage(this.sectionBounds,-1);
		}
		
		
		#region Setup/Teardown
		
		[TestFixtureSetUp]
		public void Init()
		{
			
			this.sectionBounds = new SectionBounds(new ReportSettings(),false);
		}
		
		#endregion
	}
}
