// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: -1 $</version>
// </file>

using ICSharpCode.SharpDevelop.Gui;
using System;
using ICSharpCode.Core;
using ICSharpCode.XmlEditor.Gui;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Finds the definition of the Xml element or attribute under the cursor,
	/// finds the schema definition for it and then opens the schema and puts the cursor
	/// on the definition.
	/// </summary>
	public class GoToSchemaDefinitionCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			XmlView properties = XmlView.ForView(WorkbenchSingleton.Workbench.ActiveViewContent);
			
			if (properties != null) {
				properties.GoToSchemaDefinition();
			}
		}
	}
}
