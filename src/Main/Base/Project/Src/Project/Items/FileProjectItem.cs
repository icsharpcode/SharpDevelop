// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Widgets.DesignTimeSupport;

namespace ICSharpCode.SharpDevelop.Project
{
	public enum CopyToOutputDirectory {
		Never,
		Always,
		PreserveNewest
	}
	
	public class FileProjectItem : ProjectItem
	{
		/// <summary>
		/// Creates a new FileProjectItem with the specified include.
		/// </summary>
		public FileProjectItem(IProject project, ItemType itemType, string include)
			: base(project, itemType, include)
		{
		}
		
		/// <summary>
		/// Creates a new FileProjectItem including a dummy file.
		/// </summary>
		public FileProjectItem(IProject project, ItemType itemType)
			: base(project, itemType)
		{
		}
		
		internal FileProjectItem(IProject project, IProjectItemBackendStore buildItem)
			: base(project, buildItem)
		{
		}
		
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectFile.BuildAction}",
		                   Description ="${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectFile.BuildAction.Description}")]
		[Editor(typeof(BuildActionEditor), typeof(UITypeEditor))]
		public string BuildAction {
			get {
				return this.ItemType.ItemName;
			}
			set {
				this.ItemType = new ItemType(value);
				ReFilterProperties();
			}
		}
		
		[LocalizedProperty("${res:Global.FileName}",
		                   Description = "${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectFile.FileName.Description}")]
		[Browsable(true)]
		[ReadOnly(true)]
		public override string FileName {
			get { return base.FileName; }
			set { base.FileName = value; }
		}
		
		sealed class BuildActionEditor : DropDownEditor
		{
			protected override Control CreateDropDownControl(ITypeDescriptorContext context, IWindowsFormsEditorService editorService)
			{
				FileProjectItem item = context.Instance as FileProjectItem;
				if (item != null && item.Project != null) {
					return new DropDownEditorListBox(editorService, GetNames(item.Project.AvailableFileItemTypes));
				} else {
					return new DropDownEditorListBox(editorService, GetNames(ItemType.DefaultFileItems));
				}
			}
			
			static IEnumerable<string> GetNames(IEnumerable<ItemType> itemTypes)
			{
				return itemTypes.Select(it => it.ItemName);
			}
		}
		
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectFile.CopyToOutputDirectory}",
		                   Description = "${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectFile.CopyToOutputDirectory.Description}")]
		public CopyToOutputDirectory CopyToOutputDirectory {
			get {
				return GetEvaluatedMetadata("CopyToOutputDirectory", CopyToOutputDirectory.Never);
			}
			set {
				SetEvaluatedMetadata("CopyToOutputDirectory", value);
			}
		}
		
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectFile.CustomTool}",
		                   Description ="${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectFile.CustomTool.Description}")]
		[Editor(typeof(CustomToolEditor), typeof(UITypeEditor))]
		public string CustomTool {
			get {
				return GetEvaluatedMetadata("Generator");
			}
			set {
				SetEvaluatedMetadata("Generator", value);
				ReFilterProperties();
			}
		}
		
		sealed class CustomToolEditor : DropDownEditor
		{
			protected override Control CreateDropDownControl(ITypeDescriptorContext context, IWindowsFormsEditorService editorService)
			{
				FileProjectItem item = context.Instance as FileProjectItem;
				if (item != null) {
					return new DropDownEditorListBox(editorService, CustomToolsService.GetCompatibleCustomToolNames(item));
				} else {
					return new DropDownEditorListBox(editorService, CustomToolsService.GetCustomToolNames());
				}
			}
		}

		[Browsable(false)]
		public string DependentUpon {
			get {
				return GetEvaluatedMetadata("DependentUpon");
			}
			set {
				SetEvaluatedMetadata("DependentUpon", value);
			}
		}

		[Browsable(false)]
		public string SubType {
			get {
				return GetEvaluatedMetadata("SubType");
			}
			set {
				SetEvaluatedMetadata("SubType", value);
			}
		}
		
		[Browsable(false)]
		public bool IsLink {
			get {
				return HasMetadata("Link") || !FileUtility.IsBaseDirectory(this.Project.Directory, this.FileName);
			}
		}
		
		/// <summary>
		/// Gets the name of the file in the virtual project file system.
		/// This is normally the same as Include, except for linked files, where it is
		/// the value of Properties["Link"].
		/// </summary>
		[Browsable(false)]
		public string VirtualName {
			get {
				if (HasMetadata("Link"))
					return GetEvaluatedMetadata("Link");
				else if (FileUtility.IsBaseDirectory(this.Project.Directory, this.FileName))
					return this.Include;
				else
					return Path.GetFileName(this.Include);
			}
		}
	}
}
