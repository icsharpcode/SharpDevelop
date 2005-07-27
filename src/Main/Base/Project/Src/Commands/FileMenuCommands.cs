// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Dialogs;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class CreateNewSolution : AbstractMenuCommand
	{
		public override void Run()
		{
			
			
			using (NewProjectDialog npdlg = new NewProjectDialog(true)) {
				npdlg.Owner = (Form)WorkbenchSingleton.Workbench;
				npdlg.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
			}
		}
	}
	
	public class LoadSolution : AbstractMenuCommand
	{
		public override void Run()
		{
			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
				AddInTreeNode addinTreeNode = AddInTree.GetTreeNode("/SharpDevelop/Workbench/Combine/FileFilter");
				StringBuilder b = new StringBuilder("All known project formats|");
				bool first = true;
				foreach (Codon c in addinTreeNode.Codons) {
					if (!first) {
						b.Append(';');
					} else {
						first = false;
					}
					string ext = c.Properties.Get("extensions", "");
					if (ext != "*.*" && ext.Length > 0) {
						b.Append(ext);
					}
				}
				foreach (string entry in addinTreeNode.BuildChildItems(this)) {
					b.Append('|');
					b.Append(entry);
				}
				fdiag.AddExtension    = true;
				fdiag.Filter          = b.ToString();
				fdiag.Multiselect     = false;
				fdiag.CheckFileExists = true;
				if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					switch (Path.GetExtension(fdiag.FileName).ToUpper()) {
						case ".CMBX":
						case ".SLN":
							ProjectService.LoadSolution(fdiag.FileName);
							break;
						case ".CSPROJ":
						case ".VBPROJ":
							ProjectService.LoadProject(fdiag.FileName);
							break;
//						case ".PRJX":
//
//
//							FileUtility.ObservedLoad(new NamedFileOperationDelegate(ProjectService.OpenSolution), fdiag.FileName);
//							break;
						default:
							
							
							MessageService.ShowError(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.OpenCombine.InvalidProjectOrCombine}", new string[,] {{"FileName", fdiag.FileName}}));
							break;
					}
				}
			}
		}
	}
	
	public class CloseSolution : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectService.CloseSolution();
		}
	}
}
