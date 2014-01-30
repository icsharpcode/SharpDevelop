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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project.Commands;
using ICSharpCode.SharpDevelop.WinForms;
using Microsoft.Build.Exceptions;

namespace ICSharpCode.SharpDevelop.Project.Dialogs
{
	using ICSharpCode.SharpDevelop.Templates;

	internal partial class NewProjectDialog : Form
	{
		protected List<TemplateItem> alltemplates = new List<TemplateItem>();
		internal ProjectTemplateResult result;
		
		// icon resource name => image index
		protected Dictionary<IImage, int> icons = new Dictionary<IImage, int>();
		
		protected bool createNewSolution;
		
		public string InitialProjectLocationDirectory {
			get { return locationTextBox.Text; }
			set { locationTextBox.Text = value; }
		}
		
		public NewProjectDialog(IEnumerable<TemplateCategory> templateCategories, bool createNewSolution)
		{
			this.createNewSolution = createNewSolution;
			MyInitializeComponents();
			
			InitializeTemplates(templateCategories);
			InitializeView();
			
			locationTextBox.Text = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "SharpDevelop Projects"));
		}
		
		protected virtual void InitializeView()
		{
			ImageList smalllist = new ImageList();
			ImageList imglist = new ImageList();
			
			smalllist.ColorDepth   = ColorDepth.Depth32Bit;
			imglist.ColorDepth   = ColorDepth.Depth32Bit;
			imglist.ImageSize    = new Size(32, 32);
			smalllist.ImageSize  = new Size(16, 16);
			
			smalllist.Images.Add(SD.ResourceService.GetBitmap("Icons.32x32.EmptyProjectIcon"));
			
			imglist.Images.Add(SD.ResourceService.GetBitmap("Icons.32x32.EmptyProjectIcon"));
			
			// load the icons and set their index from the image list in the hashtable
			int i = 0;
			
			foreach (IImage icon in icons.Keys.ToArray()) {
				Bitmap bitmap = icon.Bitmap;
				if (bitmap != null) {
					smalllist.Images.Add(bitmap);
					imglist.Images.Add(bitmap);
					icons[icon] = ++i;
				} else {
					LoggingService.Warn("NewProjectDialog: can't load bitmap " + icon + " using default");
				}
			}
			
			// set the correct imageindex for all templates
			foreach (TemplateItem item in alltemplates) {
				if (item.Template.Icon == null) {
					item.ImageIndex = 0;
				} else {
					item.ImageIndex = icons[item.Template.Icon];
				}
			}
			
			templateListView.LargeImageList = imglist;
			templateListView.SmallImageList = smalllist;
			
			string initialSelectedCategory = StringParser.Parse("C#\\${res:Templates.File.Categories.WindowsApplications}");
			TreeViewHelper.ApplyViewStateString(PropertyService.Get("Dialogs.NewProjectDialog.CategoryTreeState", ""), categoryTreeView);
			categoryTreeView.SelectedNode = TreeViewHelper.GetNodeByPath(categoryTreeView, PropertyService.Get("Dialogs.NewProjectDialog.LastSelectedCategory", initialSelectedCategory));
		}
		
		
		IEnumerable<TemplateCategory> Sorted(IEnumerable<TemplateCategory> templateCategories)
		{
			return templateCategories.OrderByDescending(c => c.SortOrder).ThenBy(c => StringParser.Parse(c.DisplayName));
		}
		
		protected virtual void InitializeTemplates(IEnumerable<TemplateCategory> templateCategories)
		{
			foreach (var templateCategory in Sorted(templateCategories)) {
				var cat = CreateCategory(templateCategory);
				if (!cat.IsEmpty)
					categoryTreeView.Nodes.Add(cat);
			}
		}
		
		Category CreateCategory(TemplateCategory templateCategory)
		{
			Category node = new Category(templateCategory.DisplayName);
			foreach (var subcategory in Sorted(templateCategory.Subcategories)) {
				var subnode = CreateCategory(subcategory);
				if (!subnode.IsEmpty)
					node.Nodes.Add(subnode);
			}
			foreach (var template in templateCategory.Templates.OfType<ProjectTemplate>()) {
				if (!template.IsVisible(SolutionFolder != null ? SolutionFolder.ParentSolution : null)) {
					// Do not show solution template when added a new project to existing solution
					continue;
				}
				TemplateItem titem = new TemplateItem(template);
				if (titem.Template.Icon != null) {
					icons[titem.Template.Icon] = 0; // "create template icon"
				}
				alltemplates.Add(titem);
				node.Templates.Add(titem);
			}
			return node;
		}
		
		protected void CategoryChange(object sender, TreeViewEventArgs e)
		{
			targetFrameworkComboBox.SelectedIndexChanged -= TargetFrameworkComboBoxSelectedIndexChanged;
			targetFrameworkComboBox.Items.Clear();
			if (categoryTreeView.SelectedNode != null) {
				HashSet<TargetFramework> availableTargetFrameworks = new HashSet<TargetFramework>();
				foreach (TemplateItem item in ((Category)categoryTreeView.SelectedNode).Templates) {
					availableTargetFrameworks.UnionWith(item.Template.SupportedTargetFrameworks);
				}
				targetFrameworkComboBox.Items.AddRange(
					availableTargetFrameworks.Where(fx => fx.DisplayName != null && fx.IsAvailable())
					.OrderBy(fx => fx.TargetFrameworkVersion).ToArray());
			}
			if (targetFrameworkComboBox.Items.Count > 0) {
				targetFrameworkComboBox.Visible = true;
				targetFrameworkComboBox.SelectedIndex = 0;
				string lastUsedTargetFramework = PropertyService.Get("Dialogs.NewProjectDialog.TargetFramework", TargetFramework.DefaultTargetFrameworkVersion);
				string lastUsedTargetFrameworkProfile = PropertyService.Get("Dialogs.NewProjectDialog.TargetFrameworkProfile", TargetFramework.DefaultTargetFrameworkProfile);
				for (int i = 0; i < targetFrameworkComboBox.Items.Count; i++) {
					var targetFramework = (TargetFramework)targetFrameworkComboBox.Items[i];
					if (targetFramework.TargetFrameworkVersion == lastUsedTargetFramework && targetFramework.TargetFrameworkProfile == lastUsedTargetFrameworkProfile) {
						targetFrameworkComboBox.SelectedIndex = i;
						break;
					}
				}
			} else {
				targetFrameworkComboBox.Visible = false;
			}
			TargetFrameworkComboBoxSelectedIndexChanged(sender, e);
			targetFrameworkComboBox.SelectedIndexChanged += TargetFrameworkComboBoxSelectedIndexChanged;
		}
		
		void TargetFrameworkComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			templateListView.Items.Clear();
			if (categoryTreeView.SelectedNode != null) {
				TargetFramework currentFramework = targetFrameworkComboBox.SelectedItem as TargetFramework;
				foreach (TemplateItem item in ((Category)categoryTreeView.SelectedNode).Templates) {
					if (currentFramework == null || item.Template.SupportedTargetFrameworks.Contains(currentFramework) || !item.Template.SupportedTargetFrameworks.Any()) {
						templateListView.Items.Add(item);
					}
				}
			}
			this.SelectedIndexChange(sender, e);
		}
		
		void OnBeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			e.Node.ImageIndex = 1;
		}
		
		void OnBeforeCollapse(object sender, TreeViewCancelEventArgs e)
		{
			e.Node.ImageIndex = 0;
		}
		
		string NewProjectDirectory {
			get {
				if (createDirectoryForSolutionCheckBox.Checked) {
					return Path.Combine(NewSolutionDirectory, nameTextBox.Text.Trim());
				} else {
					return NewSolutionDirectory;
				}
			}
		}
		
		string NewSolutionDirectory {
			get {
				string location = locationTextBox.Text;
				string name = createDirectoryForSolutionCheckBox.Checked ? solutionNameTextBox.Text : nameTextBox.Text;
				return Path.Combine(location.Trim(), name.Trim());
			}
		}
		
		void PathChanged(object sender, EventArgs e)
		{
			string solution = solutionNameTextBox.Text.Trim();
			string name     = nameTextBox.Text.Trim();
			string location = locationTextBox.Text.Trim();
			string projectNameError = CheckProjectName(solution, name, location);
			if (projectNameError != null) {
				createInLabel.Text = StringParser.Parse(projectNameError).Replace("\n", " ").Replace("\r", "");
				return;
			}
			
			string solutionPath;
			try {
				solutionPath = NewProjectDirectory;
				if (solutionPath.Length > 3 && Path.IsPathRooted(solutionPath)) {
					solutionPath = solutionPath.Substring(3);
					bool didCut = false;
					const int maxLength = 62;
					while (solutionPath.Length > maxLength && solutionPath.Length > 1) {
						int idx = solutionPath.IndexOf(Path.DirectorySeparatorChar, 1);
						if (idx < 0) {
							break;
						}
						solutionPath = solutionPath.Substring(idx);
						didCut = true;
					}
					solutionPath = NewProjectDirectory.Substring(0, 3) + (didCut ? "..." : "") + solutionPath;
					if (solutionPath.Length > maxLength + 6) {
						solutionPath = solutionPath.Substring(0, maxLength + 3) + "...";
					}
				}
			} catch (ArgumentException) {
				createInLabel.Text = ResourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.IllegalProjectNameError").Replace("\n", " ").Replace("\r", "");
				return;
			}
			createInLabel.Text = ResourceService.GetString("Dialog.NewProject.ProjectAtDescription")+ " " + solutionPath;
		}
		
		void IconSizeChange(object sender, EventArgs e)
		{
			templateListView.View = smallIconsRadioButton.Checked ? View.List : View.LargeIcon;
		}
		
		public ISolutionFolder SolutionFolder;
		
		string CheckProjectName(string solution, string name, string location)
		{
			if (name.Length == 0 || !char.IsLetter(name[0]) && name[0] != '_') {
				return "${res:ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.ProjectNameMustStartWithLetter}";
			}
			if (name.EndsWith(".", StringComparison.Ordinal)) {
				return "${res:ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.ProjectNameMustNotEndWithDot}";
			}
			if (!FileUtility.IsValidDirectoryEntryName(solution)
			    || !FileUtility.IsValidDirectoryEntryName(name))
			{
				return "${res:ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.IllegalProjectNameError}";
			}
			if (!FileUtility.IsValidPath(location) || !Path.IsPathRooted(location)) {
				return "${res:ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.SpecifyValidLocation}";
			}
			return null;
		}
		
		void TryCreateProject(object sender, EventArgs e)
		{
			try {
				CreateProject();
			} catch (ProjectLoadException ex) {
				LoggingService.Error("Unable to create new project.", ex);
				MessageService.ShowError(ex.Message);
			} catch (IOException ex) {
				LoggingService.Error("Unable to create new project.", ex);
				MessageService.ShowError(ex.Message);
			}
		}
		
		void CreateProject()
		{
			if (categoryTreeView.SelectedNode != null) {
				PropertyService.Set("Dialogs.NewProjectDialog.LastSelectedCategory", TreeViewHelper.GetPath(categoryTreeView.SelectedNode));
				PropertyService.Set("Dialogs.NewProjectDialog.CategoryTreeState", TreeViewHelper.GetViewStateString(categoryTreeView));
				PropertyService.Set("Dialogs.NewProjectDialog.LargeImages", largeIconsRadioButton.Checked);
			}
			
			string solution = solutionNameTextBox.Text.Trim();
			string name     = nameTextBox.Text.Trim();
			string location = locationTextBox.Text.Trim();
			string projectNameError = CheckProjectName(solution, name, location);
			if (projectNameError != null) {
				MessageService.ShowError(projectNameError);
				return;
			}
			
			if (templateListView.SelectedItems.Count == 1 && locationTextBox.Text.Length > 0 && solutionNameTextBox.Text.Length > 0) {
				TemplateItem item = (TemplateItem)templateListView.SelectedItems[0];
				try {
					System.IO.Directory.CreateDirectory(NewProjectDirectory);
				} catch (Exception) {
					MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.CantCreateDirectoryError}");
					return;
				}
				
				ProjectTemplateOptions cinfo = new ProjectTemplateOptions();
				
				if (item.Template.SupportedTargetFrameworks.Any()) {
					cinfo.TargetFramework = (TargetFramework)targetFrameworkComboBox.SelectedItem;
					PropertyService.Set("Dialogs.NewProjectDialog.TargetFramework", cinfo.TargetFramework.TargetFrameworkVersion);
				}
				
				cinfo.ProjectBasePath = DirectoryName.Create(NewProjectDirectory);
				cinfo.ProjectName     = name;
				
				if (createNewSolution) {
					if (!SD.ProjectService.CloseSolution())
						return;
					result = item.Template.CreateAndOpenSolution(cinfo, NewSolutionDirectory, solution);
				} else {
					cinfo.Solution = SolutionFolder.ParentSolution;
					cinfo.SolutionFolder = SolutionFolder;
					result = item.Template.CreateProjects(cinfo);
					cinfo.Solution.Save();
				}
				
				if (result != null)
					item.Template.RunOpenActions(result);
				
				ProjectBrowserPad.RefreshViewAsync();
				DialogResult = DialogResult.OK;
			}
		}
		
		void BrowseDirectories(object sender, EventArgs e)
		{
			string path = SD.FileService.BrowseForFolder("${res:Dialog.NewProject.SelectDirectoryForProject}", locationTextBox.Text);
			if (path != null)
				locationTextBox.Text = path;
		}
		
		// list view event handlers
		void SelectedIndexChange(object sender, EventArgs e)
		{
			if (templateListView.SelectedItems.Count == 1) {
				descriptionLabel.Text = StringParser.Parse(((TemplateItem)templateListView.SelectedItems[0]).Template.Description);
				openButton.Enabled = true;
			} else {
				descriptionLabel.Text = String.Empty;
				openButton.Enabled = false;
			}
		}
		
		static bool CreateDirectoryForSolution {
			get { return PropertyService.Get("Dialogs.NewProjectDialog.CreateDirectoryForSolution", true); }
			set { PropertyService.Set("Dialogs.NewProjectDialog.CreateDirectoryForSolution", value); }
		}
		
		protected void MyInitializeComponents()
		{
			InitializeComponent();
			
			foreach (Control ctl in Controls.GetRecursive()) {
				ctl.Text = StringParser.Parse(ctl.Text);
			}
			this.Text = StringParser.Parse(this.Text);
			SD.WinForms.ApplyRightToLeftConverter(this, recurse: false);
			
			ImageList imglist = new ImageList();
			imglist.ColorDepth = ColorDepth.Depth32Bit;
			imglist.Images.Add(IconService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			imglist.Images.Add(IconService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
			categoryTreeView.ImageList = imglist;
			
			templateListView.SelectedIndexChanged += new EventHandler(SelectedIndexChange);
			categoryTreeView.AfterSelect    += new TreeViewEventHandler(CategoryChange);
			categoryTreeView.BeforeSelect   += new TreeViewCancelEventHandler(OnBeforeExpand);
			categoryTreeView.BeforeExpand   += new TreeViewCancelEventHandler(OnBeforeExpand);
			categoryTreeView.BeforeCollapse += new TreeViewCancelEventHandler(OnBeforeCollapse);
			solutionNameTextBox.TextChanged += new EventHandler(PathChanged);
			nameTextBox.TextChanged += new EventHandler(PathChanged);
			locationTextBox.TextChanged += new EventHandler(PathChanged);
			
			if (createNewSolution) {
				createDirectoryForSolutionCheckBox.Checked = CreateDirectoryForSolution;
				createDirectoryForSolutionCheckBox.CheckedChanged += delegate {
					CreateDirectoryForSolution = createDirectoryForSolutionCheckBox.Checked;
				};
			} else {
				solutionNameTextBox.Visible = false;
				solutionNameLabel.Visible = false;
				createDirectoryForSolutionCheckBox.Visible = false;
				bottomPanel.Height -= solutionNameTextBox.Height;
				
				createDirectoryForSolutionCheckBox.Checked = false;
			}
			
			largeIconsRadioButton.Checked = PropertyService.Get("Dialogs.NewProjectDialog.LargeImages", true);
			largeIconsRadioButton.CheckedChanged += new EventHandler(IconSizeChange);
			largeIconsRadioButton.Image = IconService.GetBitmap("Icons.16x16.LargeIconsIcon");
			
			smallIconsRadioButton.Checked = !PropertyService.Get("Dialogs.NewProjectDialog.LargeImages", true);
			smallIconsRadioButton.CheckedChanged += new EventHandler(IconSizeChange);
			smallIconsRadioButton.Image = IconService.GetBitmap("Icons.16x16.SmallIconsIcon");
			
			openButton.Click += new EventHandler(TryCreateProject);
			browseButton.Click += new EventHandler(BrowseDirectories);
			createDirectoryForSolutionCheckBox.CheckedChanged += new EventHandler(PathChanged);
			
			ToolTip tooltip = new ToolTip();
			tooltip.SetToolTip(largeIconsRadioButton, StringParser.Parse("${res:Global.LargeIconToolTip}"));
			tooltip.SetToolTip(smallIconsRadioButton, StringParser.Parse("${res:Global.SmallIconToolTip}"));
			tooltip.Active = true;
			
			IconSizeChange(this, EventArgs.Empty);
		}
		
		/// <summary>
		///  Represents a category
		/// </summary>
		protected class Category : TreeNode
		{
			List<TemplateItem> templates  = new List<TemplateItem>();
			
			public Category(string name) : base(StringParser.Parse(name))
			{
				this.Name = StringParser.Parse(name);
				ImageIndex = 1;
			}
			
			public List<TemplateItem> Templates {
				get {
					return templates;
				}
			}
			
			public bool IsEmpty {
				get { return templates.Count == 0 && Nodes.Count == 0; }
			}
		}
		
		/// <summary>
		/// Holds a new file template
		/// </summary>
		public class TemplateItem : ListViewItem
		{
			ProjectTemplate template;
			
			public TemplateItem(ProjectTemplate template) : base(template.DisplayName)
			{
				this.template = template;
				ImageIndex = 0;
			}
			
			public ProjectTemplate Template {
				get {
					return template;
				}
			}
		}
		
		string oldProjectName;
		
		void NameTextBoxTextChanged(object sender, EventArgs e)
		{
			if (solutionNameTextBox.Text == oldProjectName || solutionNameTextBox.TextLength == 0) {
				solutionNameTextBox.Text = oldProjectName = nameTextBox.Text;
			}
		}
	}
}
