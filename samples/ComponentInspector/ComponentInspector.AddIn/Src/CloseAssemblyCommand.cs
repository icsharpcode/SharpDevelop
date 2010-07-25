// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.ComponentInspector.AddIn
{
	/// <summary>
	/// Closes the selected assembly in the Component Inspector.
	/// </summary>
	public class CloseAssemblyCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (ComponentInspectorView.Instance == null) {
				return;
			}
			
			ComponentInspectorView.Instance.CloseSelectedFile();
		}
	}
}
