// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class ErrorListPad : AbstractPadContent
	{
		public const string DefaultContextMenuAddInTreeEntry = "/SharpDevelop/Pads/ErrorList/TaskContextMenu";
		
		static ErrorListPad instance;
		public static ErrorListPad Instance {
			get {
				return instance;
			}
		}
		
		ToolBar toolBar;
		Grid contentPanel = new Grid();
		ListView errorView = new ListView();
		readonly ObservableCollection<SDTask> errors;
		
		Properties properties;
		
		public bool ShowErrors {
			get {
				return properties.Get<bool>("ShowErrors", true);
			}
			set {
				properties.Set<bool>("ShowErrors", value);
				InternalShowResults();
			}
		}
		
		public bool ShowMessages {
			get {
				return properties.Get<bool>("ShowMessages", true);
			}
			set {
				properties.Set<bool>("ShowMessages", value);
				InternalShowResults();
			}
		}
		
		public bool ShowWarnings {
			get {
				return properties.Get<bool>("ShowWarnings", true);
			}
			set {
				properties.Set<bool>("ShowWarnings", value);
				InternalShowResults();
			}
		}
		
		public static bool ShowAfterBuild {
			get {
				return Project.BuildOptions.ShowErrorListAfterBuild;
			}
			set {
				Project.BuildOptions.ShowErrorListAfterBuild = value;
			}
		}
		
		public override object Control {
			get {
				return contentPanel;
			}
		}
		
		public ErrorListPad()
		{
			instance = this;
			properties = PropertyService.NestedProperties("ErrorListPad");
			
			TaskService.Cleared += TaskServiceCleared;
			TaskService.Added   += TaskServiceAdded;
			TaskService.Removed += TaskServiceRemoved;
			TaskService.InUpdateChanged += delegate {
				if (!TaskService.InUpdate)
					InternalShowResults();
			};
			
			SD.BuildService.BuildFinished += ProjectServiceEndBuild;
			SD.ProjectService.SolutionOpened += OnSolutionOpen;
			SD.ProjectService.SolutionClosed += OnSolutionClosed;
			errors = new ObservableCollection<SDTask>(TaskService.Tasks.Where(t => t.TaskType != TaskType.Comment));
			
			toolBar = ToolBarService.CreateToolBar(contentPanel, this, "/SharpDevelop/Pads/ErrorList/Toolbar");
			
			contentPanel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			contentPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
			contentPanel.Children.Add(toolBar);
			contentPanel.Children.Add(errorView);
			Grid.SetRow(errorView, 1);
			errorView.ItemsSource = errors;
			errorView.MouseDoubleClick += ErrorViewMouseDoubleClick;
			errorView.Style = (Style)new TaskViewResources()["TaskListView"];
			errorView.ContextMenu = MenuService.CreateContextMenu(errorView, DefaultContextMenuAddInTreeEntry);
			
			errorView.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, ExecuteCopy, CanExecuteCopy));
			errorView.CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll, ExecuteSelectAll, CanExecuteSelectAll));
			
			InternalShowResults();
		}
		
		void ErrorViewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			SDTask task = errorView.SelectedItem as SDTask;
			var item = errorView.ItemContainerGenerator.ContainerFromItem(task) as ListViewItem;
			UIElement element = e.MouseDevice.DirectlyOver as UIElement;
			if (task != null && task.FileName != null && element != null && item != null
			    && element.IsDescendantOf(item)) {
				SD.FileService.JumpToFilePosition(task.FileName, task.Line, task.Column);
			}
		}
		
		void OnSolutionOpen(object sender, SolutionEventArgs e)
		{
			errors.Clear();
			MenuService.UpdateText(toolBar.Items);
		}
		
		void OnSolutionClosed(object sender, EventArgs e)
		{
			errors.Clear();
			MenuService.UpdateText(toolBar.Items);
		}
		
		void ProjectServiceEndBuild(object sender, EventArgs e)
		{
			if (TaskService.TaskCount > 0 && ShowAfterBuild) {
				SD.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
			}
		}
		
		public BuildResults BuildResults = null;
		
		void AddTask(SDTask task)
		{
			switch (task.TaskType) {
				case TaskType.Warning:
					if (!ShowWarnings) {
						return;
					}
					break;
				case TaskType.Error:
					if (!ShowErrors) {
						return;
					}
					break;
				case TaskType.Message:
					if (!ShowMessages) {
						return;
					}
					break;
				default:
					return;
			}
			
			errors.Add(task);
		}
		
		
		void TaskServiceCleared(object sender, EventArgs e)
		{
			if (TaskService.InUpdate)
				return;
			errors.Clear();
			MenuService.UpdateText(toolBar.Items);
		}
		
		void TaskServiceAdded(object sender, TaskEventArgs e)
		{
			if (TaskService.InUpdate)
				return;
			AddTask(e.Task);
			MenuService.UpdateText(toolBar.Items);
		}
		
		void TaskServiceRemoved(object sender, TaskEventArgs e)
		{
			if (TaskService.InUpdate)
				return;
			errors.Remove(e.Task);
			MenuService.UpdateText(toolBar.Items);
		}
		
		void InternalShowResults()
		{
			errors.Clear();
			
			foreach (SDTask task in TaskService.Tasks) {
				AddTask(task);
			}
			
			MenuService.UpdateText(toolBar.Items);
		}
		
		void CanExecuteCopy(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = errorView.SelectedItem != null;
		}
		
		void ExecuteCopy(object sender, ExecutedRoutedEventArgs e)
		{
			TaskViewResources.CopySelectionToClipboard(errorView);
		}
		
		void CanExecuteSelectAll(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}
		
		void ExecuteSelectAll(object sender, ExecutedRoutedEventArgs e)
		{
			errorView.SelectAll();
		}
	}
}
