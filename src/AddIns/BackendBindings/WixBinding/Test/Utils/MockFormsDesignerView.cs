// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.WixBinding;

namespace WixBinding.Tests.Utils
{
	public class MockFormsDesignerView : IFormsDesignerView
	{
		public OpenedFile PrimaryFile { get; set; }
	}
}
