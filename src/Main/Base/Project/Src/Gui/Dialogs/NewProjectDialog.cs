// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.Project.Dialogs
{
	/// <summary>
	/// Description of NewProjectDialog.
	/// </summary>
	public partial class NewProjectDialog : Form
	{
		protected List<TemplateItem> alltemplates = new List<TemplateItem>();
		protected List<Category> categories = new List<Category>();
		
		// icon resource name => image index
		protected Dictionary<string, int> icons = new Dictionary<string, int>();
		
		protected bool createNewSolution;
		
		public string InitialProjectLocationDirectory {
			get { return locationTextBox.Text; }
			set { locationTextBox.Text = value; }
		}
		
		public NewProjectDialog(bool createNewSolution)
		{
			StandardHeader.SetHeaders();
			this.createNewSolution = createNewSolution;
			MyInitializeComponents();
			
			InitializeTemplates();
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
			
			smalllist.Images.Add(WinFormsResourceService.GetBitmap("Icons.32x32.EmptyProjectIcon"));
			
			imglist.Images.Add(WinFormsResourceService.GetBitmap("Icons.32x32.EmptyProjectIcon"));
			
			// load the icons and set their index from the image list in the hashtable
			int i = 0;
			Dictionary<string, int> tmp = new Dictionary<string, int>(icons);
			
			foreach (KeyValuePair<string, int> entry in icons) {
				Bitmap bitmap = IconService.GetBitmap(entry.Key);
				if (bitmap != null) {
					smalllist.Images.Add(bitmap);
					imglist.Images.Add(bitmap);
					tmp[entry.Key] = ++i;
				} else {
					LoggingService.Warn("NewProjectDialog: can't load bitmap " + entry.Key.ToString() + " using default");
				}
			}
			
			// set the correct imageindex for all templates
			icons = tmp;
			foreach (TemplateItem item in alltemplates) {
				if (item.Template.Icon == null) {
					item.ImageIndex = 0;
				} else {
					item.ImageIndex = icons[item.Template.Icon];
				}
			}
			
			templateListView.LargeImageList = imglist;
			templateListView.SmallImageList = smalllist;
			
			InsertCategories(null, categories);
			categoryTreeView.TreeViewNodeSorter = new TemplateCategoryComparer();
			categoryTreeView.Sort();
			string initialSelectedCategory = StringParser.Parse("C#\\${res:Templates.File.Categories.WindowsApplications}");
			TreeViewHelper.ApplyViewStateString(PropertyService.Get("Dialogs.NewProjectDialog.CategoryTreeState", ""), categoryTreeView);
			categoryTreeView.SelectedNode = TreeViewHelper.GetNodeByPath(categoryTreeView, PropertyService.Get("Dialogs.NewProjectDialog.LastSelectedCategory", initialSelectedCategory));
		}
		
		
		void InsertCategories(TreeNode node, IEnumerable<Category> catarray)
		{
			foreach (Category cat in catarray) {
				if (node == null) {
					categoryTreeView.Nodes.Add(cat);
				} else {
					node.Nodes.Add(cat);
				}
				InsertCategories(cat, cat.Categories);
			}
		}
		
		protected Category GetCategory(string categoryname, string subcategoryname)
		{
			foreach (Category category in categories) {
				if (category.Text == categoryname) {
					if (subcategoryname == null) {
						return category;
					} else {
						return GetSubcategory(category, subcategoryname);
					}
				}
			}
			Category newcategory = new Category(categoryname, TemplateCategorySortOrderFile.GetProjectCategorySortOrder(categoryname));
			categories.Add(newcategory);
			if (subcategoryname != null) {
				return GetSubcategory(newcategory, subcategoryname);
			}
			return newcategory;
		}
		
		Category GetSubcategory(Category parentCategory, string name)
		{
			foreach (Category subcategory in parentCategory.Categories) {
				if (subcategory.Text == name)
					return subcategory;
			}
			Category newsubcategory = new Category(name, TemplateCategorySortOrderFile.GetProjectCategorySortOrder(parentCategory.Name, name));
			parentCategory.Categories.Add(newsubcategory);
			return newsubcategory;
		}
		
		protected virtual void InitializeTemplates()
		{
			foreach (ProjectTemplate template in ProjectTemplate.ProjectTemplates) {
				if (template.ProjectDescriptor == null && createNewSolution == false) {
					// Do not show solution template when added a new project to existing solution
					continue;
				}
				TemplateItem titem = new TemplateItem(template);
				if (titem.Template.Icon != null) {
					icons[titem.Template.Icon] = 0; // "create template icon"
				}
				if (template.NewProjectDialogVisible == true) {
					Category cat = GetCategory(StringParser.Parse(titem.Template.Category), StringParser.Parse(titem.Template.Subcategory));
					cat.Templates.Add(titem);
					if (cat.Templates.Count == 1)
						titem.Selected = true;
				}
				alltemplates.Add(titem);
			}
		}
		
		protected void CategoryChange(object sender, TreeViewEventArgs e)
		{
			targetFrameworkComboBox.SelectedIndexChanged -= TargetFrameworkComboBoxSelectedIndexChanged;
			targetFrameworkComboBox.Items.Clear();
			if (categoryTreeView.SelectedNode != null) {
				foreach (TargetFramework fx in TargetFramework.TargetFrameworks) {
					if (fx.DisplayName == null)
						continue;
					foreach (TemplateItem item in ((Category)categoryTreeView.SelectedNode).Templates) {
						if (item.Template.HasSupportedTargetFrameworks && item.Template.SupportsTargetFramework(fx)) {
							targetFrameworkComboBox.Items.Add(fx);
							break;
						}
					}
				}
			}
			if (targetFrameworkComboBox.Items.Count > 0) {
				targetFrameworkComboBox.Visible = true;
				targetFrameworkComboBox.SelectedIndex = 0;
				string lastUsedTargetFramework = PropertyService.Get("Dialogs.NewProjectDialog.TargetFramework", TargetFramework.DefaultTargetFrameworkName);
				for (int i = 0; i < targetFrameworkComboBox.Items.Count; i++) {
					if (((TargetFramework)targetFrameworkComboBox.Items[i]).Name == lastUsedTargetFramework) {
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
					if (currentFramework == null || item.Template.SupportsTargetFramework(currentFramework)) {
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
		
		public string NewProjectLocation;
		public string NewSolutionLocation;
		
		string CheckProjectName(string solution, string name, string location)
		{
			if (name.Length == 0 || !char.IsLetter(name[0]) && name[0] != '_') {
				return "${res:ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.ProjectNameMustStartWithLetter}";
			}
			if (!FileUtility.IsValidDirectoryEntryName(solution)
			    || !FileUtility.IsValidDirectoryEntryName(name))
			{
				return "${res:ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.IllegalProjectNameError}";
			}
			if (name.EndsWith(".")) {
				return "${res:ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.ProjectNameMustNotEndWithDot}";
			}
			if (!FileUtility.IsValidPath(location) || !Path.IsPathRooted(location)) {
				return "${res:ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.SpecifyValidLocation}";
			}
			return null;
		}
		
		void OpenEvent(object sender, EventArgs e)
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
				
				ProjectCreateInformation cinfo = new ProjectCreateInformation();
				if (!createNewSolution) {
					cinfo.Solution = ProjectService.OpenSolution;
					cinfo.SolutionPath = Path.GetDirectoryName(cinfo.Solution.FileName);
					cinfo.SolutionName = cinfo.Solution.Name;
				} else {
					cinfo.SolutionPath = NewSolutionDirectory;
				}
				
				if (item.Template.HasSupportedTargetFrameworks) {
					cinfo.TargetFramework = ((TargetFramework)targetFrameworkComboBox.SelectedItem).Name;
					PropertyService.Set("Dialogs.NewProjectDialog.TargetFramework", cinfo.TargetFramework);
				}
				
				cinfo.ProjectBasePath = NewProjectDirectory;
				
				cinfo.SolutionName    = solution;
				cinfo.ProjectName     = name;
				
				NewSolutionLocation = item.Template.CreateProject(cinfo);
				if (NewSolutionLocation == null || NewSolutionLocation.Length == 0) {
					return;
				}
				if (createNewSolution) {
					ProjectService.LoadSolution(NewSolutionLocation);
				}
				item.Template.RunOpenActions(cinfo);
				
				NewProjectLocation = cinfo.createdProjects.Count > 0 ? cinfo.createdProjects[0].FileName : "";
				DialogResult = DialogResult.OK;
			}
		}
		
		void BrowseDirectories(object sender, EventArgs e)
		{
			using (FolderBrowserDialog fd = FileService.CreateFolderBrowserDialog("${res:Dialog.NewProject.SelectDirectoryForProject}", locationTextBox.Text)) {
				if (fd.ShowDialog() == DialogResult.OK) {
					locationTextBox.Text = fd.SelectedPath;
				}
			}
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
			
			openButton.Click += new EventHandler(OpenEvent);
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
		public class Category : TreeNode, ICategory
		{
			List<Category> categories = new List<Category>();
			List<TemplateItem> templates  = new List<TemplateItem>();
			int sortOrder = TemplateCategorySortOrderFile.UndefinedSortOrder;
			
			public Category(string name) : this(name, TemplateCategorySortOrderFile.UndefinedSortOrder)
			{
			}
			
			public Category(string name, int sortOrder) : base(StringParser.Parse(name))
			{
				this.Name = StringParser.Parse(name);
				ImageIndex = 1;
				this.sortOrder = sortOrder;
			}
			
			public int SortOrder {
				get {
					return sortOrder;
				}
				set {
					sortOrder = value;
				}
			}
			public List<Category> Categories {
				get {
					return categories;
				}
			}
			public List<TemplateItem> Templates {
				get {
					return templates;
				}
			}
		}
		
		/// <summary>
		/// Holds a new file template
		/// </summary>
		public class TemplateItem : ListViewItem
		{
			ProjectTemplate template;
			
			public TemplateItem(ProjectTemplate template) : base(StringParser.Parse(template.Name))
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
