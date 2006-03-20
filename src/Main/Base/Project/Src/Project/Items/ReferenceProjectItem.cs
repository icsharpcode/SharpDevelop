// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
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
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.SpecificVersion}",
		                   Description = "${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.SpecificVersion.Description}")]
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
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.Name}",
		                   Description="${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.Name.Description}")]
		public string Name {
			get {
				AssemblyName assemblyName = GetAssemblyName(Include);
				if (assemblyName != null) {
					return assemblyName.Name;
				}
				return Include;
			}
		}
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.Version}",
		                   Description="${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.Version.Description}")]
		public Version Version {
			get {
				AssemblyName assemblyName = GetAssemblyName(Include);
				if (assemblyName != null) {
					return assemblyName.Version;
				}
				return null;
			}
		}
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.Culture}",
		                   Description="${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.Culture.Description}")]
		public string Culture {
			get {
				AssemblyName assemblyName = GetAssemblyName(Include);
				if (assemblyName != null && assemblyName.CultureInfo != null) {
					return assemblyName.CultureInfo.Name;
				}
				return null;
			}
		}
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.PublicKeyToken}",
		                   Description="${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.PublicKeyToken.Description}")]
		public string PublicKeyToken {
			get {
				AssemblyName assemblyName = GetAssemblyName(Include);
				if (assemblyName != null) {
					byte[] bytes = assemblyName.GetPublicKeyToken();
					if (bytes != null) {
						StringBuilder token = new StringBuilder();
						foreach (byte b in bytes) {
							token.Append(b.ToString("x2"));
						}
						return token.ToString();
					}
				}
				return null;
			}
		}
		
		public override string FileName {
			get {
				if (Project != null) {
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
		
		AssemblyName GetAssemblyName(string include)
		{
			try {
				if (this.ItemType == ItemType.Reference) {
					return new AssemblyName(include);
				}
			} catch (ArgumentException) { }
			
			return null;
		}
	}
}
