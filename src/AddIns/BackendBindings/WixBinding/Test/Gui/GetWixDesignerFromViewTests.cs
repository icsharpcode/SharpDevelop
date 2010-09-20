// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Gui
{
	/// <summary>
	/// Tests that the WixDialogDesigner.GetDesigner method returns the 
	/// Wix dialog designer attached to the primary view.
	/// </summary>
	[TestFixture]
	public class GetWixDesignerFromViewTests
	{
		[Test]
		public void WixDesignerAttached()
		{
			MockTextEditorViewContent view = new MockTextEditorViewContent();
			using (WixDialogDesigner designerAdded = new WixDialogDesigner(view)) {
				view.SecondaryViewContents.Add(designerAdded);
				Assert.IsNotNull(WixDialogDesigner.GetDesigner(view));
			}
		}
		
		[Test]
		public void NoWixDesignerAttached()
		{
			MockViewContent view = new MockViewContent();
			Assert.IsNull(WixDialogDesigner.GetDesigner(view));
		}
	}
}
