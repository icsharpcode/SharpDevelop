// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ICSharpCode.FormsDesigner.Services
{

	
	public interface ITypeLocator
	{
		AssemblyInfo LocateType(string name, out AssemblyInfo[] referencedAssemblies);
	}
	
	public interface IGacWrapper
	{
		bool IsGacAssembly(string path);
	}
	
	public interface IImageResourceEditorDialogWrapper
	{
		void ProduceTreeNodesAsync(IProjectResourceInfo projectResource, Type requiredResourceType, Action<bool, Exception, TreeScanResult> finishedAction);
		ImageList CreateImageList();
		void UpdateProjectResource(IProjectResourceInfo projectResource);
	}
	
	public sealed class TreeScanResult : MarshalByRefObject
	{
		public string Text { get; private set; }
		public string FileName { get; set; }
		public bool IsSelected { get; set; }
		public int ImageIndex { get; private set; }
		public int SelectedImageIndex { get; private set; }
		
		IList<TreeScanResult> children;
		
		public IList<TreeScanResult> Children {
			get {
				if (children == null)
					children = new List<TreeScanResult>();
				return children;
			}
		}
		
		public TreeScanResult(string text, int imageIndex, int selectedImageIndex)
		{
			this.Text = text;
			this.ImageIndex = imageIndex;
			this.SelectedImageIndex = selectedImageIndex;
		}
	}
	
	public static class ResourceHelpers
	{
		public static IResourceReader CreateResourceReader(Stream stream, ResourceType type)
		{
			if (stream.Length == 0)
				return null;
			if (type == ResourceType.Resources) {
				return new ResourceReader(stream);
			}
			return new ResXResourceReader(stream);
		}
		
		public static IResourceWriter CreateResourceWriter(Stream stream, ResourceType type)
		{
			if (type == ResourceType.Resources) {
				return new ResourceWriter(stream);
			}
			return new ResXResourceWriter(stream);
		}
		
		public static ResourceType GetResourceType(string fileName)
		{
			if (Path.GetExtension(fileName).ToLowerInvariant() == ".resx") {
				return ResourceType.Resx;
			}
			return ResourceType.Resources;
		}
	}
}
