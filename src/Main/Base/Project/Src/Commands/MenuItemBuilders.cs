// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class NavigationHistoryMenuBuilder : ISubmenuBuilder
	{
		// TODO: refactor BuildSubmenu to add a choice between flat and perfile, eventually per class/method sorting of the list
		
		ToolStripItem[] BuildMenuFlat(ICollection<INavigationPoint> points, int additionalItems)
		{
			ToolStripItem[] items = new ToolStripItem[points.Count+additionalItems];
			MenuCommand cmd = null;
			INavigationPoint p = null;
			List<INavigationPoint> list = new List<INavigationPoint>(points);
			
			int n = points.Count-1; // the last point
			int i = 0;
			while (i<points.Count) {
				p = list[n-i];
				cmd = new MenuCommand(p.Description, new EventHandler(NavigateTo));
				cmd.Tag = p;
//					if (p == NavigationService.CurrentPosition) {
//						cmd.Text = "*** "+cmd.Text;
//					}
				items[i++] = cmd;
			}
			return items;
		}
		ToolStripItem[] BuildMenuByFile(ICollection<INavigationPoint> points, int additionalItems)
		{
			Dictionary<string, List<INavigationPoint>> files =
				new Dictionary<string, List<INavigationPoint>>();
			List<string> fileNames = new List<string>();
			
			foreach (INavigationPoint p in points) {
				if (p.FileName==null) {
					throw new ApplicationException("should not get here!");
				}
				if (!fileNames.Contains(p.FileName)) {
					fileNames.Add(p.FileName);
					files.Add(p.FileName, new List<INavigationPoint>());
				}
				if (!files[p.FileName].Contains(p)) {
					files[p.FileName].Add(p);
				}
			}
			
			fileNames.Sort();
			
			ToolStripItem[] items =
				new ToolStripItem[fileNames.Count + additionalItems];
			ToolStripMenuItem containerItem = null;
			MenuCommand cmd = null;
			int i = 0;
			
			foreach (string fname in fileNames) {
				
				// create a menu bucket
				containerItem = new ToolStripMenuItem();
				containerItem.Text = System.IO.Path.GetFileName(fname);
				containerItem.ToolTipText = fname;
				
				// sort and populate the bucket's contents
//				files[fname].Sort();
				foreach(INavigationPoint p in files[fname]) {
					cmd = new MenuCommand(p.Description, new EventHandler(NavigateTo));
					cmd.Tag = p;
					containerItem.DropDownItems.Add(cmd);
				}
				
				// if there's only one nested item, add it
				// to the result directly, ignoring the bucket
//				if (containerItem.DropDownItems.Count==1) {
//					items[i] = containerItem.DropDownItems[0];
//					items[i].Text = ((INavigationPoint)items[i].Tag).FullDescription;
//					i++;
//				} else {
//					// add the bucket to the result
//					items[i++] = containerItem;
//				}
				// add the bucket to the result
				items[i++] = containerItem;
			}
			
			return items;
		}

		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			MenuCommand cmd = null;
			if (NavigationService.CanNavigateBack || NavigationService.CanNavigateForwards) {
				ICollection<INavigationPoint> points = NavigationService.Points;

				//ToolStripItem[] items = BuildMenuFlat(points, numberOfAdditionalItems);
				ToolStripItem[] items = BuildMenuByFile(points, numberOfAdditionalItems);
				
				int i = items.Length - numberOfAdditionalItems;
				
				// additional item 1
				items[i++] = new ToolStripSeparator();
				
				// additional item 2
				cmd = new MenuCommand("${res:XML.MainMenu.Navigation.ClearHistory}", new EventHandler(ClearHistory));
				items[i++] = cmd;
				
				return items;
			}
			
			// default is to disable the dropdown feature...
			return null;
		}

		int numberOfAdditionalItems = 2;
		
		public void NavigateTo(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			NavigationService.Go((INavigationPoint)item.Tag);
		}
		
		public void ClearHistory(object sender, EventArgs e)
		{
			NavigationService.ClearHistory();
		}
	}
	
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
		
		/// <summary>
		/// This handler gets called when a tool in the Tool menu is clicked on.
		/// </summary>
		/// <param name="sender">The MenuCommand that sent the event.</param>
		/// <param name="e">Event arguments.</param>
		void ToolEvt(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			
			// TODO: ToolLoader.Tool should get a string indexor. Overloading List or making it a Dictionary<string,ExternalTool> would work.
			for (int i = 0; i < ToolLoader.Tool.Count; ++i) {
				if (item.Text != ToolLoader.Tool[i].ToString()) { continue; }
				ExternalTool tool = (ExternalTool)ToolLoader.Tool[i];
				
				// Set these to somewhat useful values in case StingParser.Parse() passes when being called on one of them.
				string command = tool.Command;
				string args = tool.Arguments;

				// This needs it's own try/catch because if parsing these messages fail, the catch block after
				// the second try would also throw because MessageService.ShowError() calls StringParser.Parse()
				try {
					command = StringParser.Parse(tool.Command);
					args    = StringParser.Parse(tool.Arguments);
				} catch (Exception ex) {
					MessageService.ShowError("${res:XML.MainMenu.ToolMenu.ExternalTools.ExecutionFailed} '" + ex.Message);
					return;
				}
				
				if (tool.PromptForArguments) {
					args = MessageService.ShowInputBox(tool.MenuCommand, "${res:XML.MainMenu.ToolMenu.ExternalTools.EnterArguments}", args);
					if (args == null)
						return;
				}
				
				try {
					if (tool.UseOutputPad) {
						ProcessRunner processRunner = new ProcessRunner();
						processRunner.LogStandardOutputAndError = false;
						processRunner.ProcessExited += ProcessExitEvent;
						processRunner.OutputLineReceived += process_OutputLineReceived;
						processRunner.ErrorLineReceived += process_OutputLineReceived;
						processRunner.WorkingDirectory = StringParser.Parse(tool.InitialDirectory);
						if (args == null || args.Length == 0 || args.Trim('"', ' ').Length == 0) {
							processRunner.Start(command);
						} else {
							processRunner.Start(command, args);
						}
					} else {
						ProcessStartInfo startinfo;
						if (args == null || args.Length == 0 || args.Trim('"', ' ').Length == 0) {
							startinfo = new ProcessStartInfo(command);
						} else {
							startinfo = new ProcessStartInfo(command, args);
						}
						startinfo.WorkingDirectory = StringParser.Parse(tool.InitialDirectory);
						Process process = new Process();
						process.StartInfo = startinfo;
						process.Start();
					}
				} catch (Exception ex) {
					MessageService.ShowError("${res:XML.MainMenu.ToolMenu.ExternalTools.ExecutionFailed} '" + command + " " + args + "'\n" + ex.Message);
				}
				return;
			}
		}

		void ProcessExitEvent(object sender, EventArgs e)
		{
			WorkbenchSingleton.SafeThreadAsyncCall(
				delegate {
					ProcessRunner p = (ProcessRunner)sender;
					TaskService.BuildMessageViewCategory.AppendLine(StringParser.Parse("${res:XML.MainMenu.ToolMenu.ExternalTools.ExitedWithCode} " + p.ExitCode));
					p.Dispose();
				});
		}
		
		void process_OutputLineReceived(object sender, LineReceivedEventArgs e)
		{
			TaskService.BuildMessageViewCategory.AppendLine(e.Line);
		}
	}
	
	public class OpenContentsMenuBuilder : ISubmenuBuilder
	{
		
		class MyMenuItem : MenuCheckBox
		{
			IWorkbenchWindow window;
			
			public MyMenuItem(IWorkbenchWindow window) : base(StringParser.Parse(window.Title))
			{
				this.window = window;
			}
			
			protected override void OnClick(EventArgs e)
			{
				base.OnClick(e);
				Checked = true;
				window.SelectWindow();
			}
		}

		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			int windowCount = WorkbenchSingleton.Workbench.WorkbenchWindowCollection.Count;
			if (windowCount == 0) {
				return new ToolStripItem[] {};
			}
			ToolStripItem[] items = new ToolStripItem[windowCount + 1];
			items[0] = new MenuSeparator(null, null);
			for (int i = 0; i < windowCount; ++i) {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.WorkbenchWindowCollection[i];
				MenuCheckBox item = new MyMenuItem(window);
				item.Tag = window;
				item.Checked = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == window;
				item.Description = "Activate this window";
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
					ShortcutKeys = MenuCommand.ParseShortcut(padDescriptor.Shortcut);
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
			List<ToolStripItem> items = new List<ToolStripItem>();
			foreach (PadDescriptor padContent in WorkbenchSingleton.Workbench.PadContentCollection) {
				if (padContent.Category == Category) {
					items.Add(new MyMenuItem(padContent));
				}
			}
			return items.ToArray();
		}
	}
}
