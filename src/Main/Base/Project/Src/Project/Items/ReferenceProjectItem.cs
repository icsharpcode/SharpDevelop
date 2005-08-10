// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Description of ReferenceProjectItem.
	/// </summary>
	public class ReferenceProjectItem : ProjectItem
	{
		public override ItemType ItemType {
			get {
				return ItemType.Reference;
			}
		}
		
		[Browsable(false)]
		public string HintPath {
			get {
				return Properties["HintPath"];
			}
			set {
				Properties["HintPath"] = value;
			}
		}
		
		[DefaultValue(false)]
		[LocalizedProperty("Specific Version",
		                   Description = "Indicates if this reference is bound to a specific version of the assembly.")]
		public bool SpecificVersion {
			get {
				return Properties.Get("SpecificVersion", true);
			}
			set {
				Properties.Set("SpecificVersion", value);
			}
		}
		
		[DefaultValue(true)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.LocalCopy}",
		                   Description = "${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.LocalCopy.Description}")]
		public bool Private {
			get {
				return Properties.Get("Private", true);
			}
			set {
				Properties.Set("Private", value);
			}
		}
		
		public override string FileName {
			get {
				try
				{
					string hintPath = HintPath;
					if (hintPath != null && hintPath.Length > 0) {
						return Path.Combine(Project.Directory, hintPath);
					}
					string name = Path.Combine(Project.Directory, Include);
					if (File.Exists(name)) {
						return name;
					}
					if (File.Exists(name + ".dll")) {
						return name + ".dll";
					}
					if (File.Exists(name + ".exe")) {
						return name + ".exe";
					}
				}
				catch (Exception) { }
				return Include;
			}
			set {
				// Set by file name is unsupported by references. (otherwise GAC references might have strange renaming effects ...)
			}
		}
		
		public ReferenceProjectItem(IProject project) : base(project)
		{
		}
		
		public ReferenceProjectItem(IProject project, string include) : base(project)
		{
			this.Include = include;
		}
		
		public override string ToString()
		{
			return String.Format("[ReferenceProjectItem: Include={0}, Properties={1}]",
			                     Include,
			                     Properties);
		}
	}
}
