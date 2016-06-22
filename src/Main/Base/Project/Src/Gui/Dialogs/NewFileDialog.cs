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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Templates;
using Microsoft.Build.Framework.XamlTypes;

namespace ICSharpCode.SharpDevelop.Gui
{
	// TODO: rewrite without XMLForms
	#pragma warning disable 618

	/// <summary>
	///  This class is for creating a new "empty" file
	/// </summary>
	internal class NewFileDialog : BaseSharpDevelopForm
	{
		List<TemplateItem> alltemplates = new List<TemplateItem>();
		Dictionary<IImage, int> icons = new Dictionary<IImage, int>();
		bool allowUntitledFiles;
		IProject project;
		DirectoryName basePath;
		internal FileTemplateOptions options;
		internal FileTemplateResult result;
		
		public NewFileDialog(IProject project, DirectoryName basePath, IEnumerable<TemplateCategory> templateCategories)
		{
			this.project = project;
			this.basePath = basePath;
			this.allowUntitledFiles = basePath == null;
			try {
				InitializeComponents();
				InitializeTemplates(templateCategories);
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
			
			foreach (var image in icons.Keys.ToArray()) {
				Bitmap bitmap = image.Bitmap;
				if (bitmap != null) {
					smalllist.Images.Add(bitmap);
					imglist.Images.Add(bitmap);
					icons[image] = ++i;
				} else {
					LoggingService.Warn("NewFileDialog: can't load bitmap " + image.ToString() + " using default");
				}
			}
			
			foreach (TemplateItem item in alltemplates) {
				if (item.Template.Icon == null) {
					item.ImageIndex = 0;
				} else {
					item.ImageIndex = (int)icons[item.Template.Icon];
				}
			}
			
			templateListView.LargeImageList = imglist;
			templateListView.SmallImageList = smalllist;
			
			TreeViewHelper.ApplyViewStateString(PropertyService.Get("Dialogs.NewFileDialog.CategoryViewState", ""), categoryTreeView);
			categoryTreeView.SelectedNode = TreeViewHelper.GetNodeByPath(categoryTreeView, PropertyService.Get("Dialogs.NewFileDialog.LastSelectedCategory", "C#"));
		}
		
		ListView templateListView;
		System.Windows.Forms.TreeView categoryTreeView;
		
		void InitializeTemplates(IEnumerable<TemplateCategory> templateCategories)
		{
			foreach (var templateCategory in Sorted(templateCategories)) {
				var cat = CreateCategory(templateCategory);
				if (!cat.IsEmpty)
					categoryTreeView.Nodes.Add(cat);
			}
		}
		
		IEnumerable<TemplateCategory> Sorted(IEnumerable<TemplateCategory> templateCategories)
		{
			return templateCategories.OrderByDescending(c => c.SortOrder).ThenBy(c => StringParser.Parse(c.DisplayName));
		}
		
		Category CreateCategory(TemplateCategory templateCategory)
		{
			Category node = new NewFileDialog.Category(templateCategory.DisplayName);
			foreach (var subcategory in Sorted(templateCategory.Subcategories)) {
				var subnode = CreateCategory(subcategory);
				if (!subnode.IsEmpty)
					node.Nodes.Add(subnode);
			}
			foreach (var template in templateCategory.Templates.OfType<FileTemplate>()) {
				if (!template.IsVisible(project))
					continue;
				TemplateItem titem = new TemplateItem(template);
				if (titem.Template.Icon != null) {
					icons[titem.Template.Icon] = 0; // "create template icon"
				}
				alltemplates.Add(titem);
				node.Templates.Add(titem);
			}
			return node;
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
		object localizedTypeDescriptor = null;
		
		void ShowPropertyGrid()
		{
			if (!Controls.Contains(propertyGrid)) {
				this.SuspendLayout();
				propertyGrid.Location = new Point(Width - GridMargin, GridMargin);
				propertyGrid.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
				
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
			return SelectedTemplate.SuggestFileName(basePath);
		}
		
		bool isNameModified = false;
		
		// list view event handlers
		void SelectedIndexChange(object sender, EventArgs e)
		{
			if (templateListView.SelectedItems.Count == 1) {
				ControlDictionary["descriptionLabel"].Text = StringParser.Parse(SelectedTemplate.Description);
				ControlDictionary["openButton"].Enabled = true;
				localizedTypeDescriptor = SelectedTemplate.CreateCustomizationObject();
				if (localizedTypeDescriptor != null) {
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
			if (templateListView.SelectedItems.Count == 1) {
				TemplateItem item = (TemplateItem)templateListView.SelectedItems[0];
				
				PropertyService.Set("Dialogs.NewFileDialog.LastSelectedTemplate", item.Template.Name);
				
				string fileName;
				string standardNamespace = "DefaultNamespace";
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
						fileName += Path.GetExtension(item.Template.SuggestFileName(null));
					}
					fileName = Path.Combine(basePath, fileName);
					fileName = FileUtility.NormalizePath(fileName);
					if (project != null) {
						standardNamespace = CustomToolsService.GetDefaultNamespace(project, fileName);
					}
				}
				
				options = new FileTemplateOptions();
				options.ClassName = GenerateValidClassOrNamespaceName(Path.GetFileNameWithoutExtension(fileName), false);
				options.FileName = FileName.Create(fileName);
				options.IsUntitled = allowUntitledFiles;
				options.Namespace = standardNamespace;
				options.CustomizationObject = localizedTypeDescriptor;
				options.Project = project;
				
				result = SelectedTemplate.Create(options);
				DialogResult = DialogResult.OK;
				if (result != null)
					SelectedTemplate.RunActions(result);
			}
		}
		
		/// <summary>
		///  Represents a category
		/// </summary>
		class Category : TreeNode
		{
			List<TemplateItem> templates  = new List<TemplateItem>();
			public bool Selected = false;
			public bool HasSelectedTemplate = false;
			
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
				get {
					return templates.Count == 0 && Nodes.Count == 0;
				}
			}
		}
		
		/// <summary>
		///  Represents a new file template
		/// </summary>
		class TemplateItem : ListViewItem
		{
			FileTemplate template;
			
			public TemplateItem(FileTemplate template) : base(template.DisplayName)
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
				SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.SharpDevelop.Resources.NewFileDialog.xfrm"));
			} else {
				SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.SharpDevelop.Resources.NewFileWithNameDialog.xfrm"));
				ControlDictionary["fileNameTextBox"].TextChanged += new EventHandler(FileNameChanged);
			}
			
			ImageList imglist = new ImageList();
			imglist.ColorDepth = ColorDepth.Depth32Bit;
			imglist.Images.Add(IconService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			imglist.Images.Add(IconService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
			
			templateListView = ((ListView)ControlDictionary["templateListView"]);
			categoryTreeView = ((System.Windows.Forms.TreeView)ControlDictionary["categoryTreeView"]);
			
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
