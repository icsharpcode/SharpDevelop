// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
