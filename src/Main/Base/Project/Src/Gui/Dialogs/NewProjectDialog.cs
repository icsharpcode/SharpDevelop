// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Project.Dialogs
{
	/// <summary>
	/// This class displays a new project dialog and sets up and creates a a new project,
	/// the project types are described in an XML options file
	/// </summary>
	public class NewProjectDialog : BaseSharpDevelopForm
	{
		protected Container components = new System.ComponentModel.Container();
		
		protected ArrayList alltemplates = new ArrayList();
		protected ArrayList categories   = new ArrayList();
		protected Hashtable icons        = new Hashtable();
		
		protected bool openCombine;
		
		public NewProjectDialog(bool openCombine)
		{
			StandardHeader.SetHeaders();
			this.openCombine = openCombine;
			InitializeComponents();
			
			InitializeTemplates();
			InitializeView();
			
			((TreeView)ControlDictionary["categoryTreeView"]).Select();
			((TextBox)ControlDictionary["locationTextBox"]).Text = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "SharpDevelop Projects"));
			StartPosition = FormStartPosition.CenterParent;
			Icon = null;
		}
		
		public NewProjectDialog()
		{
		}
		
		protected virtual void InitializeView()
		{
			ImageList smalllist = new ImageList();
			ImageList imglist = new ImageList();
			
			smalllist.ColorDepth   = ColorDepth.Depth32Bit;
			imglist.ColorDepth   = ColorDepth.Depth32Bit;
			imglist.ImageSize    = new Size(32, 32);
			smalllist.ImageSize  = new Size(16, 16);
			
			smalllist.Images.Add(ResourceService.GetBitmap("Icons.32x32.EmptyProjectIcon"));
			
			imglist.Images.Add(ResourceService.GetBitmap("Icons.32x32.EmptyProjectIcon"));
			
			// load the icons and set their index from the image list in the hashtable
			int i = 0;
			Hashtable tmp = new Hashtable(icons);
			
			foreach (DictionaryEntry entry in icons) {
				Bitmap bitmap = IconService.GetBitmap(entry.Key.ToString());
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
					item.ImageIndex = (int)icons[item.Template.Icon];
				}
			}
			
			((ListView)ControlDictionary["templateListView"]).LargeImageList = imglist;
			((ListView)ControlDictionary["templateListView"]).SmallImageList = smalllist;
			
			InsertCategories(null, categories);
			((TreeView)ControlDictionary["categoryTreeView"]).TreeViewNodeSorter = new TemplateCategoryComparer();
			((TreeView)ControlDictionary["categoryTreeView"]).Sort();
			SelectLastSelectedCategoryNode(((TreeView)ControlDictionary["categoryTreeView"]).Nodes, PropertyService.Get("Dialogs.NewProjectDialog.LastSelectedCategory", "C#"));
		}
		
		void InsertCategories(TreeNode node, ArrayList catarray)
		{
			foreach (Category cat in catarray) {
				if (node == null) {
					((TreeView)ControlDictionary["categoryTreeView"]).Nodes.Add(cat);
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
			((ListView)ControlDictionary["templateListView"]).Items.Clear();
			if (((TreeView)ControlDictionary["categoryTreeView"]).SelectedNode != null) {
				foreach (TemplateItem item in ((Category)((TreeView)ControlDictionary["categoryTreeView"]).SelectedNode).Templates) {
					((ListView)ControlDictionary["templateListView"]).Items.Add(item);
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
		
		void CheckedChange(object sender, EventArgs e)
		{
			((TextBox)ControlDictionary["solutionNameTextBox"]).ReadOnly = !((CheckBox)ControlDictionary["createSeparateDirCheckBox"]).Checked;
			
			if (((TextBox)ControlDictionary["solutionNameTextBox"]).ReadOnly) { // unchecked created own directory for solution
				NameTextChanged(null, null);    // set the value of the ((TextBox)ControlDictionary["solutionNameTextBox"]) to ((TextBox)ControlDictionary["nameTextBox"])
			}
		}
		
		void NameTextChanged(object sender, EventArgs e)
		{
			if (!((CheckBox)ControlDictionary["createSeparateDirCheckBox"]).Checked) {
				((TextBox)ControlDictionary["solutionNameTextBox"]).Text = ((TextBox)ControlDictionary["nameTextBox"]).Text;
			}
		}
		
		string ProjectSolution {
			get {
				string name = String.Empty;
				if (((CheckBox)ControlDictionary["createSeparateDirCheckBox"]).Checked) {
					name += Path.DirectorySeparatorChar + ((TextBox)ControlDictionary["solutionNameTextBox"]).Text;
				}
				return ProjectLocation + name;
			}
		}
		
		string ProjectLocation {
			get {
				string location = ((TextBox)ControlDictionary["locationTextBox"]).Text.TrimEnd('\\', '/', Path.DirectorySeparatorChar);
				string name     = ((TextBox)ControlDictionary["nameTextBox"]).Text;
				return location + (((CheckBox)ControlDictionary["autoCreateSubDirCheckBox"]).Checked ? Path.DirectorySeparatorChar + name : "");
			}
		}
		
		void PathChanged(object sender, EventArgs e)
		{
			string solutionPath = ProjectSolution;
			try {
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
					solutionPath = ProjectSolution.Substring(0, 3) + (didCut ? "..." : "") + solutionPath;
					if (solutionPath.Length > maxLength + 6) {
						solutionPath = solutionPath.Substring(0, maxLength + 3) + "...";
					}
				}
			} catch (ArgumentException) {
				ControlDictionary["createInLabel"].Text = ResourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.IllegalProjectNameError").Replace("\n", " ").Replace("\r", "");
				return;
			}
			ControlDictionary["createInLabel"].Text = ResourceService.GetString("Dialog.NewProject.ProjectAtDescription")+ " " + solutionPath;
		}
		
		void IconSizeChange(object sender, EventArgs e)
		{
			((ListView)ControlDictionary["templateListView"]).View = ((RadioButton)ControlDictionary["smallIconsRadioButton"]).Checked ? View.List : View.LargeIcon;
		}
		
		public string NewProjectLocation;
		public string NewCombineLocation;
		
		void OpenEvent(object sender, EventArgs e)
		{
			
			if (((TreeView)ControlDictionary["categoryTreeView"]).SelectedNode != null) {
				PropertyService.Set("Dialogs.NewProjectDialog.LastSelectedCategory", ((TreeView)ControlDictionary["categoryTreeView"]).SelectedNode.Text);
				PropertyService.Set("Dialogs.NewProjectDialog.LargeImages", ((RadioButton)ControlDictionary["largeIconsRadioButton"]).Checked);
			}
			
			
			string solution = ((TextBox)ControlDictionary["solutionNameTextBox"]).Text;
			string name     = ((TextBox)ControlDictionary["nameTextBox"]).Text;
			string location = ((TextBox)ControlDictionary["locationTextBox"]).Text;
			if (!FileUtility.IsValidFileName(solution)
			    || solution.IndexOf(Path.DirectorySeparatorChar) >= 0
			    || solution.IndexOf(Path.AltDirectorySeparatorChar) >= 0
			    || !FileUtility.IsValidFileName(name)
			    || name.IndexOf(Path.AltDirectorySeparatorChar) >= 0
			    || name.IndexOf(Path.DirectorySeparatorChar) >= 0
			    || !FileUtility.IsValidFileName(location))
			{
				MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.IllegalProjectNameError}");
				return;
			}
			if (!char.IsLetter(name[0]) && name[0] != '_') {
				MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.ProjectNameMustStartWithLetter}");
				return;
			}
			if (name.EndsWith(".")) {
				MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.ProjectNameMustNotEndWithDot}");
				return;
			}
			
			PropertyService.Set("ICSharpCode.SharpDevelop.Gui.NewProjectDialog.AutoCreateProjectSubdir", ((CheckBox)ControlDictionary["autoCreateSubDirCheckBox"]).Checked);
			if (((ListView)ControlDictionary["templateListView"]).SelectedItems.Count == 1 && ((TextBox)ControlDictionary["locationTextBox"]).Text.Length > 0 && ((TextBox)ControlDictionary["solutionNameTextBox"]).Text.Length > 0) {
				TemplateItem item = (TemplateItem)((ListView)ControlDictionary["templateListView"]).SelectedItems[0];
				try {
					System.IO.Directory.CreateDirectory(ProjectSolution);
				} catch (Exception) {
					
					MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.CantCreateDirectoryError}");
					return;
				}
				
				ProjectCreateInformation cinfo = new ProjectCreateInformation();
				
				cinfo.CombinePath     = ProjectLocation;
				cinfo.ProjectBasePath = ProjectSolution;
				
				cinfo.ProjectName     = ((TextBox)ControlDictionary["nameTextBox"]).Text;
				
				NewCombineLocation = item.Template.CreateProject(cinfo);
				if (NewCombineLocation == null || NewCombineLocation.Length == 0) {
					return;
				}
				if (openCombine) {
					item.Template.OpenCreatedCombine();
				}
				
				NewProjectLocation = cinfo.CreatedProjects.Count > 0 ? cinfo.CreatedProjects[0] : "";
				
				DialogResult = DialogResult.OK;
				/*
						if (item.Template.LanguageName != null && item.Template.LanguageName.Length > 0)  {
							
						}
						
						if (item.Template.WizardPath != null) {
							Properties customizer = new Properties();
							customizer.Set("Template", item.Template);
							customizer.Set("Creator",  this);
							WizardDialog wizard = new WizardDialog("Project Wizard", customizer, item.Template.WizardPath);
							if (wizard.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
								DialogResult = DialogResult.OK;
							}
						}
						
						NewCombineLocation = FileUtility.GetDirectoryNameWithSeparator(ProjectLocation) + ((TextBox)ControlDictionary["nameTextBox"]).Text + ".cmbx";
						
						if (File.Exists(NewCombineLocation)) {
							DialogResult result = MessageBox.Show("Combine file " + NewCombineLocation + " already exists, do you want to overwrite\nthe existing file ?", "File already exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
							switch(result) {
								case DialogResult.Yes:
									cmb.SaveCombine(NewCombineLocation);
									break;
								case DialogResult.No:
									break;
							}
						} else {
							cmb.SaveCombine(NewCombineLocation);
						}
					} else {
						MessageBox.Show(ResourceService.GetString("Dialog.NewProject.EmptyProjectFieldWarning"), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
				 */
			}
		}
		
		void BrowseDirectories(object sender, EventArgs e)
		{
			// Changes Shankar
			FolderDialog fd = new FolderDialog();
			if (fd.DisplayDialog("${res:Dialog.NewProject.SelectDirectoryForProject}") == DialogResult.OK) {
				((TextBox)ControlDictionary["locationTextBox"]).Text = fd.Path;
			}
			// End
		}
		
		// list view event handlers
		void SelectedIndexChange(object sender, EventArgs e)
		{
			if (((ListView)ControlDictionary["templateListView"]).SelectedItems.Count == 1) {
				ControlDictionary["descriptionLabel"].Text = StringParser.Parse(((TemplateItem)((ListView)ControlDictionary["templateListView"]).SelectedItems[0]).Template.Description);
				ControlDictionary["openButton"].Enabled = true;
			} else {
				ControlDictionary["descriptionLabel"].Text = String.Empty;
				ControlDictionary["openButton"].Enabled = false;
			}
		}
		
		TreeNode SelectLastSelectedCategoryNode(TreeNodeCollection nodes, string name)
		{
			foreach (TreeNode node in nodes) {
				if (node.Name == name) {
					((TreeView)ControlDictionary["categoryTreeView"]).SelectedNode = node;
					node.ExpandAll();
					return node;
				}
				TreeNode selectedNode = SelectLastSelectedCategoryNode(node.Nodes, name);
				if (selectedNode != null) {
					return selectedNode;
				}
			}
			return null;
		}
		
		protected void InitializeComponents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.NewProjectDialog.xfrm"));
			
			ImageList imglist = new ImageList();
			imglist.ColorDepth = ColorDepth.Depth32Bit;
			imglist.Images.Add(IconService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			imglist.Images.Add(IconService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
			((TreeView)ControlDictionary["categoryTreeView"]).ImageList = imglist;
			
			((ListView)ControlDictionary["templateListView"]).DoubleClick += new EventHandler(OpenEvent);
			((ListView)ControlDictionary["templateListView"]).SelectedIndexChanged += new EventHandler(SelectedIndexChange);
			((TreeView)ControlDictionary["categoryTreeView"]).AfterSelect    += new TreeViewEventHandler(CategoryChange);
			((TreeView)ControlDictionary["categoryTreeView"]).BeforeSelect   += new TreeViewCancelEventHandler(OnBeforeExpand);
			((TreeView)ControlDictionary["categoryTreeView"]).BeforeExpand   += new TreeViewCancelEventHandler(OnBeforeExpand);
			((TreeView)ControlDictionary["categoryTreeView"]).BeforeCollapse += new TreeViewCancelEventHandler(OnBeforeCollapse);
			((TextBox)ControlDictionary["solutionNameTextBox"]).TextChanged += new EventHandler(PathChanged);
			((TextBox)ControlDictionary["nameTextBox"]).TextChanged += new EventHandler(NameTextChanged);
			((TextBox)ControlDictionary["nameTextBox"]).TextChanged += new EventHandler(PathChanged);
			((TextBox)ControlDictionary["locationTextBox"]).TextChanged += new EventHandler(PathChanged);
			
			
			((RadioButton)ControlDictionary["largeIconsRadioButton"]).Checked = PropertyService.Get("Dialogs.NewProjectDialog.LargeImages", true);
			((RadioButton)ControlDictionary["largeIconsRadioButton"]).CheckedChanged += new EventHandler(IconSizeChange);
			((RadioButton)ControlDictionary["largeIconsRadioButton"]).FlatStyle = FlatStyle.Standard;
			((RadioButton)ControlDictionary["largeIconsRadioButton"]).Image  = IconService.GetBitmap("Icons.16x16.LargeIconsIcon");
			
			((RadioButton)ControlDictionary["smallIconsRadioButton"]).Checked = !PropertyService.Get("Dialogs.NewProjectDialog.LargeImages", true);
			((RadioButton)ControlDictionary["smallIconsRadioButton"]).CheckedChanged += new EventHandler(IconSizeChange);
			((RadioButton)ControlDictionary["smallIconsRadioButton"]).FlatStyle = FlatStyle.Standard;
			((RadioButton)ControlDictionary["smallIconsRadioButton"]).Image  = IconService.GetBitmap("Icons.16x16.SmallIconsIcon");
			
			ControlDictionary["openButton"] .Click += new EventHandler(OpenEvent);
			ControlDictionary["browseButton"].Click += new EventHandler(BrowseDirectories);
			((CheckBox)ControlDictionary["createSeparateDirCheckBox"]).CheckedChanged += new EventHandler(CheckedChange);
			((CheckBox)ControlDictionary["createSeparateDirCheckBox"]).CheckedChanged += new EventHandler(PathChanged);
			((CheckBox)ControlDictionary["autoCreateSubDirCheckBox"]).CheckedChanged  += new EventHandler(PathChanged);
			
			ToolTip tooltip = new ToolTip();
			tooltip.SetToolTip(ControlDictionary["largeIconsRadioButton"], StringParser.Parse("${res:Global.LargeIconToolTip}"));
			tooltip.SetToolTip(ControlDictionary["smallIconsRadioButton"], StringParser.Parse("${res:Global.SmallIconToolTip}"));
			tooltip.Active = true;
			Owner         = (Form)WorkbenchSingleton.Workbench;
			StartPosition = FormStartPosition.CenterParent;
			Icon          = null;
			
			CheckedChange(this, EventArgs.Empty);
			IconSizeChange(this, EventArgs.Empty);
		}
		
		/// <summary>
		///  Represents a category
		/// </summary>
		public class Category : TreeNode, ICategory
		{
			ArrayList categories = new ArrayList();
			ArrayList templates  = new ArrayList();
			int sortOrder        = TemplateCategorySortOrderFile.UndefinedSortOrder;
			
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
			public ArrayList Categories {
				get {
					return categories;
				}
			}
			public ArrayList Templates {
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
	}
}
