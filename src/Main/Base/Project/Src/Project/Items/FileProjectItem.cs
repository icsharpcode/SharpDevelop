using System;
using System.ComponentModel;
using System.IO;

namespace ICSharpCode.SharpDevelop.Project
{
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
		
		public ItemType BuildAction {
			get {
				return type;
			}
			set {
				type = value;
			}
		}
		
		public bool CopyToOutputDirectory {
			get {
				return base.Properties.Get("CopyToOutputDirectory", false);
			}
			set {
				base.Properties.Set("CopyToOutputDirectory", value);
			}
		}
		
		public string CustomTool {
			get {
				return base.Properties["Generator"];
			}
			set {
				base.Properties["Generator"] = value;
			}
		}
		
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
		
		public FileProjectItem(IProject project, ItemType type) : base(project)
		{
			this.type = type;
		}
	}
}
