// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class RecentFilesMenuBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			RecentOpen recentOpen = FileService.RecentOpen;
			
			if (recentOpen.RecentFile.Count > 0) {
				MenuCommand[] items = new MenuCommand[recentOpen.RecentFile.Count];
				
				for (int i = 0; i < recentOpen.RecentFile.Count; ++i) {
					string accelaratorKeyPrefix = i < 10 ? "&" + ((i + 1) % 10) + " " : "";
					items[i] = new MenuCommand(accelaratorKeyPrefix + recentOpen.RecentFile[i], new EventHandler(LoadRecentFile));
					items[i].Tag = recentOpen.RecentFile[i].ToString();
					items[i].Description = StringParser.Parse(ResourceService.GetString("Dialog.Componnents.RichMenuItem.LoadFileDescription"),
					                                          new string[,] { {"FILE", recentOpen.RecentFile[i].ToString()} });
				}
				return items;
			}
			
			MenuCommand defaultMenu = new MenuCommand("${res:Dialog.Componnents.RichMenuItem.NoRecentFilesString}");
			defaultMenu.Enabled = false;
			
			return new MenuCommand[] { defaultMenu };
		}
		
		void LoadRecentFile(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			
			FileService.OpenFile(item.Tag.ToString());
		}
	}
	
	public class RecentProjectsMenuBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			RecentOpen recentOpen = FileService.RecentOpen;
			
			if (recentOpen.RecentProject.Count > 0) {
				MenuCommand[] items = new MenuCommand[recentOpen.RecentProject.Count];
				for (int i = 0; i < recentOpen.RecentProject.Count; ++i) {
					string accelaratorKeyPrefix = i < 10 ? "&" + ((i + 1) % 10) + " " : "";
					items[i] = new MenuCommand(accelaratorKeyPrefix + recentOpen.RecentProject[i], new EventHandler(LoadRecentProject));
					items[i].Tag = recentOpen.RecentProject[i].ToString();
					items[i].Description = StringParser.Parse(ResourceService.GetString("Dialog.Componnents.RichMenuItem.LoadProjectDescription"),
					                                          new string[,] { {"PROJECT", recentOpen.RecentProject[i].ToString()} });
				}
				return items;
			}
			
			MenuCommand defaultMenu = new MenuCommand("${res:Dialog.Componnents.RichMenuItem.NoRecentProjectsString}");
			defaultMenu.Enabled = false;
			
			return new MenuCommand[] { defaultMenu };
		}
		void LoadRecentProject(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			
			string fileName = item.Tag.ToString();
			
			FileUtility.ObservedLoad(new NamedFileOperationDelegate(ProjectService.LoadSolution), fileName);
		}
	}
	
	public class ToolMenuBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			MenuCommand[] items = new MenuCommand[ToolLoader.Tool.Count];
			for (int i = 0; i < ToolLoader.Tool.Count; ++i) {
				MenuCommand item = new MenuCommand(ToolLoader.Tool[i].ToString(), new EventHandler(ToolEvt));
				item.Description = "Start tool " + String.Join(String.Empty, ToolLoader.Tool[i].ToString().Split('&'));
				items[i] = item;
			}
			return items;
		}
		
		void ProcessExitEvent(object sender, EventArgs e)
		{
			Process p = (Process)sender;
			string output = p.StandardOutput.ReadToEnd();
			
			TaskService.BuildMessageViewCategory.AppendText(output + Environment.NewLine + "Exited with code:" + p.ExitCode + Environment.NewLine);
		}
		
		void ToolEvt(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			
			for (int i = 0; i < ToolLoader.Tool.Count; ++i) {
				if (item.Text == ToolLoader.Tool[i].ToString()) {
					ExternalTool tool = (ExternalTool)ToolLoader.Tool[i];
					IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
					string fileName = window == null ? null : window.ViewContent.FileName;
					StringParser.Properties["ItemPath"]        = fileName == null ? String.Empty : fileName;
					StringParser.Properties["ItemDir"]         = fileName == null ? String.Empty : Path.GetDirectoryName(fileName);
					StringParser.Properties["ItemFileName"]    = fileName == null ? String.Empty : Path.GetFileName(fileName);
					StringParser.Properties["ItemExt"]         = fileName == null ? String.Empty : Path.GetExtension(fileName);
					
					// TODO:
					StringParser.Properties["CurLine"]         = "0";
					StringParser.Properties["CurCol"]          = "0";
					StringParser.Properties["CurText"]         = "0";
					
					ILanguageBinding binding = ProjectService.CurrentProject == null ? null : LanguageBindingService.GetBindingPerLanguageName(ProjectService.CurrentProject.Language);
					string targetPath = ProjectService.CurrentProject == null ? null : ProjectService.CurrentProject.OutputAssemblyFullPath;
					StringParser.Properties["TargetPath"]      = targetPath == null ? String.Empty : targetPath;
					StringParser.Properties["TargetDir"]       = targetPath == null ? String.Empty : Path.GetDirectoryName(targetPath);
					StringParser.Properties["TargetName"]      = targetPath == null ? String.Empty : Path.GetFileName(targetPath);
					StringParser.Properties["TargetExt"]       = targetPath == null ? String.Empty : Path.GetExtension(targetPath);
					
					string projectFileName = ProjectService.CurrentProject == null ? null : ProjectService.CurrentProject.FileName;
					StringParser.Properties["ProjectDir"]      = projectFileName == null ? null : Path.GetDirectoryName(projectFileName);
					StringParser.Properties["ProjectFileName"] = projectFileName == null ? null : projectFileName;
					
					string combineFileName = ProjectService.OpenSolution == null ? null : ProjectService.OpenSolution.FileName;
					StringParser.Properties["CombineDir"]      = combineFileName == null ? null : Path.GetDirectoryName(combineFileName);
					StringParser.Properties["CombineFileName"] = combineFileName == null ? null : combineFileName;
					
					StringParser.Properties["StartupPath"]     = Application.StartupPath;
					
					string command = StringParser.Parse(tool.Command);
					string args    = StringParser.Parse(tool.Arguments);
					
					if (tool.PromptForArguments) {
						InputBox box = new InputBox();
						box.Text = tool.MenuCommand;
						box.Label.Text = "Enter arguments for the tool:";
						box.TextBox.Text = args;
						if (box.ShowDialog() != DialogResult.OK)
							return;
						args = box.TextBox.Text;
					}
					
					try {
						ProcessStartInfo startinfo;
						if (args == null || args.Length == 0 || args.Trim('"', ' ').Length == 0) {
							startinfo = new ProcessStartInfo(command);
						} else {
							startinfo = new ProcessStartInfo(command, args);
						}
						
						startinfo.WorkingDirectory = StringParser.Parse(tool.InitialDirectory);
						if (tool.UseOutputPad) {
							startinfo.UseShellExecute = false;
							startinfo.RedirectStandardOutput = true;
						}
						Process process = new Process();
						process.EnableRaisingEvents = true;
						process.StartInfo = startinfo;
						if (tool.UseOutputPad) {
							process.Exited += new EventHandler(ProcessExitEvent);
						}
						process.Start();
					} catch (Exception ex) {
						MessageService.ShowError("External program execution failed.\nError while starting:\n '" + command + " " + args + "'\n" + ex.Message);
					}
					break;
				}
			}
		}
	}
	
	public class OpenContentsMenuBuilder : ISubmenuBuilder
	{
		
		class MyMenuItem : MenuCheckBox
		{
			IViewContent content;
			public MyMenuItem(IViewContent content) : base(StringParser.Parse(content.TitleName))
			{
				this.content = content;
			}
			
			protected override void OnClick(EventArgs e)
			{
				base.OnClick(e);
				Checked = true;
				content.WorkbenchWindow.SelectWindow();
			}
		}

		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			int contentCount = WorkbenchSingleton.Workbench.ViewContentCollection.Count;
			if (contentCount == 0) {
				return new ToolStripItem[] {};
			}
			ToolStripItem[] items = new ToolStripItem[contentCount + 1];
			items[0] = new MenuSeparator(null, null);
			for (int i = 0; i < contentCount; ++i) {
				IViewContent content = (IViewContent)WorkbenchSingleton.Workbench.ViewContentCollection[i];
				if (content.WorkbenchWindow == null) {
					continue;
				}
				MenuCheckBox item = new MyMenuItem(content);
				item.Tag = content.WorkbenchWindow;
				item.Checked = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == content.WorkbenchWindow;
				item.Description = "Activate this window ";
				items[i + 1] = item;
			}
			return items;
		}
	}
	
