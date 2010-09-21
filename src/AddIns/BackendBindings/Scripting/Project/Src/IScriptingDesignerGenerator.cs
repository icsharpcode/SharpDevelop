// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

using ICSharpCode.FormsDesigner;

namespace ICSharpCode.Scripting
{
	public interface IScriptingDesignerGenerator : IDesignerGenerator
	{
		/// <summary>
		/// Updates the form or user control's InitializeComponent method with any 
		/// changes to the designed form or user control.
		/// </summary>
		void MergeRootComponentChanges(IDesignerHost host, IDesignerSerializationManager serializationManager);
	}
}
