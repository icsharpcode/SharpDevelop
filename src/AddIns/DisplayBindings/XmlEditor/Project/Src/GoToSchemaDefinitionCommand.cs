// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui;
using System;
using ICSharpCode.Core;

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
			XmlView xmlView = XmlView.ActiveXmlView;
			if (xmlView != null) {
				xmlView.GoToSchemaDefinition();
			}
		}
	}
}
