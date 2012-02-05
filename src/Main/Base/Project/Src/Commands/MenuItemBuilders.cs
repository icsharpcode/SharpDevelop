// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class NavigationHistoryMenuBuilder : IMenuItemBuilder
	{
		// TODO: refactor BuildSubmenu to add a choice between flat and perfile, eventually per class/method sorting of the list
		
		/*ToolStripItem[] BuildMenuFlat(ICollection<INavigationPoint> points, int additionalItems)
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
		}*/
		List<FrameworkElement> BuildMenuByFile(ICollection<INavigationPoint> points)
		{
			List<FrameworkElement> items = new List<FrameworkElement>();
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
			
			MenuItem containerItem = null;
			MenuItem cmd = null;
			
			foreach (string fname in fileNames) {
				
				// create a menu bucket
				containerItem = new MenuItem();
				containerItem.Header = System.IO.Path.GetFileName(fname);
				containerItem.ToolTip = fname;
				
				// sort and populate the bucket's contents
//				files[fname].Sort();
				foreach(INavigationPoint p in files[fname]) {
					cmd = new MenuItem();
					cmd.Header = p.Description;
					cmd.Click += NavigateTo;
					cmd.Tag = p;
					containerItem.Items.Add(cmd);
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
				items.Add(containerItem);
			}
			
			return items;
		}

		public ICollection BuildItems(Codon codon, object owner)
		{
			if (NavigationService.CanNavigateBack || NavigationService.CanNavigateForwards) {
				ICollection<INavigationPoint> points = NavigationService.Points;

				//ToolStripItem[] items = BuildMenuFlat(points, numberOfAdditionalItems);
				var result = BuildMenuByFile(points);
				
				// additional item 1
				result.Add(new Separator());
				
				// additional item 2
				MenuItem clearHistory = new MenuItem();
				clearHistory.Header = StringParser.Parse("${res:XML.MainMenu.Navigation.ClearHistory}");
				clearHistory.Click += delegate { NavigationService.ClearHistory(); };
				result.Add(clearHistory);
				
				return result;
			}
			
			// default is to disable the dropdown feature...
			return null;
		}
		
		public void NavigateTo(object sender, EventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			NavigationService.Go((INavigationPoint)item.Tag);
		}
	}
	
	public class RecentFilesMenuBuilder : IMenuItemBuilder
	{
		public ICollection BuildItems(Codon codon, object owner)
		{
			RecentOpen recentOpen = FileService.RecentOpen;
			
			if (recentOpen.RecentFile.Count > 0) {
				var items = new System.Windows.Controls.MenuItem[recentOpen.RecentFile.Count];
				
				for (int i = 0; i < recentOpen.RecentFile.Count; ++i) {
					// variable inside loop, so that anonymous method refers to correct recent file
					string recentFile = recentOpen.RecentFile[i];
					string accelaratorKeyPrefix = i < 10 ? "_" + ((i + 1) % 10) + " " : "";
					items[i] = new System.Windows.Controls.MenuItem() {
						Header = accelaratorKeyPrefix + recentFile
					};
					items[i].Click += delegate {
						FileService.OpenFile(recentFile);
					};
				}
				return items;
			} else {
				return new [] { new System.Windows.Controls.MenuItem {
						Header = StringParser.Parse("${res:Dialog.Componnents.RichMenuItem.NoRecentFilesString}"),
						IsEnabled = false
					} };
			}
		}
	}
	
	public class RecentProjectsMenuBuilder : IMenuItemBuilder
	{
		public ICollection BuildItems(Codon codon, object owner)
		{
			RecentOpen recentOpen = FileService.RecentOpen;
			
			if (recentOpen.RecentProject.Count > 0) {
				var items = new System.Windows.Controls.MenuItem[recentOpen.RecentProject.Count];
				
				for (int i = 0; i < recentOpen.RecentProject.Count; ++i) {
					// variable inside loop, so that anonymous method refers to correct recent file
					string recentProject = recentOpen.RecentProject[i];
					string accelaratorKeyPrefix = i < 10 ? "_" + ((i + 1) % 10) + " " : "";
					items[i] = new System.Windows.Controls.MenuItem() {
						Header = accelaratorKeyPrefix + recentProject
					};
					items[i].Click += delegate {
						ProjectService.LoadSolution(recentProject);
					};
				}
				return items;
			} else {
				return new [] { new System.Windows.Controls.MenuItem {
						Header = StringParser.Parse("${res:Dialog.Componnents.RichMenuItem.NoRecentProjectsString}"),
						IsEnabled = false
					} };
			}
		}
	}
	
	public class ToolMenuBuilder : IMenuItemBuilder
	{
		public ICollection BuildItems(Codon codon, object owner)
		{
			var items = new System.Windows.Controls.MenuItem[ToolLoader.Tool.Count];
			for (int i = 0; i < ToolLoader.Tool.Count; ++i) {
				ExternalTool tool = ToolLoader.Tool[i];
				items[i] = new System.Windows.Controls.MenuItem {
					Header = tool.ToString()
				};
				items[i].Click += delegate { RunTool(tool); };
			}
			return items;
		}
		
		void RunTool(ExternalTool tool)
		{
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
	
	public class OpenContentsMenuBuilder : IMenuItemBuilder
	{
		public ICollection BuildItems(Codon codon, object owner)
		{
			int windowCount = WorkbenchSingleton.Workbench.WorkbenchWindowCollection.Count;
			if (windowCount == 0) {
				return new object[] {};
			}
			var items = new object[windowCount + 1];
			items[0] = new System.Windows.Controls.Separator();
			for (int i = 0; i < windowCount; ++i) {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.WorkbenchWindowCollection[i];
				var item = new System.Windows.Controls.MenuItem() {
					IsChecked = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == window,
					IsCheckable = true,
					Header = StringParser.Parse(window.Title).Replace("_", "__")
				};
				item.Click += delegate {
					window.SelectWindow();
				};
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
	
	public abstract class ViewMenuBuilder : IMenuItemBuilder
	{
		class BringPadToFrontCommand : System.Windows.Input.ICommand
		{
			PadDescriptor padDescriptor;
			
			public BringPadToFrontCommand(PadDescriptor padDescriptor)
			{
				this.padDescriptor = padDescriptor;
			}
			
			public event EventHandler CanExecuteChanged { add {} remove {} }
			
			public void Execute(object parameter)
			{
				padDescriptor.BringPadToFront();
			}
			
			public bool CanExecute(object parameter)
			{
				return true;
			}
		}
		
		protected abstract string Category {
			get;
		}
		
		public ICollection BuildItems(Codon codon, object owner)
		{
			ArrayList list = new ArrayList();
			foreach (PadDescriptor padContent in WorkbenchSingleton.Workbench.PadContentCollection) {
				if (padContent.Category == Category) {
					var item = new System.Windows.Controls.MenuItem();
					item.Header = ICSharpCode.Core.Presentation.MenuService.ConvertLabel(StringParser.Parse(padContent.Title));
					if (!string.IsNullOrEmpty(padContent.Icon)) {
						item.Icon = PresentationResourceService.GetImage(padContent.Icon);
					}
					item.Command = new BringPadToFrontCommand(padContent);
					if (!string.IsNullOrEmpty(padContent.Shortcut)) {
						var kg = Core.Presentation.MenuService.ParseShortcut(padContent.Shortcut);
						WorkbenchSingleton.MainWindow.InputBindings.Add(
							new System.Windows.Input.InputBinding(item.Command, kg)
						);
						item.InputGestureText = kg.GetDisplayStringForCulture(Thread.CurrentThread.CurrentUICulture);
					}
					
					list.Add(item);
				}
			}
			return list;
		}
	}
}
