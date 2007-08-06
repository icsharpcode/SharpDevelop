// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.TextEditor.Document;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.BuildEngine;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	///  This class is for creating a new "empty" file
	/// </summary>
	public class NewFileDialog : BaseSharpDevelopForm
	{
		ArrayList alltemplates = new ArrayList();
		ArrayList categories   = new ArrayList();
		Hashtable icons        = new Hashtable();
		bool allowUntitledFiles;
		string basePath;
		List<KeyValuePair<string, FileDescriptionTemplate>> createdFiles = new List<KeyValuePair<string, FileDescriptionTemplate>>();
		
		public List<KeyValuePair<string, FileDescriptionTemplate>> CreatedFiles {
			get {
				return createdFiles;
			}
		}
		
		public NewFileDialog(string basePath)
		{
			StandardHeader.SetHeaders();
			this.basePath = basePath;
			this.allowUntitledFiles = basePath == null;
			try {
				InitializeComponents();
				InitializeTemplates();
				InitializeView();
				
				((TreeView)ControlDictionary["categoryTreeView"]).Select();
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
		}
		
		void InitializeView()
		{
			ImageList smalllist  = new ImageList();
			ImageList imglist    = new ImageList();
			smalllist.ColorDepth = ColorDepth.Depth32Bit;
			imglist.ColorDepth   = ColorDepth.Depth32Bit;
			
			imglist.ImageSize    = new Size(32, 32);
			smalllist.ImageSize  = new Size(16, 16);
			
			smalllist.Images.Add(IconService.GetBitmap("Icons.32x32.EmptyFileIcon"));
			imglist.Images.Add(IconService.GetBitmap("Icons.32x32.EmptyFileIcon"));
			
			int i = 0;
			Hashtable tmp = new Hashtable(icons);
			
			foreach (DictionaryEntry entry in icons) {
				Bitmap bitmap = IconService.GetBitmap(entry.Key.ToString());
				if (bitmap != null) {
					smalllist.Images.Add(bitmap);
					imglist.Images.Add(bitmap);
					tmp[entry.Key] = ++i;
				} else {
					LoggingService.Warn("NewFileDialog: can't load bitmap " + entry.Key.ToString() + " using default");
				}
			}
			
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
			
			SelectLastSelectedCategoryNode(((TreeView)ControlDictionary["categoryTreeView"]).Nodes, PropertyService.Get("Dialogs.NewFileDialog.LastSelectedCategory", "C#"));
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
		
		Category GetCategory(string categoryname, string subcategoryname)
		{
			foreach (Category category in categories) {
				if (category.Name == categoryname) {
					if (subcategoryname == null) {
						return category;
					} else {
						return GetSubcategory(category, subcategoryname);
					}
				}
			}
			Category newcategory = new Category(categoryname, TemplateCategorySortOrderFile.GetFileCategorySortOrder(categoryname));
			categories.Add(newcategory);
			if (subcategoryname != null) {
				return GetSubcategory(newcategory, subcategoryname);
			}
			return newcategory;
		}
		
		Category GetSubcategory(Category parentCategory, string name)
		{
			foreach (Category subcategory in parentCategory.Categories) {
				if (subcategory.Name == name)
					return subcategory;
			}
			Category newsubcategory = new Category(name, TemplateCategorySortOrderFile.GetFileCategorySortOrder(parentCategory.Name, name));
			parentCategory.Categories.Add(newsubcategory);
			return newsubcategory;
		}
		
		void InitializeTemplates()
		{
			foreach (FileTemplate template in FileTemplate.FileTemplates) {
				TemplateItem titem = new TemplateItem(template);
				if (titem.Template.Icon != null) {
					icons[titem.Template.Icon] = 0; // "create template icon"
				}
				if (template.NewFileDialogVisible == true) {
					Category cat = GetCategory(StringParser.Parse(titem.Template.Category), StringParser.Parse(titem.Template.Subcategory));
					cat.Templates.Add(titem);
					
					if (cat.Selected == false && template.WizardPath == null) {
						cat.Selected = true;
					}
					if (!cat.HasSelectedTemplate && titem.Template.FileDescriptionTemplates.Count == 1) {
						if (((FileDescriptionTemplate)titem.Template.FileDescriptionTemplates[0]).Name.StartsWith("Empty")) {
							titem.Selected = true;
							cat.HasSelectedTemplate = true;
						}
					}
				}
				alltemplates.Add(titem);
			}
		}
		
		// tree view event handlers
		void CategoryChange(object sender, TreeViewEventArgs e)
		{
			((ListView)ControlDictionary["templateListView"]).Items.Clear();
			HidePropertyGrid();
			if (((TreeView)ControlDictionary["categoryTreeView"]).SelectedNode != null) {
				foreach (TemplateItem item in ((Category)((TreeView)ControlDictionary["categoryTreeView"]).SelectedNode).Templates) {
					((ListView)ControlDictionary["templateListView"]).Items.Add(item);
				}
			}
		}
		
		void OnBeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			e.Node.ImageIndex = 1;
		}
		
		void OnBeforeCollapse(object sender, TreeViewCancelEventArgs e)
		{
			e.Node.ImageIndex = 0;
		}
		
		const int GridWidth = 256;
		const int GridMargin = 8;
		PropertyGrid propertyGrid = new PropertyGrid();
		LocalizedTypeDescriptor localizedTypeDescriptor = null;
		
		bool AllPropertiesHaveAValue {
			get {
				foreach (TemplateProperty property in SelectedTemplate.Properties) {
					string val = StringParser.Properties["Properties." + property.Name];
					if (val == null || val.Length == 0) {
						return false;
					}
				}
				return true;
			}
		}
		
		void ShowPropertyGrid()
		{
			if (localizedTypeDescriptor == null) {
				localizedTypeDescriptor = new LocalizedTypeDescriptor();
			}
			
			if (!Controls.Contains(propertyGrid)) {
				this.SuspendLayout();
				propertyGrid.Location = new Point(Width - GridMargin, GridMargin);
				localizedTypeDescriptor.Properties.Clear();
				foreach (TemplateProperty property in SelectedTemplate.Properties) {
					LocalizedProperty localizedProperty;
					if (property.Type.StartsWith("Types:")) {
						localizedProperty = new LocalizedProperty(property.Name, "System.Enum", property.Category, property.Description);
						TemplateType type = null;
						foreach (TemplateType templateType in SelectedTemplate.CustomTypes) {
							if (templateType.Name == property.Type.Substring("Types:".Length)) {
								type = templateType;
								break;
							}
						}
						if (type == null) {
							throw new Exception("type : " + property.Type + " not found.");
						}
						localizedProperty.TypeConverterObject = new CustomTypeConverter(type);
						StringParser.Properties["Properties." + localizedProperty.Name] = property.DefaultValue;
						localizedProperty.DefaultValue = property.DefaultValue; // localizedProperty.TypeConverterObject.ConvertFrom();
					} else {
						localizedProperty = new LocalizedProperty(property.Name, property.Type, property.Category, property.Description);
						if (property.Type == "System.Boolean") {
							localizedProperty.TypeConverterObject = new BooleanTypeConverter();
							string defVal = property.DefaultValue == null ? null : property.DefaultValue.ToString();
							if (defVal == null || defVal.Length == 0) {
								defVal = "True";
							}
							StringParser.Properties["Properties." + localizedProperty.Name] = defVal;
							localizedProperty.DefaultValue = Boolean.Parse(defVal);
						}
					}
					localizedProperty.LocalizedName = property.LocalizedName;
					localizedTypeDescriptor.Properties.Add(localizedProperty);
				}
				propertyGrid.ToolbarVisible = false;
				propertyGrid.SelectedObject = localizedTypeDescriptor;
				propertyGrid.Size     = new Size(GridWidth, Height - GridMargin * 4);
				
				Width = Width + GridWidth;
				Controls.Add(propertyGrid);
				this.ResumeLayout(false);
			}
		}
		
		void HidePropertyGrid()
		{
			if (Controls.Contains(propertyGrid)) {
				this.SuspendLayout();
				Controls.Remove(propertyGrid);
				Width = Width - GridWidth;
				this.ResumeLayout(false);
			}
		}
		
		FileTemplate SelectedTemplate {
			get {
				if (((ListView)ControlDictionary["templateListView"]).SelectedItems.Count == 1) {
					return ((TemplateItem)((ListView)ControlDictionary["templateListView"]).SelectedItems[0]).Template;
				}
				return null;
			}
		}
		string GenerateCurrentFileName()
		{
			if (SelectedTemplate.DefaultName.IndexOf("${Number}") >= 0) {
				try {
					int curNumber = 1;
					
					while (true) {
						StringParser.Properties["Number"] = curNumber.ToString();
						string fileName = StringParser.Parse(SelectedTemplate.DefaultName);
						if (allowUntitledFiles) {
							bool found = false;
							foreach (string openFile in FileService.GetOpenFiles()) {
								if (Path.GetFileName(openFile) == fileName) {
									found = true;
									break;
								}
							}
							if (found == false)
								break;
						} else if (!File.Exists(Path.Combine(basePath, fileName))) {
							break;
						}
						++curNumber;
					}
				} catch (Exception e) {
					MessageService.ShowError(e);
				}
			}
			return StringParser.Parse(SelectedTemplate.DefaultName);
		}
		
		bool isNameModified = false;
		
		// list view event handlers
		void SelectedIndexChange(object sender, EventArgs e)
		{
			if (((ListView)ControlDictionary["templateListView"]).SelectedItems.Count == 1) {
				ControlDictionary["descriptionLabel"].Text = StringParser.Parse(SelectedTemplate.Description);
				ControlDictionary["openButton"].Enabled = true;
				if (SelectedTemplate.HasProperties) {
					ShowPropertyGrid();
				}
				if (!this.allowUntitledFiles && !isNameModified) {
					ControlDictionary["fileNameTextBox"].Text = GenerateCurrentFileName();
					isNameModified = false;
				}
			} else {
				ControlDictionary["descriptionLabel"].Text = String.Empty;
				ControlDictionary["openButton"].Enabled = false;
				HidePropertyGrid();
			}
		}
		
		void FileNameChanged(object sender, EventArgs e)
		{
			isNameModified = true;
		}
		
		// button events
		
		void CheckedChange(object sender, EventArgs e)
		{
			((ListView)ControlDictionary["templateListView"]).View = ((RadioButton)ControlDictionary["smallIconsRadioButton"]).Checked ? View.List : View.LargeIcon;
		}
		
		public bool IsFilenameAvailable(string fileName)
		{
			if (Path.IsPathRooted(fileName)) {
				return !File.Exists(fileName);
			}
			return true;
		}
		
		public void SaveFile(FileDescriptionTemplate newfile, string content, byte[] binaryContent)
		{
			string parsedFileName = StringParser.Parse(newfile.Name);
			// Parse twice so that tags used in included standard header are parsed
			string parsedContent = StringParser.Parse(StringParser.Parse(content));
			
			if (parsedContent != null) {
				if (SharpDevelopTextEditorProperties.IndentationString != "\t") {
					parsedContent = parsedContent.Replace("\t", SharpDevelopTextEditorProperties.IndentationString);
				}
			}
			
			
			// when newFile.Name is "${Path}/${FileName}", there might be a useless '/' in front of the file name
			// if the file is created when no project is opened. So we remove single '/' or '\', but not double
			// '\\' (project is saved on network share).
			if (parsedFileName.StartsWith("/") && !parsedFileName.StartsWith("//")
			    || parsedFileName.StartsWith("\\") && !parsedFileName.StartsWith("\\\\"))
			{
				parsedFileName = parsedFileName.Substring(1);
			}
			
			if (newfile.IsDependentFile && Path.IsPathRooted(parsedFileName)) {
				Directory.CreateDirectory(Path.GetDirectoryName(parsedFileName));
				if (binaryContent != null)
					File.WriteAllBytes(parsedFileName, binaryContent);
				else
					File.WriteAllText(parsedFileName, parsedContent, ParserService.DefaultFileEncoding);
				ParserService.ParseFile(parsedFileName, parsedContent);
			} else {
				if (binaryContent != null) {
					LoggingService.Warn("binary file was skipped");
					return;
				}
				IWorkbenchWindow window = FileService.NewFile(Path.GetFileName(parsedFileName), StringParser.Parse(newfile.Language), parsedContent);
				if (window == null) {
					return;
				}
				if (Path.IsPathRooted(parsedFileName)) {
					Directory.CreateDirectory(Path.GetDirectoryName(parsedFileName));
					window.ViewContent.Save(parsedFileName);
				}
			}
			createdFiles.Add(new KeyValuePair<string, FileDescriptionTemplate>(parsedFileName, newfile));
		}
		
		internal static string GenerateValidClassOrNamespaceName(string className, bool allowDot)
		{
			if (className == null)
				throw new ArgumentNullException("className");
			className = className.Trim();
			if (className.Length == 0)
				return string.Empty;
			StringBuilder nameBuilder = new StringBuilder();
			if (className[0] != '_' && !char.IsLetter(className, 0))
				nameBuilder.Append('_');
			for (int idx = 0; idx < className.Length; ++idx) {
				if (Char.IsLetterOrDigit(className[idx]) || className[idx] == '_') {
					nameBuilder.Append(className[idx]);
				} else if (className[idx] == '.' && allowDot) {
					nameBuilder.Append('.');
				} else {
					nameBuilder.Append('_');
				}
			}
			return nameBuilder.ToString();
		}
		
		void OpenEvent(object sender, EventArgs e)
		{
			if (((TreeView)ControlDictionary["categoryTreeView"]).SelectedNode != null) {
				
				PropertyService.Set("Dialogs.NewProjectDialog.LargeImages", ((RadioButton)ControlDictionary["largeIconsRadioButton"]).Checked);
				PropertyService.Set("Dialogs.NewFileDialog.LastSelectedCategory", ((TreeView)ControlDictionary["categoryTreeView"]).SelectedNode.Text);
			}
			createdFiles.Clear();
			if (((ListView)ControlDictionary["templateListView"]).SelectedItems.Count == 1) {
				if (!AllPropertiesHaveAValue) {
					MessageService.ShowMessage("${res:Dialog.NewFile.FillOutFirstMessage}", "${res:Dialog.NewFile.FillOutFirstCaption}");
					return;
				}
				TemplateItem item = (TemplateItem)((ListView)ControlDictionary["templateListView"]).SelectedItems[0];
				string fileName;
				StringParser.Properties["StandardNamespace"] = "DefaultNamespace";
				if (allowUntitledFiles) {
					fileName = GenerateCurrentFileName();
				} else {
					fileName = ControlDictionary["fileNameTextBox"].Text.Trim();
					if (!FileUtility.IsValidFileName(fileName)
					    || fileName.IndexOf(Path.AltDirectorySeparatorChar) >= 0
					    || fileName.IndexOf(Path.DirectorySeparatorChar) >= 0)
					{
						MessageService.ShowError(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.SaveFile.InvalidFileNameError}", new string[,] {{"FileName", fileName}}));
						return;
					}
					if (Path.GetExtension(fileName).Length == 0) {
						fileName += Path.GetExtension(item.Template.DefaultName);
					}
					fileName = Path.Combine(basePath, fileName);
					fileName = Path.GetFullPath(fileName);
					IProject project = ProjectService.CurrentProject;
					if (project != null) {
						StringParser.Properties["StandardNamespace"] = CustomToolsService.GetDefaultNamespace(project, fileName);
					}
				}
				StringParser.Properties["FullName"]                 = fileName;
				StringParser.Properties["FileName"]                 = Path.GetFileName(fileName);
				StringParser.Properties["FileNameWithoutExtension"] = Path.GetFileNameWithoutExtension(fileName);
				StringParser.Properties["Extension"]                = Path.GetExtension(fileName);
				StringParser.Properties["Path"]                     = Path.GetDirectoryName(fileName);
				
				StringParser.Properties["ClassName"] = GenerateValidClassOrNamespaceName(Path.GetFileNameWithoutExtension(fileName), false);
				
				
				if (item.Template.WizardPath != null) {
					Properties customizer = new Properties();
					customizer.Set("Template", item.Template);
					customizer.Set("Creator",  this);
					WizardDialog wizard = new WizardDialog("File Wizard", customizer, item.Template.WizardPath);
					if (wizard.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
						DialogResult = DialogResult.OK;
					}
				} else {
					foreach (FileDescriptionTemplate newfile in item.Template.FileDescriptionTemplates) {
						if (!IsFilenameAvailable(StringParser.Parse(newfile.Name))) {
							MessageService.ShowError("Filename " + StringParser.Parse(newfile.Name) + " is in use.\nChoose another one");
							return;
						}
					}
					ScriptRunner scriptRunner = new ScriptRunner();
					
					foreach (FileDescriptionTemplate newfile in item.Template.FileDescriptionTemplates) {
						if (newfile.ContentData != null) {
							SaveFile(newfile, null, newfile.ContentData);
						} else {
							SaveFile(newfile, scriptRunner.CompileScript(item.Template, newfile), null);
						}
					}
					DialogResult = DialogResult.OK;
					
					// raise FileCreated event for the new files
					foreach (KeyValuePair<string, FileDescriptionTemplate> entry in createdFiles) {
						FileService.FireFileCreated(entry.Key);
					}
				}
			}
		}
		
		/// <summary>
		///  Represents a category
		/// </summary>
		public class Category : TreeNode, ICategory
		{
			ArrayList categories = new ArrayList();
			ArrayList templates  = new ArrayList();
			int sortOrder        = TemplateCategorySortOrderFile.UndefinedSortOrder;
			public bool Selected = false;
			public bool HasSelectedTemplate = false;
			
			public Category(string name, int sortOrder) : base(StringParser.Parse(name))
			{
				this.Name = StringParser.Parse(name);
				ImageIndex = 1;
				this.sortOrder = sortOrder;
			}
			
			public Category(string name) : this(name, TemplateCategorySortOrderFile.UndefinedSortOrder)
			{
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
			
			public int SortOrder {
				get {
					return sortOrder;
				}
				set {
					sortOrder = value;
				}
			}
		}
		
		/// <summary>
		///  Represents a new file template
		/// </summary>
		class TemplateItem : ListViewItem
		{
			FileTemplate template;
			
			public TemplateItem(FileTemplate template) : base(StringParser.Parse(template.Name))
			{
				this.template = template;
				ImageIndex    = 0;
			}
			
			public FileTemplate Template {
				get {
					return template;
				}
			}
		}
		
		void InitializeComponents()
		{
			if (allowUntitledFiles) {
				SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.NewFileDialog.xfrm"));
			} else {
				SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.NewFileWithNameDialog.xfrm"));
				ControlDictionary["fileNameTextBox"].TextChanged += new EventHandler(FileNameChanged);
			}
			
			ImageList imglist = new ImageList();
			imglist.ColorDepth = ColorDepth.Depth32Bit;
			imglist.Images.Add(IconService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			imglist.Images.Add(IconService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
			((TreeView)ControlDictionary["categoryTreeView"]).ImageList = imglist;
			
			((TreeView)ControlDictionary["categoryTreeView"]).AfterSelect    += new TreeViewEventHandler(CategoryChange);
			((TreeView)ControlDictionary["categoryTreeView"]).BeforeSelect   += new TreeViewCancelEventHandler(OnBeforeExpand);
			((TreeView)ControlDictionary["categoryTreeView"]).BeforeExpand   += new TreeViewCancelEventHandler(OnBeforeExpand);
			((TreeView)ControlDictionary["categoryTreeView"]).BeforeCollapse += new TreeViewCancelEventHandler(OnBeforeCollapse);
			
			((ListView)ControlDictionary["templateListView"]).SelectedIndexChanged += new EventHandler(SelectedIndexChange);
			((ListView)ControlDictionary["templateListView"]).DoubleClick          += new EventHandler(OpenEvent);
			
			ControlDictionary["openButton"].Click += new EventHandler(OpenEvent);
			
			((RadioButton)ControlDictionary["largeIconsRadioButton"]).Checked = PropertyService.Get("Dialogs.NewProjectDialog.LargeImages", true);
			((RadioButton)ControlDictionary["largeIconsRadioButton"]).CheckedChanged += new EventHandler(CheckedChange);
			((RadioButton)ControlDictionary["largeIconsRadioButton"]).FlatStyle = FlatStyle.Standard;
			((RadioButton)ControlDictionary["largeIconsRadioButton"]).Image  = IconService.GetBitmap("Icons.16x16.LargeIconsIcon");
			
			((RadioButton)ControlDictionary["smallIconsRadioButton"]).Checked = !PropertyService.Get("Dialogs.NewProjectDialog.LargeImages", true);
			((RadioButton)ControlDictionary["smallIconsRadioButton"]).CheckedChanged += new EventHandler(CheckedChange);
			((RadioButton)ControlDictionary["smallIconsRadioButton"]).FlatStyle = FlatStyle.Standard;
			((RadioButton)ControlDictionary["smallIconsRadioButton"]).Image  = IconService.GetBitmap("Icons.16x16.SmallIconsIcon");
			
			ToolTip tooltip = new ToolTip();
			tooltip.SetToolTip(ControlDictionary["largeIconsRadioButton"], StringParser.Parse("${res:Global.LargeIconToolTip}"));
			tooltip.SetToolTip(ControlDictionary["smallIconsRadioButton"], StringParser.Parse("${res:Global.SmallIconToolTip}"));
			tooltip.Active = true;
			Owner         = (Form)WorkbenchSingleton.Workbench;
			StartPosition = FormStartPosition.CenterParent;
			Icon          = null;
			
			CheckedChange(this, EventArgs.Empty);
		}
	}
}
