// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using ICSharpCode.FormsDesigner.Services;

namespace ICSharpCode.FormsDesigner
{
	/// <summary>
	/// Interface used within the foms designer to refer to the designer view content.
	/// </summary>
	public interface IFormsDesigner
	{
		IDesignerGenerator Generator { get; }
		
		IntPtr GetDialogOwnerWindowHandle();
		
		void ShowSourceCode();
		void ShowSourceCode(int lineNumber);
		void ShowSourceCode(IComponent component, EventDescriptor edesc, string methodName);
		
		SharpDevelopDesignerOptions DesignerOptions { get; }
	}
}
