// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
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
		
		[ReadOnly(true)]
		public string Guid {
			get {
				return base.Properties["Guid"];
			}
			set {
				base.Properties["Guid"] = value;
			}
		}
		
		[ReadOnly(true)]
		public int VersionMajor {
			get {
				return base.Properties.Get("VersionMajor", 1);
			}
			set {
				base.Properties.Set("VersionMajor", value);
			}
		}
		
		[ReadOnly(true)]
		public int VersionMinor {
			get {
				return base.Properties.Get("VersionMinor", 0);
			}
			set {
				base.Properties.Set("VersionMinor", value);
			}
		}
		
		[ReadOnly(true)]
		public string Lcid {
			get {
				return base.Properties["Lcid"];
			}
			set {
				base.Properties["Lcid"] = value;
			}
		}
		
		[ReadOnly(true)]
		public string WrapperTool {
			get {
				return base.Properties["WrapperTool"];
			}
			set {
				base.Properties["WrapperTool"] = value;
			}
		}
		
		[ReadOnly(true)]
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
		
		public override string FileName {
			get {
				try {
					if (Project != null && Project.OutputAssemblyFullPath != null) {
						string outputFolder = Path.GetDirectoryName(Project.OutputAssemblyFullPath);
						string interopFileName = Path.Combine(outputFolder, String.Concat("Interop.", Include, ".dll"));
						if (File.Exists(interopFileName)) {
							return interopFileName;
						}
					}
				}
				catch (Exception) { }
				return Include;
			}
			set {
			}
		}
	}
}
