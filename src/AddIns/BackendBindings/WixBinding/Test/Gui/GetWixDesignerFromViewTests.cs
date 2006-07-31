// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
			MockViewContent view = new MockViewContent();
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
