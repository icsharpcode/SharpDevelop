// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Security.AccessControl;

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
		
		void MakeDirty();
		void InvalidateRequerySuggested();
	}
	
	public enum ResourceType
	{
		Resx = 0,
		Resources = 1
	}
	
	public interface IResourceStore
	{
		Stream GetResourceAsStreamForReading(CultureInfo info, out ResourceType type);
		Stream GetResourceAsStreamForWriting(CultureInfo info, out ResourceType type);
	}
}