//	public class IncludeFilesBuilder : ISubmenuBuilder
//	{
//		public ProjectBrowserView browser;
//
//		MyMenuItem includeInCompileItem;
//		MyMenuItem includeInDeployItem;
//
//		class MyMenuItem : MenuCheckBox
//		{
//			IncludeFilesBuilder builder;
//
//			public MyMenuItem(IncludeFilesBuilder builder, string name, EventHandler handler) : base(null, null, name)
//			{
//				base.Click += handler;
//				this.builder = builder;
//			}
//
//			public override void UpdateStatus()
//			{
//				base.UpdateStatus();
//				if (builder == null) {
//					return;
//				}
//				AbstractBrowserNode node = builder.browser.SelectedNode as AbstractBrowserNode;
//
//				if (node == null) {
//					return;
//				}
//
//				ProjectFile finfo = node.UserData as ProjectFile;
//				if (finfo == null) {
//					builder.includeInCompileItem.Enabled = builder.includeInCompileItem.Enabled = false;
//				} else {
//					if (!builder.includeInCompileItem.Enabled) {
//						builder.includeInCompileItem.Enabled = builder.includeInCompileItem.Enabled = true;
//					}
//					builder.includeInCompileItem.Checked = finfo.BuildAction == BuildAction.Compile;
//					builder.includeInDeployItem.Checked  = !node.Project.DeployInformation.IsFileExcluded(finfo.Name);
//				}
//			}
//		}
//
//		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
//		{
//			browser = (ProjectBrowserView)owner;
//			includeInCompileItem = new MyMenuItem(this, "${res:ProjectComponent.ContextMenu.IncludeMenu.InCompile}", new EventHandler(ChangeCompileInclude));
//			includeInDeployItem  = new MyMenuItem(this, "${res:ProjectComponent.ContextMenu.IncludeMenu.InDeploy}",  new EventHandler(ChangeDeployInclude));
//
//			return new ToolStripItem[] {
//				includeInCompileItem,
//				includeInDeployItem
//			};
//
//		}
//		void ChangeCompileInclude(object sender, EventArgs e)
//		{
//			AbstractBrowserNode node = browser.SelectedNode as AbstractBrowserNode;
//
//			if (node == null) {
//				return;
//			}
//
//			ProjectFile finfo = node.UserData as ProjectFile;
//			if (finfo != null) {
//				if (finfo.BuildAction == BuildAction.Compile) {
//					finfo.BuildAction = BuildAction.Nothing;
//				} else {
//					finfo.BuildAction = BuildAction.Compile;
//				}
//			}
//
//			ProjectService.SaveCombine();
//		}
//
//		void ChangeDeployInclude(object sender, EventArgs e)
//		{
//			AbstractBrowserNode node = browser.SelectedNode as AbstractBrowserNode;
//
//			if (node == null) {
//				return;
//			}
//
//			ProjectFile finfo = node.UserData as ProjectFile;
//			if (finfo != null) {
//				if (node.Project.DeployInformation.IsFileExcluded(finfo.Name)) {
//					node.Project.DeployInformation.RemoveExcludedFile(finfo.Name);
//				} else {
//					node.Project.DeployInformation.AddExcludedFile(finfo.Name);
//				}
//			}
//
//			ProjectService.SaveCombine();
//		}
//	}
	
	public class ToolsViewMenuBuilder : ViewMenuBuilder
	{
		protected override string Category {
			get {
				return "Tools";
			}
		}
	}
	
	public class MainViewMenuBuilder : ViewMenuBuilder
	{
		protected override string Category {
			get {
				return "Main";
			}
		}
	}
	
	public class DebugViewMenuBuilder : ViewMenuBuilder
	{
		protected override string Category {
			get {
				return "Debugger";
			}
		}
	}
	
	public abstract class ViewMenuBuilder : ISubmenuBuilder
	{
		class MyMenuItem : MenuCommand
		{
			PadDescriptor padDescriptor;
			
			public MyMenuItem(PadDescriptor padDescriptor) : base(null, null)
			{
				this.padDescriptor = padDescriptor;
				Text = StringParser.Parse(padDescriptor.Title);
				
				if (!string.IsNullOrEmpty(padDescriptor.Icon)) {
					base.Image = IconService.GetBitmap(padDescriptor.Icon);
				}
				
				if (padDescriptor.Shortcut != null) {
					ShortcutKeys = ICSharpCode.Core.MenuCommand.ParseShortcut(padDescriptor.Shortcut);
				}
			}
			
			protected override void OnClick(EventArgs e)
			{
				base.OnClick(e);
				padDescriptor.BringPadToFront();
			}
			
		}
		protected abstract string Category {
			get;
		}
		
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			ArrayList items = new ArrayList();
			foreach (PadDescriptor padContent in WorkbenchSingleton.Workbench.PadContentCollection) {
				if (padContent.Category == Category) {
					items.Add(new MyMenuItem(padContent));
				}
			}
			return (ToolStripItem[])items.ToArray(typeof(ToolStripItem));
		}
	}
}
