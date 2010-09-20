// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.FormsDesigner;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Scripting.Tests.Utils
{
	/// <summary>
	/// Derived FormDesignerViewContent class that is used for
	/// unit testing the PythonDesignerGenerator.
	/// </summary>
	public class DerivedFormDesignerViewContent : FormsDesignerViewContent
	{
		bool mergeFormChangesCalled;
				
		public DerivedFormDesignerViewContent(IViewContent view, ICSharpCode.SharpDevelop.OpenedFile mockFile)
			: base(view, mockFile)
		{
		}
		
		public override void MergeFormChanges()
		{
			mergeFormChangesCalled = true;
		}
		
		/// <summary>
		/// Gets whether the MergeFormChanges method was called.
		/// </summary>
		public bool MergeFormChangesCalled {
			get { return mergeFormChangesCalled; }
		}		
	}
}
