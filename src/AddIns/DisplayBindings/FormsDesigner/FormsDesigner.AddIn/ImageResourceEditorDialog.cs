// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Resources;
using System.Resources.Tools;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.FormsDesigner.Gui
{
	/// <summary>
	/// Allows the user to select a resource for an image or icon property.
	/// </summary>
	public sealed partial class ImageResourceEditorDialog : Form
	{
		readonly IProject project;
		readonly Type requiredResourceType;
		object originalImage;
		bool selectedImageIsProjectResource;
		object selectedImage;
		
		#region Constructors
		
		ImageResourceEditorDialog(IProject project, Type requiredResourceType, bool designerSupportsProjectResources)
			: base()
		{
			if (requiredResourceType == null)
				throw new ArgumentNullException("requiredResourceType");
			this.requiredResourceType = requiredResourceType;
			this.project = project;
			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			Translate(this);
			
			this.projectResourcesTreeView.Nodes.Add(ResourceService.GetString("Global.PleaseWait"));
			
			this.importLocalResourceButton.DataBindings.Add("Enabled", this.localResourceRadioButton, "Checked");
			this.projectResourcesTreeView.DataBindings.Add("Enabled", this.projectResourceRadioButton, "Checked");
			
			this.projectResourceRadioButton.Visible = designerSupportsProjectResources;
			this.projectResourcesTreeView.Visible = designerSupportsProjectResources;
		}
		
		public ImageResourceEditorDialog(IProject project, Type requiredResourceType, ProjectResourceInfo projectResource)
			: this(project, requiredResourceType, true)
		{
			if (projectResource == null)
				throw new ArgumentNullException("projectResource");
			
			this.projectResourceRadioButton.Checked = true;
			this.originalImage = this.selectedImage = projectResource.OriginalValue;
			
			Image image = this.selectedImage as Image;
			if (image != null) {
				this.selectedImageIsProjectResource = true;
				this.SetPreviewImage(image);
			} else {
				Icon icon = this.selectedImage as Icon;
				if (icon != null) {
					this.selectedImageIsProjectResource = true;
					this.SetPreviewImage(icon.ToBitmap());
				}
			}
			this.projectTreeScanningBackgroundWorker.RunWorkerAsync(projectResource);
		}
		
		public ImageResourceEditorDialog(IProject project, Image localResource, bool designerSupportsProjectResources)
			: this(project, typeof(Image), designerSupportsProjectResources)
		{
			if (localResource != null) {
				this.localResourceRadioButton.Checked = true;
				this.originalImage = this.selectedImage = localResource;
				this.SetPreviewImage(localResource);
			} else {
				this.noResourceRadioButton.Checked = true;
			}
			this.projectTreeScanningBackgroundWorker.RunWorkerAsync();
		}
		
		public ImageResourceEditorDialog(IProject project, Icon localResource, bool designerSupportsProjectResources)
			: this(project, typeof(Icon), designerSupportsProjectResources)
		{
			if (localResource != null) {
				this.localResourceRadioButton.Checked = true;
				this.originalImage = this.selectedImage = localResource;
				this.SetPreviewImage(localResource.ToBitmap());
			} else {
				this.noResourceRadioButton.Checked = true;
			}
			this.projectTreeScanningBackgroundWorker.RunWorkerAsync();
		}
		
		static void Translate(Control c)
		{
			c.Text = StringParser.Parse(c.Text);
			foreach (Control child in c.Controls) {
				Translate(child);
			}
		}
		
		#endregion
		
		#region Properties
		
		/// <summary>
		/// Gets the <see cref="ProjectResourceInfo"/> for the selected project resource,
		/// or <c>null</c> if the selected resource is not a project resource.
		/// </summary>
		public ProjectResourceInfo SelectedProjectResource {
			get {
				if (this.selectedImageIsProjectResource && this.projectResourceRadioButton.Checked) {
					
					TreeNode node = this.projectResourcesTreeView.SelectedNode;
					if (node == null) return null;
					
					return new ProjectResourceInfo(((FileProjectItem)node.Parent.Tag).FileName, node.Text);
					
				} else {
					return null;
				}
			}
		}
		
		/// <summary>
		/// Gets the selected image.
		/// This can be an Image or an Icon (matching the type that was passed to the constructor) or null.
		/// </summary>
		public object SelectedResourceValue {
			get {
				return this.selectedImage;
			}
		}
		
		#endregion
		
		void SetPreviewImage(Image image)
		{
			this.previewPictureBox.Image = image;
			if (image != null) {
				if (image.Width > this.previewPictureBox.ClientSize.Width ||
				    image.Height > this.previewPictureBox.ClientSize.Height) {
					this.previewPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
				} else {
					this.previewPictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
				}
			}
		}
		
		void DisposeImageIfNotOriginal(object image)
		{
			if (!Object.ReferenceEquals(image, this.originalImage)) {
				IDisposable d = image as IDisposable;
				if (d != null) {
					d.Dispose();
				}
			}
		}
		
		void SetSelectedImage(object image, bool isProjectResource)
		{
			if (!Object.ReferenceEquals(this.selectedImage, this.previewPictureBox.Image)) {
				Image temp = this.previewPictureBox.Image;
				this.previewPictureBox.Image = null;
				this.DisposeImageIfNotOriginal(temp);
			} else {
				this.previewPictureBox.Image = null;
			}
			
			if (!this.selectedImageIsProjectResource) {
				this.DisposeImageIfNotOriginal(this.selectedImage);
			}
			
			Image img = image as Image;
			if (img != null) {
				this.selectedImage = img;
				this.selectedImageIsProjectResource = isProjectResource;
				this.SetPreviewImage(img);
			} else {
				Icon icon = image as Icon;
				if (icon != null) {
					this.selectedImage = icon;
					this.selectedImageIsProjectResource = isProjectResource;
					this.SetPreviewImage(icon.ToBitmap());
				} else {
					this.selectedImageIsProjectResource = false;
					this.selectedImage = null;
				}
			}
		}
		
		#region Project tree filling
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		void ProjectTreeScanningBackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
		{
			if (this.project == null) {
				return;
			}
			
			ProjectResourceInfo selectedProjectResource = e.Argument as ProjectResourceInfo;
			
			IProjectContent projectContent = ParserService.GetProjectContent(this.project);
			
			TreeNode root = new TreeNode(this.project.Name, 0, 0);
			TreeNode preSelection = null;
			TreeNode lastFileNode = null;
			int fileNodesCount = 0;
			
			foreach (FileProjectItem item in this.project.GetItemsOfType(ItemType.EmbeddedResource).OfType<FileProjectItem>().OrderBy(fpi => Path.GetFileName(fpi.VirtualName))) {
				
				if (this.projectTreeScanningBackgroundWorker.CancellationPending) {
					e.Cancel = true;
					break;
				}
				
				// Skip files where the generated class name
				// would conflict with an existing class.
				string namespaceName = item.GetEvaluatedMetadata("CustomToolNamespace");
				if (string.IsNullOrEmpty(namespaceName)) {
					namespaceName = CustomToolsService.GetDefaultNamespace(item.Project, item.FileName);
				}
				IClass existingClass = projectContent.GetClass(namespaceName + "." + StronglyTypedResourceBuilder.VerifyResourceName(Path.GetFileNameWithoutExtension(item.FileName), projectContent.Language.CodeDomProvider), 0);
				if (existingClass != null) {
					if (!ProjectResourceService.IsGeneratedResourceClass(existingClass)) {
						continue;
					}
				}
				
				bool selectedFile = (selectedProjectResource != null) && FileUtility.IsEqualFileName(selectedProjectResource.ResourceFile, item.FileName);
				TreeNode file = null;
				
				try {
					
					foreach (KeyValuePair<string, object> r in this.GetResources(item.FileName).OrderBy(pair => pair.Key)) {
						if (this.projectTreeScanningBackgroundWorker.CancellationPending) {
							e.Cancel = true;
							break;
						}
						
						if (file == null) {
							file = CreateAndAddFileNode(root, item);
						}
						
						TreeNode resNode = new TreeNode(r.Key, 3, 3);
						resNode.Tag = r.Value;
						file.Nodes.Add(resNode);
						
						if (selectedFile) {
							if (String.Equals(r.Key, selectedProjectResource.ResourceKey, StringComparison.Ordinal)) {
								preSelection = resNode;
							}
						}
					}
					
					if (file != null) {
						lastFileNode = file;
						++fileNodesCount;
					}
					
				} catch (Exception ex) {
					if (file == null) {
						file = CreateAndAddFileNode(root, item);
					}
					TreeNode error = new TreeNode(ex.Message, 4, 4);
					file.Nodes.Add(error);
				}
			}
			
			if (e.Cancel) {
				DisposeNodeImages(root);
			} else {
				// Preselect the file node if there is only one
				if (preSelection == null && fileNodesCount == 1) {
					preSelection = lastFileNode;
				}
				e.Result = new TreeScanResult(root, preSelection);
			}
		}
		
		sealed class TreeScanResult {
			readonly TreeNode root;
			readonly TreeNode preSelection;
			
			public TreeNode Root {
				get { return root; }
			}
			
			public TreeNode PreSelection {
				get { return preSelection; }
			}
			
			public TreeScanResult(TreeNode root, TreeNode preSelection)
			{
				this.root = root;
				this.preSelection = preSelection;
			}
		}
		
		static TreeNode CreateAndAddFileNode(TreeNode root, FileProjectItem item)
		{
			string directory = Path.GetDirectoryName(item.VirtualName);
			TreeNode dir;
			
			if (String.IsNullOrEmpty(directory)) {
				dir = root;
			} else {
				dir = GetOrCreateDirectoryNode(root, directory);
			}
			
			TreeNode file = new TreeNode(Path.GetFileName(item.VirtualName), 2, 2);
			file.Tag = item;
			dir.Nodes.Add(file);
			return file;
		}
		
		static TreeNode GetOrCreateDirectoryNode(TreeNode root, string directory)
		{
			int index = directory.IndexOfAny(new [] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar});
			string searchDir;
			
			if (index == -1) {
				searchDir = directory;
			} else {
				searchDir = directory.Substring(0, index);
			}
			
			TreeNode node = null;
			foreach (TreeNode n in root.Nodes) {
				if (n.Text == searchDir) {
					node = n;
					break;
				}
			}
			
			if (node == null) {
				node = new TreeNode(searchDir, 1, 1);
				int insertIndex;
				for (insertIndex = 0; insertIndex < root.Nodes.Count; insertIndex++) {
					TreeNode n = root.Nodes[insertIndex];
					if (n.ImageIndex != 1 || StringComparer.CurrentCulture.Compare(searchDir, n.Text) < 0) {
						break;
					}
				}
				root.Nodes.Insert(insertIndex, node);
			}
			
			if (index == -1) {
				return node;
			} else {
				return GetOrCreateDirectoryNode(node, directory.Substring(index + 1));
			}
		}
		
		Dictionary<string, object> GetResources(string fileName)
		{
			Stream s = null;
			WorkbenchSingleton.SafeThreadCall(
				delegate {
					OpenedFile file = FileService.GetOpenedFile(fileName);
					if (file != null) {
						s = file.OpenRead();
					}
				});
			if (s == null) {
				s = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			using(s) {
				using(IResourceReader reader = ResourceStore.CreateResourceReader(s, ResourceStore.GetResourceType(fileName))) {
					ResXResourceReader resXReader = reader as ResXResourceReader;
					if (resXReader != null) {
						resXReader.BasePath = Path.GetDirectoryName(fileName);
					}
					
					var resources = new Dictionary<string, object>();
					foreach (System.Collections.DictionaryEntry entry in reader) {
						if (entry.Value == null) continue;
						if (this.requiredResourceType.IsAssignableFrom(entry.Value.GetType())) {
							resources.Add((string)entry.Key, entry.Value);
						}
					}
					return resources;
				}
			}
		}
		
		void ProjectTreeScanningBackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (this.IsDisposed || this.projectResourcesTreeView.IsDisposed) {
				// This can happen when the dialog is closed before
				// the scan has finished
				if (!e.Cancelled && e.Error == null) {
					TreeScanResult r = e.Result as TreeScanResult;
					if (r != null) {
						DisposeNodeImages(r.Root);
					}
				}
				return;
			}
			
			this.projectResourcesTreeView.Nodes.Clear();
			
			if (e.Cancelled) {
				return;
			}
			
			if (e.Error != null) {
				MessageService.ShowException(e.Error, "Error in project tree scanning thread");
			}
			
			TreeScanResult result = e.Result as TreeScanResult;
			if (result == null) {
				return;
			}
			
			this.projectResourcesTreeView.BeginUpdate();
			
			ImageList imageList = new ImageList();
			imageList.ColorDepth = ColorDepth.Depth32Bit;
			imageList.Images.Add(IconService.GetBitmap(IconService.GetImageForProjectType(this.project.Language)));
			imageList.Images.Add(WinFormsResourceService.GetBitmap("ProjectBrowser.Folder.Closed"));
			imageList.Images.Add(IconService.GetBitmap(IconService.GetImageForFile("a.resx")));
			imageList.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Field"));
			imageList.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Error"));
			this.projectResourcesTreeView.ImageList = imageList;
			
			this.projectResourcesTreeView.Nodes.Add(result.Root);
			
			if (result.PreSelection != null) {
				result.PreSelection.EnsureVisible();
				this.projectResourcesTreeView.SelectedNode = result.PreSelection;
				result.PreSelection.Expand();
			} else {
				result.Root.Expand();
			}
			
			this.projectResourcesTreeView.EndUpdate();
			
			if (result.PreSelection != null) {
				this.projectResourcesTreeView.Focus();
			}
		}
		
		#endregion
		
		void NoResourceRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			if (this.noResourceRadioButton.Checked) {
				this.SetSelectedImage(null, false);
				this.okButton.Enabled = true;
			}
		}
		
		void LocalResourceRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			if (this.localResourceRadioButton.Checked) {
				this.okButton.Enabled = true;
			}
		}
		
		void ProjectResourceRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			if (this.projectResourceRadioButton.Checked) {
				this.UpdateOnProjectResourceSelection();
				this.projectResourcesTreeView.Focus();
			}
		}
		
		void ProjectResourcesTreeViewAfterSelect(object sender, TreeViewEventArgs e)
		{
			if (this.projectResourceRadioButton.Checked) {
				this.UpdateOnProjectResourceSelection();
			}
		}
		
		void UpdateOnProjectResourceSelection()
		{
			TreeNode node = this.projectResourcesTreeView.SelectedNode;
			if (node != null && node.Tag != null && this.requiredResourceType.IsAssignableFrom(node.Tag.GetType())) {
				this.SetSelectedImage(node.Tag, true);
				this.okButton.Enabled = true;
			} else {
				this.SetSelectedImage(null, false);
				this.okButton.Enabled = false;
			}
		}
		
		void ImageResourceEditorDialogFormClosed(object sender, FormClosedEventArgs e)
		{
			this.projectTreeScanningBackgroundWorker.CancelAsync();
			if (this.projectResourcesTreeView.Nodes.Count > 0) {
				DisposeNodeImages(this.projectResourcesTreeView.Nodes[0]);
			}
		}
		
		static void DisposeNodeImages(TreeNode root)
		{
			if (root.Nodes.Count == 0) {
				IDisposable d = root.Tag as IDisposable;
				if (d != null) {
					d.Dispose();
				}
				root.Tag = null;
			} else {
				foreach (TreeNode node in root.Nodes) {
					DisposeNodeImages(node);
				}
			}
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		void ImportLocalResourceButtonClick(object sender, EventArgs e)
		{
			bool isIcon = typeof(Icon).IsAssignableFrom(this.requiredResourceType);
			using(OpenFileDialog dialog = new OpenFileDialog()) {
				dialog.Filter = (isIcon ? DummyIconEditor.FileFilterEntry : DummyImageEditor.FileFilterEntry);
				dialog.RestoreDirectory = true;
				dialog.Title = StringParser.Parse("${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ImageResourceEditor.Title}");
				if (dialog.ShowDialog(this) == DialogResult.OK && !String.IsNullOrEmpty(dialog.FileName)) {
					try {
						
						if (isIcon) {
							this.SetSelectedImage(new Icon(dialog.FileName), false);
						} else {
							this.SetSelectedImage(Image.FromFile(dialog.FileName), false);
						}
						
					} catch (Exception ex) {
						MessageService.ShowError(ex.Message);
					}
				}
			}
		}
		
		#region Dummy editors for getting the file filter from the framework
		
		sealed class DummyImageEditor : ImageEditor
		{
			DummyImageEditor()
			{
			}
			
			internal static string FileFilterEntry {
				get { return CreateFilterEntry(new ImageEditor()); }
			}
		}
		
		sealed class DummyIconEditor : IconEditor
		{
			DummyIconEditor()
			{
			}
			
			internal static string FileFilterEntry {
				get { return CreateFilterEntry(new IconEditor()); }
			}
		}
		
		#endregion
	}
}
