// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit.AddIn.Options;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class TextEditorOptions : ITextEditorOptions
	{
		IPackageManagementWorkbench workbench;
		
		public TextEditorOptions()
		{
			workbench = new PackageManagementWorkbench();
		}
		
		public double FontSize {
			get { return CodeEditorOptions.FontSize; }
			set {
				SetFontSize(value);
			}
		}
		
		CodeEditorOptions CodeEditorOptions {
			get { return CodeEditorOptions.Instance; }
		}
		
		void SetFontSize(double value)
		{
			if (workbench.InvokeRequired) {
				Action<double> action = SetFontSize;
				workbench.SafeThreadAsyncCall(action, value);
			} else {
				CodeEditorOptions.FontSize = value;
			}
		}
	}
}
