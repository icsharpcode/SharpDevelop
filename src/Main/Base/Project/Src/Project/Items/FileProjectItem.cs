// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.IO;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public enum CopyToOutputDirectory {
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
		
		[LocalizedProperty("Copy to output Directory",
		                   Description ="Specifies if the file should be copied to the output directory")]
		public CopyToOutputDirectory CopyToOutputDirectory {
			get {
				return base.Properties.Get("CopyToOutputDirectory", CopyToOutputDirectory.Never);
			}
			set {
				base.Properties.Set("CopyToOutputDirectory", value);
			}
		}
		
		[LocalizedProperty("Custom Tool",
		                   Description ="Specifies the tool that converts the file to the output.")]
		public string CustomTool {
			get {
				return base.Properties["Generator"];
			}
			set {
				base.Properties["Generator"] = value;
			}
		}
		
		[LocalizedProperty("Custom Tool Namespace",
		                   Description ="Specifies the namespace the custom tool places it's output.")]
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
				return base.Properties.IsSet("Link");
			}
		}
		
		public FileProjectItem(IProject project, ItemType type) : base(project)
		{
			this.type = type;
		}
	}
}
