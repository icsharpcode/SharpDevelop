// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

using ICSharpCode.SharpDevelop.Project;
using NSvn.Common;
using NSvn.Core;

namespace ICSharpCode.Svn.Commands
{
	public abstract class SubversionCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			AbstractProjectBrowserTreeNode node = ProjectBrowserPad.Instance.SelectedNode;
			if (node != null) {
				if (node is DirectoryNode) {
					Run(((DirectoryNode)node).Directory);
				} else if (node is FileNode) {
					Run(((FileNode)node).FileName);
				} else if (node is SolutionNode) {
					Run(((SolutionNode)node).Solution.Directory);
				}
			}
		}
		
		protected void Callback()
		{
			WorkbenchSingleton.SafeThreadAsyncCall((MethodInvoker)CallbackInvoked);
		}
		
		void CallbackInvoked()
		{
			SubversionStateCondition.ResetCache();
			AbstractProjectBrowserTreeNode node = ProjectBrowserPad.Instance.SelectedNode;
			if (node != null) {
				OverlayIconManager.EnqueueRecursive(node);
			}
		}
		
		protected abstract void Run(string filename);
	}
	
	public class UpdateCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Update(filename, Callback);
		}
	}
	
	public class UpdateToRevisionCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.UpdateToRevision(filename, Callback);
		}
	}
	
	public class RevertCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Revert(filename, Callback);
		}
	}
	
	public class CreatePatchCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.CreatePatch(filename, null);
		}
	}
	
	public class ApplyPatchCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.ApplyPatch(filename, null);
		}
	}
	
	public class CommitCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Commit(filename, Callback);
		}
	}
	
	public class AddCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Add(filename, Callback);
		}
	}
	
	public class IgnoreCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Ignore(filename, Callback);
		}
	}
	
	public class BlameCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Blame(filename, Callback);
		}
	}
	
	public class UnignoreCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			PropertyDictionary pd = SvnClient.Instance.Client.PropGet("svn:ignore", Path.GetDirectoryName(filename), Revision.Working, false);
			if (pd != null) {
				string shortFileName = Path.GetFileName(filename);
				foreach (Property p in pd.Values) {
					StringBuilder b = new StringBuilder();
					using (StreamReader r = new StreamReader(new MemoryStream(p.Data))) {
						string line;
						while ((line = r.ReadLine()) != null) {
							if (!string.Equals(line, shortFileName, StringComparison.InvariantCultureIgnoreCase)) {
								b.AppendLine(line);
							}
						}
					}
					SvnClient.Instance.Client.PropSet(new Property(p.Name, b.ToString()),
					                                  Path.GetDirectoryName(filename), false);
				}
				MessageService.ShowMessage(shortFileName + " was removed from the ignore list.");
				Callback();
			}
		}
	}
	
	public class HelpCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			SvnGuiWrapper.ShowSvnHelp();
		}
	}
	
	public class SettingsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			SvnGuiWrapper.ShowSvnSettings();
		}
	}
	
	public class AboutCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			SvnGuiWrapper.ShowSvnAbout();
		}
	}
	
	public class DiffCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Diff(filename, Callback);
		}
	}
	
	public class EditConflictsCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.ConflictEditor(filename, Callback);
		}
	}
	
	public class ResolveConflictsCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.ResolveConflict(filename, Callback);
		}
	}
	
	public class ShowLogCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.ShowLog(filename, Callback);
		}
	}
	
	public class CleanupCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Cleanup(filename, Callback);
		}
	}
	
	public class RepoBrowserCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.RepoBrowser(filename, Callback);
		}
	}
	
	public class RepoStatusCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.RepoStatus(filename, Callback);
		}
	}
	
	public class RevisionGraphCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.RevisionGraph(filename, Callback);
		}
	}
	
	public class BranchCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Branch(filename, Callback);
		}
	}
	
	public class SwitchCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Switch(filename, Callback);
		}
	}
	
	public class MergeCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Merge(filename, Callback);
		}
	}
	
	public class ExportWorkingCopyCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Export(filename, Callback);
		}
	}
	
	public class RelocateCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Relocate(filename, Callback);
		}
	}
}
