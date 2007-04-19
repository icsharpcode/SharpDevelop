// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ICSharpCode.Core;
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
				string nodeFileName = null;
				if (node is DirectoryNode) {
					nodeFileName = ((DirectoryNode)node).Directory;
				} else if (node is FileNode) {
					nodeFileName =  ((FileNode)node).FileName;
				} else if (node is SolutionNode) {
					nodeFileName = ((SolutionNode)node).Solution.Directory;
				}
				if (nodeFileName != null) {
					List<IViewContent> unsavedViewContents = new List<IViewContent>();
					foreach (IViewContent vc in WorkbenchSingleton.Workbench.ViewContentCollection) {
						if (string.IsNullOrEmpty(vc.FileName)) continue;
						if (FileUtility.IsUrl(vc.FileName)) continue;
						if (vc.IsDirty == false) continue;
						if (FileUtility.IsBaseDirectory(nodeFileName, vc.FileName)) {
							unsavedViewContents.Add(vc);
						}
					}
					if (unsavedViewContents.Count > 0) {
						if (MessageService.ShowCustomDialog(
							MessageService.DefaultMessageBoxTitle,
							"The version control operation would affect files with unsaved modifications.\n" +
							"You have to save those files before running the operation.",
							0, 1,
							"Save files", "Cancel")
						    == 0)
						{
							// Save
							foreach (IViewContent vc in unsavedViewContents) {
								ProjectService.MarkFileDirty(vc.FileName);
								FileUtility.ObservedSave(new FileOperationDelegate(vc.Save), vc.FileName, FileErrorPolicy.ProvideAlternative);
							}
						} else {
							// Cancel
							return;
						}
					}
					// now run the actual operation:
					Run(nodeFileName);
				}
			}
		}
		
		protected ProjectWatcher WatchProjects()
		{
			return new ProjectWatcher(ProjectService.OpenSolution);
		}
		
		static void CallbackInvoked()
		{
			SubversionStateCondition.ResetCache();
			AbstractProjectBrowserTreeNode node = ProjectBrowserPad.Instance.SelectedNode;
			if (node != null) {
				OverlayIconManager.EnqueueRecursive(node);
			}
		}
		
		protected abstract void Run(string filename);
		
		struct ProjectEntry
		{
			string fileName;
			long size;
			DateTime writeTime;
			
			public ProjectEntry(FileInfo file)
			{
				fileName = file.FullName;
				if (file.Exists) {
					size = file.Length;
					writeTime = file.LastWriteTime;
				} else {
					size = -1;
					writeTime = DateTime.MinValue;
				}
			}
			
			public bool HasFileChanged()
			{
				FileInfo file = new FileInfo(fileName);
				long newSize;
				DateTime newWriteTime;
				if (file.Exists) {
					newSize = file.Length;
					newWriteTime = file.LastWriteTime;
				} else {
					newSize = -1;
					newWriteTime = DateTime.MinValue;
				}
				return size != newSize || writeTime != newWriteTime;
			}
		}
		
		/// <summary>
		/// Remembers a list of file sizes and last write times. If a project
		/// changed during the operation, suggest that the user reloads the solution.
		/// </summary>
		protected sealed class ProjectWatcher
		{
			List<ProjectEntry> list = new List<ProjectEntry>();
			Solution solution;
			
			internal ProjectWatcher(Solution solution)
			{
				this.solution = solution;
				if (AddInOptions.AutomaticallyReloadProject && solution != null)
				{
					list.Add(new ProjectEntry(new FileInfo(solution.FileName)));
					foreach (IProject p in solution.Projects) {
						list.Add(new ProjectEntry(new FileInfo(p.FileName)));
					}
				}
			}
			
			public void Callback()
			{
				WorkbenchSingleton.SafeThreadAsyncCall(CallbackInvoked);
			}
			
			void CallbackInvoked()
			{
				SubversionCommand.CallbackInvoked();
				if (ProjectService.OpenSolution != solution)
					return;
				if (!list.TrueForAll(delegate (ProjectEntry pe) {
				                     	return !pe.HasFileChanged();
				                     }))
				{
					// if at least one project was changed:
					if (MessageService.ShowCustomDialog(
						MessageService.DefaultMessageBoxTitle,
						"SharpDevelop detected that the version control operation changed " +
						"project files.\n" +
						"You should reload the solution.",
						0, 1,
						"Reload solution", "Keep old solution open")
					    == 0)
					{
						ProjectService.LoadSolution(solution.FileName);
					}
				}
			}
		}
	}
	
	public class UpdateCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Update(filename, WatchProjects().Callback);
		}
	}
	
	public class UpdateToRevisionCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.UpdateToRevision(filename, WatchProjects().Callback);
		}
	}
	
	public class RevertCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Revert(filename, WatchProjects().Callback);
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
			SvnGuiWrapper.ApplyPatch(filename, WatchProjects().Callback);
		}
	}
	
	public class CommitCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Commit(filename, WatchProjects().Callback);
		}
	}
	
	public class AddCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Add(filename, WatchProjects().Callback);
		}
	}
	
	public class IgnoreCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Ignore(filename, WatchProjects().Callback);
		}
	}
	
	public class BlameCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Blame(filename, null);
		}
	}
	
	public class UnignoreCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			PropertyDictionary pd = SvnClient.Instance.Client.PropGet("svn:ignore", Path.GetDirectoryName(filename), Revision.Working, Recurse.None);
			if (pd != null) {
				ProjectWatcher watcher = WatchProjects();
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
					                                  Path.GetDirectoryName(filename), Recurse.None);
				}
				MessageService.ShowMessage(shortFileName + " was removed from the ignore list.");
				watcher.Callback();
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
			SvnGuiWrapper.Diff(filename, WatchProjects().Callback);
		}
	}
	
	public class EditConflictsCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.ConflictEditor(filename, WatchProjects().Callback);
		}
	}
	
	public class ResolveConflictsCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.ResolveConflict(filename, WatchProjects().Callback);
		}
	}
	
	public class ShowLogCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.ShowLog(filename, WatchProjects().Callback);
		}
	}
	
	public class CleanupCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Cleanup(filename, null);
		}
	}
	
	public class RepoBrowserCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.RepoBrowser(filename, null);
		}
	}
	
	public class RepoStatusCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.RepoStatus(filename, null);
		}
	}
	
	public class RevisionGraphCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.RevisionGraph(filename, null);
		}
	}
	
	public class BranchCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Branch(filename, WatchProjects().Callback);
		}
	}
	
	public class SwitchCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Switch(filename, WatchProjects().Callback);
		}
	}
	
	public class MergeCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Merge(filename, WatchProjects().Callback);
		}
	}
	
	public class ExportWorkingCopyCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Export(filename, null);
		}
	}
	
	public class RelocateCommand : SubversionCommand
	{
		protected override void Run(string filename)
		{
			SvnGuiWrapper.Relocate(filename, WatchProjects().Callback);
		}
	}
}
