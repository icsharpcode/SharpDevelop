// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
