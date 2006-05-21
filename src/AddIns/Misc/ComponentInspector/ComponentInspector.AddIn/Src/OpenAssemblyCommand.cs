// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using System;
using System.Windows.Forms;

namespace ICSharpCode.ComponentInspector.AddIn
{
	public class OpenAssemblyCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (ComponentInspectorView.Instance == null) {
				return;
			}
			
			using (OpenFileDialog dialog  = new OpenFileDialog()) {
				dialog.CheckFileExists = true;
				if (DialogResult.OK == dialog.ShowDialog()) {
					ComponentInspectorView.Instance.OpenFile(dialog.FileName);
				}
			}
		}
	}
}
