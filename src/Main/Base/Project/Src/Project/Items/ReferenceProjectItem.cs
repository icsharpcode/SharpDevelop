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
		
		[DefaultValue(false)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.LocalCopy}",
		                   Description = "${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.LocalCopy.Description}")]
		public bool Private {
			get {
				return Properties.Get("Private", false);
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
					if (hintPath != null && hintPath.Length > 0)
					{
						return Path.Combine(Project.Directory, hintPath);
					}
					string name = Path.Combine(Project.Directory, Include);
					if (File.Exists(name))
					{
						return name;
					}
				}
				catch (Exception) { }

				return GetPathToGACAssembly(Include);
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
		
		static string GetPathToGACAssembly(string referenceName)
		{ 
			string[] info = referenceName.Split(',');
			
			if (info.Length < 4) {
				try {
					Assembly refAssembly = Assembly.LoadWithPartialName(referenceName);
					
					// if it failed, then return just the short name
					if (refAssembly == null) {
						return info[0];
					}
					
					// split up the peices again to find the assembly file path
					info = refAssembly.FullName.Split(',');
				} catch (Exception) {
					return referenceName;
				}
			}
			
			string aName      = info[0];
			string aVersion   = info[1].Substring(info[1].LastIndexOf('=') + 1);
			string aPublicKey = info[3].Substring(info[3].LastIndexOf('=') + 1);
			
			return FileUtility.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.System),
			                       "..",
			                       "assembly",
			                       "GAC",
			                       aName,
			                       aVersion + "__" + aPublicKey,
			                       aName + ".dll");
		}
	}
}
