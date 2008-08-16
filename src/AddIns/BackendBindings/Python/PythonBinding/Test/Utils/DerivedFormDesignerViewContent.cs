// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.FormsDesigner;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Gui;

namespace PythonBinding.Tests.Utils
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
