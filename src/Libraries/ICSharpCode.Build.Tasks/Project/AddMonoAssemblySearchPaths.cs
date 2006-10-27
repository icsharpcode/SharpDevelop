// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ICSharpCode.Build.Tasks
{
	/// <summary>
	/// Adds GAC folders to the assembly search path for any GAC references.
	/// </summary>
	public class AddMonoAssemblySearchPaths : Task
	{
		/// <summary>
		/// String used in the AssemblySearchPath property to specify where
		/// Mono GAC folder items should be added.
		/// </summary>
		public const string MonoGacSearchPathIndicator = "{MonoGAC}";
		
		string[] paths;
		ITaskItem[] assemblies;
		
		public AddMonoAssemblySearchPaths()
		{
		}
		
		/// <summary>
		/// Gets or sets the Mono assembly search paths.
		/// </summary>
		[Required]
		[Output]
		public string[] Paths {
			get {
				return paths;
			}
			set {
				paths = value;
			}
		}
		
		/// <summary>
		/// These are the assembly references in the project being built.  This 
		/// set of items is also passed to the ResolveAssemblyReference task.
		/// </summary>
		[Required]
		public ITaskItem[] Assemblies {
			get {
				return assemblies;
			}
			set {
				assemblies = value;
			}
		}
		
		/// <summary>
		/// Replaces the {MonoGAC} entry in the AssemblySearchPaths.
		/// </summary>
		public override bool Execute()
		{
			List<string> updatedSearchPaths = new List<string>();
			List<string> monoGacSearchPaths = new List<string>();
			
			if (assemblies != null) {
				foreach (ITaskItem item in assemblies) {
					string monoGacFolder = GetMonoGacFolder(item);
					if (monoGacFolder != null) {
						monoGacSearchPaths.Add(monoGacFolder);
					}
				}
			}
			
			// Add Mono GAC search paths to existing search paths.
			foreach (string path in paths) {
				if (path.Equals(MonoGacSearchPathIndicator, StringComparison.InvariantCultureIgnoreCase)) {	
					updatedSearchPaths.AddRange(monoGacSearchPaths);
				} else {
					updatedSearchPaths.Add(path);
				}
			}
			paths = new string[updatedSearchPaths.Count];
			updatedSearchPaths.CopyTo(paths);
			
			return true;
		}
		
		/// <summary>
		/// Gets the corresponding Mono GAC folder for the task item.
		/// </summary>
		/// <remarks>
		/// Basic logic is:
		/// 
		/// 1) If the Gac reference has a full specified assembly name 
		///    (e.g. name, version, culture, public key token) then just generate
		///    the expected Gac folder.
		/// 2) If the Gac reference is a short name, then look in Mono's gac for
		///    latest version (i.e. highest version number) of the assembly and 
		///    adds it folder.
		/// 
		/// Extra possiblities:
		/// 
		/// 1) Verify the assembly actually exists and take action accordingly.
		/// 2) Allow partial assembly names (i.e short + version and nothing else).
		/// 3) Check the hint path actually resolves to an assembly otherwise add
		///    a possible GAC folder.
		/// </remarks>
		/// <returns><see langword="null"/> if no GAC folder can be found
		/// for the task item.</returns>
		string GetMonoGacFolder(ITaskItem item)
		{
			MonoAssemblyName assemblyName = MonoGlobalAssemblyCache.FindAssemblyName(item.ItemSpec);
			if (assemblyName != null) {
				return assemblyName.Directory;
			}
			return null;
		}
	}
}
