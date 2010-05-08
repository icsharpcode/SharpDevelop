/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 07.10.2008
 * Zeit: 19:36
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using ICSharpCode.Reports.Addin.ReportWizard;
using ICSharpCode.Reports.Core;
using NUnit.Framework;

namespace ICSharpCode.Reports.Addin.Test.Wizard.Generators
{
	[TestFixture]
	public class LayoutFixture
	{
		[Test]
		public void ListLayoutTest()
		{
			AbstractLayout l = LayoutFactory .CreateGenerator(GlobalEnums.ReportLayout.ListLayout,
			                                                     ReportModel.Create(),
			                                                     new ReportItemCollection());
			Assert.IsAssignableFrom<ListLayout>(l,"Should be 'ListLayout'");
		}
		
		[Test]
		public void TableLayoutTest()
		{
			AbstractLayout l = LayoutFactory.CreateGenerator(GlobalEnums.ReportLayout.TableLayout,
			                                                     ReportModel.Create(),
			                                                     new ReportItemCollection());
			Assert.IsAssignableFrom<TableLayout>(l,"Should be 'TableLayout'");
		}
	}
}
