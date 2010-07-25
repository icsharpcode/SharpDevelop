// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;

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
				dialog.Filter = StringParser.Parse("${res:ComponentInspector.ObjectBrowserForm.AssemblyFilesFilterName} (*.exe;*.dll)|*.exe;*.dll|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
				dialog.FilterIndex = 0;
				if (DialogResult.OK == dialog.ShowDialog()) {
					ComponentInspectorView.Instance.OpenFile(dialog.FileName);
				}
			}
		}
	}
}
