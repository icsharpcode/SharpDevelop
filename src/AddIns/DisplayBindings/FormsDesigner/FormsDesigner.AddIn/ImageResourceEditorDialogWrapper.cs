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
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.FormsDesigner.Gui
{
	public class ImageResourceEditorDialogWrapper : MarshalByRefObject, IImageResourceEditorDialogWrapper
	{
		IProject project;
		Type requiredResourceType;
		
		public ImageResourceEditorDialogWrapper(IProject project)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			this.project = project;
		}
		
		TreeScanResult ProduceTreeNodes(IProjectResourceInfo selectedProjectResource, CancellationToken ct)
		{
			IProjectContent projectContent = ParserService.GetProjectContent(this.project);
			
			TreeScanResult root = new TreeScanResult(project.Name, 0, 0);
			TreeScanResult preSelection = null;
			TreeScanResult lastFileNode = null;
			int fileNodesCount = 0;
			foreach (FileProjectItem item in this.project.GetItemsOfType(ItemType.EmbeddedResource)
			         .OfType<FileProjectItem>().OrderBy(fpi => Path.GetFileName(fpi.VirtualName))) {
				ct.ThrowIfCancellationRequested();
				
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
				TreeScanResult file = CreateAndAddFileNode(root, item);
				
				if (file != null) {
					lastFileNode = file;
					++fileNodesCount;
				}
			}
			
			// Preselect the file node if there is only one
			if (preSelection == null && fileNodesCount == 1) {
				preSelection = lastFileNode;
				lastFileNode.IsSelected = true;
			}
			return root;
		}
		
		static TreeScanResult CreateAndAddFileNode(TreeScanResult root, FileProjectItem item)
		{
			string directory = Path.GetDirectoryName(item.VirtualName);
			TreeScanResult dir;
			
			if (String.IsNullOrEmpty(directory)) {
				dir = root;
			} else {
				dir = GetOrCreateDirectoryNode(root, directory);
			}
			
			TreeScanResult file = new TreeScanResult(Path.GetFileName(item.VirtualName), 2, 2);
			dir.Children.Add(file);
			file.FileName = item.FileName;
			return file;
		}
		
		static TreeScanResult GetOrCreateDirectoryNode(TreeScanResult root, string directory)
		{
			int index = directory.IndexOfAny(new [] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar});
			string searchDir;
			
			if (index == -1) {
				searchDir = directory;
			} else {
				searchDir = directory.Substring(0, index);
			}
			
			TreeScanResult node = null;
			foreach (TreeScanResult n in root.Children) {
				if (n.Text == searchDir) {
					node = n;
					break;
				}
			}
			
			if (node == null) {
				node = new TreeScanResult(searchDir, 1, 1);
				int insertIndex;
				for (insertIndex = 0; insertIndex < root.Children.Count; insertIndex++) {
					TreeScanResult n = root.Children[insertIndex];
					if (n.ImageIndex != 1 || StringComparer.CurrentCulture.Compare(searchDir, n.Text) < 0) {
						break;
					}
				}
				root.Children.Insert(insertIndex, node);
			}
			
			if (index == -1) {
				return node;
			} else {
				return GetOrCreateDirectoryNode(node, directory.Substring(index + 1));
			}
		}
		
		public ImageList CreateImageList()
		{
			ImageList imageList = new ImageList();
			imageList.ColorDepth = ColorDepth.Depth32Bit;
			imageList.Images.Add(IconService.GetBitmap(IconService.GetImageForProjectType(this.project.Language)));
			imageList.Images.Add(WinFormsResourceService.GetBitmap("ProjectBrowser.Folder.Closed"));
			imageList.Images.Add(IconService.GetBitmap(IconService.GetImageForFile("a.resx")));
			imageList.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Field"));
			imageList.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Error"));
			return imageList;
		}
		
		public void ProduceTreeNodesAsync(IProjectResourceInfo projectResource, Type requiredResourceType, Action<bool, Exception, TreeScanResult> finishedAction)
		{
			var ct = new CancellationTokenSource().Token;
			this.requiredResourceType = requiredResourceType;
			var task = new System.Threading.Tasks.Task<TreeScanResult>(() => ProduceTreeNodes(projectResource, ct));
			task.ContinueWith(t => finishedAction(t.IsCanceled, t.Exception, t.Result))
				.ContinueWith(t => WorkbenchSingleton.SafeThreadAsyncCall((Action)delegate { if (t.Exception != null) MessageService.ShowException(t.Exception); }));
			task.Start();
		}
		
		public void UpdateProjectResource(IProjectResourceInfo projectResource)
		{
			// Ensure the resource generator is turned on for the selected resource file.
			if (project != null) {
				FileProjectItem fpi = project.FindFile(projectResource.ResourceFile);
				if (fpi == null) {
					throw new InvalidOperationException("The selected resource file '" + projectResource.ResourceFile + "' was not found in the project.");
				}
				const string resourceGeneratorToolName = "ResXFileCodeGenerator";
				const string publicResourceGeneratorToolName = "PublicResXFileCodeGenerator";
				if (!String.Equals(resourceGeneratorToolName, fpi.CustomTool, StringComparison.Ordinal) &&
				    !String.Equals(publicResourceGeneratorToolName, fpi.CustomTool, StringComparison.Ordinal)) {
					fpi.CustomTool = resourceGeneratorToolName;
				}
				CustomToolsService.RunCustomTool(fpi, true);
			}
		}
	}
}
