// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Resources;
using System.Resources.Tools;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ICSharpCode.FormsDesigner.Services;

//using ICSharpCode.Core;
//using ICSharpCode.Core.WinForms;
//using ICSharpCode.SharpDevelop;
//using ICSharpCode.SharpDevelop.Dom;
//using ICSharpCode.SharpDevelop.Editor;
//using ICSharpCode.SharpDevelop.Gui;
//using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.FormsDesigner.Gui
{
	/// <summary>
	/// Allows the user to select a resource for an image or icon property.
	/// </summary>
	public sealed partial class ImageResourceEditorDialog : Form
	{
		readonly Type requiredResourceType;
		object originalImage;
		bool selectedImageIsProjectResource;
		object selectedImage;
		IImageResourceEditorDialogWrapper dialogProxy;
		CancellationTokenSource cts;
		IServiceProvider provider;
		ISharpDevelopIDEService messageService;
		IResourceStore resourceStore;
		
		#region Constructors
		
		ImageResourceEditorDialog(IServiceProvider provider, IImageResourceEditorDialogWrapper dialogProxy, Type requiredResourceType, bool designerSupportsProjectResources)
			: base()
		{
			if (requiredResourceType == null)
				throw new ArgumentNullException("requiredResourceType");
			this.requiredResourceType = requiredResourceType;
			this.dialogProxy = dialogProxy;
			InitializeComponent();
			
			cts = new CancellationTokenSource();
			this.provider = provider;
			this.messageService = (ISharpDevelopIDEService)provider.GetService(typeof(ISharpDevelopIDEService));
			this.resourceStore = (IResourceStore)provider.GetService(typeof(IResourceStore));
			
			Translate(this);
			this.projectResourcesTreeView.Nodes.Add(messageService.GetResourceString("Global.PleaseWait"));
			
			this.importLocalResourceButton.DataBindings.Add("Enabled", this.localResourceRadioButton, "Checked");
			this.projectResourcesTreeView.DataBindings.Add("Enabled", this.projectResourceRadioButton, "Checked");
			
			this.projectResourceRadioButton.Visible = designerSupportsProjectResources;
			this.projectResourcesTreeView.Visible = designerSupportsProjectResources;
		}
		
		public ImageResourceEditorDialog(IServiceProvider provider, IImageResourceEditorDialogWrapper dialogProxy, Type requiredResourceType, IProjectResourceInfo projectResource)
			: this(provider, dialogProxy, requiredResourceType, true)
		{
			if (projectResource == null)
				throw new ArgumentNullException("projectResource");
			
			this.projectResourceRadioButton.Checked = true;
			this.originalImage = this.selectedImage = projectResource.OriginalValue;
			
			this.selectedImageIsProjectResource = true;
			if (selectedImage is Stream)
				this.SetPreviewImage(new Bitmap(selectedImage as Stream));
			else if (selectedImage is Icon)
				this.SetPreviewImage((selectedImage as Icon).ToBitmap());
			dialogProxy.ProduceTreeNodesAsync(projectResource, requiredResourceType, ProduceNodesFinished);
		}
		
		public ImageResourceEditorDialog(IServiceProvider provider, IImageResourceEditorDialogWrapper dialogProxy, object localResource, bool isIcon, bool designerSupportsProjectResources)
			: this(provider, dialogProxy, isIcon ? typeof(Icon) : typeof(Image), designerSupportsProjectResources)
		{
			if (selectedImage is Stream) {
				this.localResourceRadioButton.Checked = true;
				this.originalImage = this.selectedImage = localResource;
				this.SetPreviewImage(new Bitmap(localResource as Stream));
			} else if (selectedImage is Icon) {
				this.localResourceRadioButton.Checked = true;
				this.originalImage = this.selectedImage = localResource;
				this.SetPreviewImage((localResource as Icon).ToBitmap());
			} else {
				this.noResourceRadioButton.Checked = true;
			}
			dialogProxy.ProduceTreeNodesAsync(null, requiredResourceType, ProduceNodesFinished);
		}
		
		void Translate(Control control)
		{
			control.Text = messageService.Parse(control.Text);
			foreach (Control child in control.Controls) {
				Translate(child);
			}
		}
		
		object CreateStream(object value)
		{
			MemoryStream stream = new MemoryStream();
			
			if (value is Image)
				((Image)value).Save(stream, ImageFormat.Png);
			else
				return value;
			
			return stream;
		}
		#endregion
		
		#region Properties
		
		/// <summary>
		/// Gets the <see cref="ProjectResourceInfo"/> for the selected project resource,
		/// or <c>null</c> if the selected resource is not a project resource.
		/// </summary>
		public IProjectResourceInfo SelectedProjectResource {
			get {
				if (this.selectedImageIsProjectResource && this.projectResourceRadioButton.Checked) {
					
					TreeNode node = this.projectResourcesTreeView.SelectedNode;
					if (node == null) return null;
					
					return ProjectResourceInfo.Create(resourceStore, (string)node.Parent.Tag, node.Text);
					
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
		
		void ProduceNodesFinished(bool isCancelled, Exception exception, TreeScanResult result)
		{
			projectResourcesTreeView.Invoke(
				(Action)delegate {
					if (this.IsDisposed || this.projectResourcesTreeView.IsDisposed) {
						// This can happen when the dialog is closed before
						// the scan has finished
						return;
					}

					this.projectResourcesTreeView.Nodes.Clear();
					
					if (isCancelled) {
						return;
					}
					
					if (exception != null) {
						messageService.ShowException(exception, "Error in project tree scanning thread");
					}
					
					if (result == null) {
						return;
					}
					
					this.projectResourcesTreeView.BeginUpdate();
					
					this.projectResourcesTreeView.ImageList = dialogProxy.CreateImageList();
					TreeNode selection = null;
					GenerateTreeNodes(result, this.projectResourcesTreeView.Nodes, ref selection);
					
					if (selection != null) {
						selection.EnsureVisible();
						this.projectResourcesTreeView.SelectedNode = selection;
						selection.Expand();
					} else {
						this.projectResourcesTreeView.Nodes[0].Expand();
					}
					
					this.projectResourcesTreeView.EndUpdate();
					
					if (selection != null) {
						this.projectResourcesTreeView.Focus();
					}
				}
			);
		}
		
		void GenerateTreeNodes(TreeScanResult parent, TreeNodeCollection nodes, ref TreeNode selection)
		{
			var treeNode = new TreeNode(parent.Text, parent.ImageIndex, parent.SelectedImageIndex);
			treeNode.Tag = parent.FileName;
			
			foreach (var res in GetResources(parent.FileName)) {
				var resNode = new TreeNode(res.Key, 3, 3);
				resNode.Tag = res.Value;
				treeNode.Nodes.Add(resNode);
			}
			
			if (parent.IsSelected)
				selection = treeNode;
			nodes.Add(treeNode);
			foreach (var node in parent.Children) {
				GenerateTreeNodes(node, treeNode.Nodes, ref selection);
			}
		}
		
		IEnumerable<KeyValuePair<string, object>> GetResources(string fileName)
		{
			if (fileName == null)
				yield break;
			
			Stream s = resourceStore.OpenFile(fileName);
			using(s) {
				using(IResourceReader reader = ResourceHelpers.CreateResourceReader(s, ResourceHelpers.GetResourceType(fileName))) {
					ResXResourceReader resXReader = reader as ResXResourceReader;
					if (resXReader != null) {
						resXReader.BasePath = Path.GetDirectoryName(fileName);
					}
					
					foreach (System.Collections.DictionaryEntry entry in reader) {
						if (entry.Value == null) continue;
						if (this.requiredResourceType.IsAssignableFrom(entry.Value.GetType())) {
							yield return new KeyValuePair<string, object>((string)entry.Key, entry.Value);
						}
					}
				}
			}
		}
		
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
		
		void SetSelectedImage(object image, bool isIcon, bool isProjectResource)
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
			
			if (image == null) {
				this.selectedImageIsProjectResource = false;
				this.selectedImage = null;
				return;
			}
			
			if (isIcon) {
				this.selectedImage = image;
				this.selectedImageIsProjectResource = isProjectResource;
				this.SetPreviewImage((image as Icon).ToBitmap());
			} else {
				this.selectedImage = image;
				this.selectedImageIsProjectResource = isProjectResource;
				this.SetPreviewImage(image as Image);
			}
		}
		
		void NoResourceRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			if (this.noResourceRadioButton.Checked) {
				this.SetSelectedImage(null, false, false);
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
				this.SetSelectedImage(node.Tag, false, true);
				this.okButton.Enabled = true;
			} else {
				this.SetSelectedImage(null, false, false);
				this.okButton.Enabled = false;
			}
		}
		
		void ImageResourceEditorDialogFormClosed(object sender, FormClosedEventArgs e)
		{
			cts.Cancel();
		}
		
		void ImportLocalResourceButtonClick(object sender, EventArgs e)
		{
			bool isIcon = typeof(Icon).IsAssignableFrom(this.requiredResourceType);
			using(OpenFileDialog dialog = new OpenFileDialog()) {
				dialog.Filter = (isIcon ? DummyIconEditor.FileFilterEntry : DummyImageEditor.FileFilterEntry);
				dialog.RestoreDirectory = true;
				dialog.Title = messageService.Parse("${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.ImageResourceEditor.Title}");
				if (dialog.ShowDialog(this) == DialogResult.OK && !String.IsNullOrEmpty(dialog.FileName)) {
					try {
						object data = null;
						if (isIcon)
							data = new Icon(dialog.FileName);
						else
							data = new Bitmap(dialog.FileName);
						this.SetSelectedImage(data, isIcon, false);
					} catch (Exception ex) {
						messageService.ShowException(ex, null);
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
