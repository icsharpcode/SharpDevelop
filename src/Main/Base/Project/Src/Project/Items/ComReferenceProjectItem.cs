using System;
using System.IO;
using System.Reflection;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ComReferenceProjectItem : ReferenceProjectItem
	{
		public override ItemType ItemType {
			get {
				return ItemType.COMReference;
			}
		}
		
		public string Guid {
			get {
				return base.Properties["Guid"];
			}
			set {
				base.Properties["Guid"] = value;
			}
		}
		
		public int VersionMajor {
			get {
				return base.Properties.Get("VersionMajor", 1);
			}
			set {
				base.Properties.Set("VersionMajor", value);
			}
		}
		
		public int VersionMinor {
			get {
				return base.Properties.Get("VersionMinor", 0);
			}
			set {
				base.Properties.Set("VersionMinor", value);
			}
		}
		
		public string Lcid {
			get {
				return base.Properties["Lcid"];
			}
			set {
				base.Properties["Lcid"] = value;
			}
		}
		
		public string WrapperTool {
			get {
				return base.Properties["WrapperTool"];
			}
			set {
				base.Properties["WrapperTool"] = value;
			}
		}
		public bool Isolated {
			get {
				return base.Properties.Get("Isolated", false);
			}
			set {
				base.Properties.Set("Isolated", value);
			}
		}
		
		public ComReferenceProjectItem(IProject project) : base(project)
		{
		}
		public ComReferenceProjectItem(IProject project, TypeLibrary library) : base(project)
		{
			this.Include      = library.Name;
			this.Guid         = library.Guid;
			this.VersionMajor = library.VersionMajor;
			this.VersionMinor = library.VersionMinor;
			this.Lcid         = library.Lcid;
			this.WrapperTool  = library.WrapperTool;
			this.Isolated     = library.Isolated;
		}
		
		public override string ToString()
		{
			return String.Format("[ComReferenceProjectItem: Include={0}, Properties={1}]",
			                     Include,
			                     Properties);
		}
	}
}
