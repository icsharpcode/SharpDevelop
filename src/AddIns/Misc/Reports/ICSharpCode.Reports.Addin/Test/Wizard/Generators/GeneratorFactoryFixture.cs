// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Reports.Addin.ReportWizard;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
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
