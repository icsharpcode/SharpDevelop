// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.MonoAddIn
{
	/// <summary>
	/// Displays the AddMonoReference dialog allowing the user to add 
	/// a Mono GAC reference to the project
	/// </summary>
	public class AddMonoReferenceCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			AbstractProjectBrowserTreeNode node = Owner as AbstractProjectBrowserTreeNode;
			IProject project = (node != null) ? node.Project : ProjectService.CurrentProject;
			if (project == null) {
				return;
			}
			using (SelectMonoReferenceDialog selDialog = new SelectMonoReferenceDialog(project)) {
				if (selDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {

					/*MonoProjectContentLoader.CreateMonoProjectContent(project);
					
					foreach (ReferenceProjectItem gacReference in selDialog.GacReferences) {
						MonoProjectContentLoader.AddGacReference(gacReference.Include);
					}*/
				
					foreach (ReferenceProjectItem reference in selDialog.ReferenceInformations) {
						ProjectService.AddProjectItem(project, reference);
					}

					project.Save();
				}
			}
		}
	}
}
