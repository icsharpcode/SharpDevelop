// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

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
				
				if (allowUntitledFiles)
					categoryTreeView.Select();
				else
					ControlDictionary["fileNameTextBox"].Select();
			} catch (Exception e) {
				MessageService.ShowException(e);
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
			
			templateListView.LargeImageList = imglist;
			templateListView.SmallImageList = smalllist;
			
			InsertCategories(null, categories);
			
			categoryTreeView.TreeViewNodeSorter = new TemplateCategoryComparer();
			categoryTreeView.Sort();
			
			TreeViewHelper.ApplyViewStateString(PropertyService.Get("Dialogs.NewFileDialog.CategoryViewState", ""), categoryTreeView);
			categoryTreeView.SelectedNode = TreeViewHelper.GetNodeByPath(categoryTreeView, PropertyService.Get("Dialogs.NewFileDialog.LastSelectedCategory", "C#"));
		}
		
		ListView templateListView;
		TreeView categoryTreeView;
		
		void InsertCategories(TreeNode node, ArrayList catarray)
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
					
					cat.Selected = true;
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
			templateListView.Items.Clear();
			HidePropertyGrid();
			if (categoryTreeView.SelectedNode != null) {
				foreach (TemplateItem item in ((Category)categoryTreeView.SelectedNode).Templates) {
					templateListView.Items.Add(item);
				}
			}
			
			string activeTemplate = PropertyService.Get("Dialogs.NewFileDialog.LastSelectedTemplate", "");
			foreach (TemplateItem item in templateListView.Items) {
				if (item.Template.Name == activeTemplate)
					item.Selected = true;
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
					string val = StringParserPropertyContainer.LocalizedProperty["Properties." + property.Name];
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
				propertyGrid.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
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
						StringParserPropertyContainer.LocalizedProperty["Properties." + localizedProperty.Name] = property.DefaultValue;
						localizedProperty.DefaultValue = property.DefaultValue; // localizedProperty.TypeConverterObject.ConvertFrom();
					} else {
						localizedProperty = new LocalizedProperty(property.Name, property.Type, property.Category, property.Description);
						if (property.Type == "System.Boolean") {
							localizedProperty.TypeConverterObject = new BooleanTypeConverter();
							string defVal = property.DefaultValue == null ? null : property.DefaultValue.ToString();
							if (defVal == null || defVal.Length == 0) {
								defVal = "True";
							}
							StringParserPropertyContainer.LocalizedProperty["Properties." + localizedProperty.Name] = defVal;
							localizedProperty.DefaultValue = Boolean.Parse(defVal);
						} else {
							string defVal = property.DefaultValue == null ? String.Empty : property.DefaultValue.ToString();
							StringParserPropertyContainer.LocalizedProperty["Properties." + localizedProperty.Name] = defVal;
							localizedProperty.DefaultValue = defVal;
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
				if (templateListView.SelectedItems.Count == 1) {
					return ((TemplateItem)templateListView.SelectedItems[0]).Template;
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
						string fileName = StringParser.Parse(SelectedTemplate.DefaultName, new StringTagPair("Number", curNumber.ToString()));
						if (allowUntitledFiles) {
							bool found = false;
							foreach (string openFile in FileService.GetOpenFiles()) {
								if (Path.GetFileName(openFile) == fileName) {
									found = true;
									break;
								}
							}
							if (found == false)
								return fileName;
						} else if (!File.Exists(Path.Combine(basePath, fileName))) {
							return fileName;
						}
						++curNumber;
					}
				} catch (Exception e) {
					MessageService.ShowException(e);
				}
			}
			return StringParser.Parse(SelectedTemplate.DefaultName);
		}
		
		bool isNameModified = false;
		
		// list view event handlers
		void SelectedIndexChange(object sender, EventArgs e)
		{
			if (templateListView.SelectedItems.Count == 1) {
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
			templateListView.View = ((RadioButton)ControlDictionary["smallIconsRadioButton"]).Checked ? View.List : View.LargeIcon;
		}
		
		public bool IsFilenameAvailable(string fileName)
		{
			if (Path.IsPathRooted(fileName)) {
				return !File.Exists(fileName);
			}
			return true;
		}
		
		public void SaveFile(FileDescriptionTemplate newfile, string content, string binaryFileName)
		{
			string parsedFileName = StringParser.Parse(newfile.Name);
			// Parse twice so that tags used in included standard header are parsed
			string parsedContent = StringParser.Parse(StringParser.Parse(content));
			
			if (parsedContent != null) {
				if (EditorControlService.GlobalOptions.IndentationString != "\t") {
					parsedContent = parsedContent.Replace("\t", EditorControlService.GlobalOptions.IndentationString);
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
				if (!String.IsNullOrEmpty(binaryFileName))
					File.Copy(binaryFileName, parsedFileName);
				else
					File.WriteAllText(parsedFileName, parsedContent, ParserService.DefaultFileEncoding);
				ParserService.ParseFile(parsedFileName, new StringTextBuffer(parsedContent));
			} else {
				if (!String.IsNullOrEmpty(binaryFileName)) {
					LoggingService.Warn("binary file was skipped");
					return;
				}
				IViewContent viewContent = FileService.NewFile(Path.GetFileName(parsedFileName), parsedContent);
				if (viewContent == null) {
					return;
				}
				if (Path.IsPathRooted(parsedFileName)) {
					Directory.CreateDirectory(Path.GetDirectoryName(parsedFileName));
					viewContent.PrimaryFile.SaveToDisk(parsedFileName);
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
			if (categoryTreeView.SelectedNode != null) {
				PropertyService.Set("Dialogs.NewProjectDialog.LargeImages", ((RadioButton)ControlDictionary["largeIconsRadioButton"]).Checked);
				PropertyService.Set("Dialogs.NewFileDialog.CategoryViewState", TreeViewHelper.GetViewStateString(categoryTreeView));
				PropertyService.Set("Dialogs.NewFileDialog.LastSelectedCategory", TreeViewHelper.GetPath(categoryTreeView.SelectedNode));
			}
			createdFiles.Clear();
			if (templateListView.SelectedItems.Count == 1) {
				if (!AllPropertiesHaveAValue) {
					MessageService.ShowMessage("${res:Dialog.NewFile.FillOutFirstMessage}", "${res:Dialog.NewFile.FillOutFirstCaption}");
					return;
				}
				TemplateItem item = (TemplateItem)templateListView.SelectedItems[0];
				
				PropertyService.Set("Dialogs.NewFileDialog.LastSelectedTemplate", item.Template.Name);
				
				string fileName;
				StringParserPropertyContainer.FileCreation["StandardNamespace"] = "DefaultNamespace";
				if (allowUntitledFiles) {
					fileName = GenerateCurrentFileName();
				} else {
					fileName = ControlDictionary["fileNameTextBox"].Text.Trim();
					if (!FileUtility.IsValidPath(fileName)
					    || fileName.IndexOf(Path.AltDirectorySeparatorChar) >= 0
					    || fileName.IndexOf(Path.DirectorySeparatorChar) >= 0)
					{
						MessageService.ShowError(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.SaveFile.InvalidFileNameError}", new StringTagPair("FileName", fileName)));
						return;
					}
					if (Path.GetExtension(fileName).Length == 0) {
						fileName += Path.GetExtension(item.Template.DefaultName);
					}
					fileName = Path.Combine(basePath, fileName);
					fileName = FileUtility.NormalizePath(fileName);
					IProject project = ProjectService.CurrentProject;
					if (project != null) {
						StringParserPropertyContainer.FileCreation["StandardNamespace"] = CustomToolsService.GetDefaultNamespace(project, fileName);
					}
				}
				
				FileTemplateOptions options = new FileTemplateOptions();
				options.ClassName = GenerateValidClassOrNamespaceName(Path.GetFileNameWithoutExtension(fileName), false);
				options.FileName = FileName.Create(fileName);
				options.IsUntitled = allowUntitledFiles;
				options.Namespace = StringParserPropertyContainer.FileCreation["StandardNamespace"];
				
				StringParserPropertyContainer.FileCreation["FullName"]                 = fileName;
				StringParserPropertyContainer.FileCreation["FileName"]                 = Path.GetFileName(fileName);
				StringParserPropertyContainer.FileCreation["FileNameWithoutExtension"] = Path.GetFileNameWithoutExtension(fileName);
				StringParserPropertyContainer.FileCreation["Extension"]                = Path.GetExtension(fileName);
				StringParserPropertyContainer.FileCreation["Path"]                     = Path.GetDirectoryName(fileName);
				
				StringParserPropertyContainer.FileCreation["ClassName"] = options.ClassName;
				
				// when adding a file to a project (but not when creating a standalone file while a project is open):
				if (ProjectService.CurrentProject != null && !this.allowUntitledFiles) {
					options.Project = ProjectService.CurrentProject;
					// add required assembly references to the project
					bool changes = false;
					foreach (ReferenceProjectItem reference in item.Template.RequiredAssemblyReferences) {
						IEnumerable<ProjectItem> refs = ProjectService.CurrentProject.GetItemsOfType(ItemType.Reference);
						if (!refs.Any(projItem => string.Equals(projItem.Include, reference.Include, StringComparison.OrdinalIgnoreCase))) {
							ReferenceProjectItem projItem = (ReferenceProjectItem)reference.CloneFor(ProjectService.CurrentProject);
							ProjectService.AddProjectItem(ProjectService.CurrentProject, projItem);
							changes = true;
						}
					}
					if (changes) {
						ProjectService.CurrentProject.Save();
					}
				}
				
				foreach (FileDescriptionTemplate newfile in item.Template.FileDescriptionTemplates) {
					if (!IsFilenameAvailable(StringParser.Parse(newfile.Name))) {
						MessageService.ShowError(string.Format("Filename {0} is in use.\nChoose another one", StringParser.Parse(newfile.Name))); // TODO : translate
						return;
					}
				}
				ScriptRunner scriptRunner = new ScriptRunner();
				foreach (FileDescriptionTemplate newFile in item.Template.FileDescriptionTemplates) {
					FileOperationResult result = FileUtility.ObservedSave(
						() => {
							if (!String.IsNullOrEmpty(newFile.BinaryFileName)) {
								SaveFile(newFile, null, newFile.BinaryFileName);
							} else {
								SaveFile(newFile, scriptRunner.CompileScript(item.Template, newFile), null);
							}
						}, StringParser.Parse(newFile.Name)
					);
					if (result != FileOperationResult.OK)
						return;
				}
				
				DialogResult = DialogResult.OK;
				
				// raise FileCreated event for the new files.
				foreach (KeyValuePair<string, FileDescriptionTemplate> entry in createdFiles) {
					FileService.FireFileCreated(entry.Key, false);
				}
				item.Template.RunActions(options);
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
			
			templateListView = ((ListView)ControlDictionary["templateListView"]);
			categoryTreeView = ((TreeView)ControlDictionary["categoryTreeView"]);
			
			categoryTreeView.ImageList = imglist;
			
			categoryTreeView.AfterSelect    += new TreeViewEventHandler(CategoryChange);
			categoryTreeView.BeforeSelect   += new TreeViewCancelEventHandler(OnBeforeExpand);
			categoryTreeView.BeforeExpand   += new TreeViewCancelEventHandler(OnBeforeExpand);
			categoryTreeView.BeforeCollapse += new TreeViewCancelEventHandler(OnBeforeCollapse);
			
			templateListView.SelectedIndexChanged += new EventHandler(SelectedIndexChange);
			templateListView.DoubleClick          += new EventHandler(OpenEvent);
			
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
			StartPosition = FormStartPosition.CenterParent;
			Icon          = null;
			
			CheckedChange(this, EventArgs.Empty);
		}
	}
}
