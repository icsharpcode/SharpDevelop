// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public enum CopyToOutputDirectory {
		[Description("test")]
		Never,
		Always,
		PreserveNewest
	}
	/// <summary>
	/// Description of CompileProjectItem.
	/// </summary>
	public class FileProjectItem : ProjectItem
	{
		ItemType type;
		
		public override ItemType ItemType {
			get {
				return type;
			}
		}
		
		public enum FileBuildAction {
			None                  = ItemType.None,
			Compile               = ItemType.Compile,
			EmbeddedResource      = ItemType.EmbeddedResource,
			Resource              = ItemType.Resource,
			Content               = ItemType.Content,
			ApplicationDefinition = ItemType.ApplicationDefinition,
			Page                  = ItemType.Page
		}
		
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectFile.BuildAction}",
		                   Description ="${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectFile.BuildAction.Description}")]
		public FileBuildAction BuildAction {
			get {
				return (FileBuildAction)type;
			}
			set {
				type = (ItemType)value;
			}
		}
		
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectFile.CopyToOutputDirectory}",
		                   Description = "${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectFile.CopyToOutputDirectory.Description}")]
		public CopyToOutputDirectory CopyToOutputDirectory {
			get {
				return base.Properties.Get("CopyToOutputDirectory", CopyToOutputDirectory.Never);
			}
			set {
				base.Properties.Set("CopyToOutputDirectory", value);
			}
		}
		
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectFile.CustomTool}",
		                   Description ="${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectFile.CustomTool.Description}")]
		public string CustomTool {
			get {
				return base.Properties["Generator"];
			}
			set {
				base.Properties["Generator"] = value;
			}
		}
		
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectFile.CustomToolNamespace}",
		                   Description ="${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectFile.CustomToolNamespace.Description}")]
		public string CustomToolNamespace {
			get {
				return base.Properties["CustomToolNamespace"];
			}
			set {
				base.Properties["CustomToolNamespace"] = value;
			}
		}
		
		[Browsable(false)]
		public string DependentUpon {
			get {
				return base.Properties["DependentUpon"];
			}
			set {
				base.Properties["DependentUpon"] = value;
			}
		}

		[Browsable(false)]
		public string SubType {
			get {
				return base.Properties["SubType"];
			}
			set {
				base.Properties["SubType"] = value;
			}
		}
		
		[Browsable(false)]
		public bool IsLink {
			get {
				return base.Properties.IsSet("Link") || !FileUtility.IsBaseDirectory(this.Project.Directory, this.FileName);
			}
		}
		
		[Browsable(false)]
		/// <summary>
		/// Gets the name of the file in the virtual project file system.
		/// This is normally the same as Include, except for linked files, where it is
		/// the value of Properties["Link"].
		/// </summary>
		public string VirtualName {
			get {
				if (base.Properties.IsSet("Link"))
					return base.Properties["Link"];
				else if (FileUtility.IsBaseDirectory(this.Project.Directory, this.FileName))
					return this.Include;
				else
					return Path.GetFileName(this.Include);
			}
		}
		
		public FileProjectItem(IProject project, ItemType type) : base(project)
		{
			this.type = type;
		}
		
		public override ProjectItem Clone()
		{
			ProjectItem n = new FileProjectItem(this.Project, this.ItemType);
			n.Include = this.Include;
			this.CopyExtraPropertiesTo(n);
			return n;
		}
	}
}
