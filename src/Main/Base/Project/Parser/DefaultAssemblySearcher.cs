// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using Mono.Cecil;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.ClassBrowser;

namespace ICSharpCode.SharpDevelop.Parser
{
	public class DefaultAssemblySearcher : IAssemblySearcher
	{
		FileName mainAssemblyFileName;
		DirectoryName baseDirectory;
		Lazy<IDictionary<DomAssemblyName, FileName>> localAssemblies;
		
		public DefaultAssemblySearcher(FileName mainAssemblyFileName)
		{
			if (mainAssemblyFileName == null)
				throw new ArgumentNullException("mainAssemblyFileName");
			this.mainAssemblyFileName = mainAssemblyFileName;
			this.baseDirectory = mainAssemblyFileName.GetParentDirectory();
			this.localAssemblies = new Lazy<IDictionary<DomAssemblyName, FileName>>(
				delegate {
					var list = new Dictionary<DomAssemblyName, FileName>();
					var dirInfo = new DirectoryInfo(baseDirectory);
					
					foreach (var file in dirInfo.GetFiles("*.dll").Concat(dirInfo.GetFiles("*.exe"))) {
						try {
							var definition = AssemblyDefinition.ReadAssembly(file.FullName);
							var name = new DomAssemblyName(definition.FullName);
							if (!list.ContainsKey(name))
								list.Add(name, new FileName(file.FullName));
						} catch (IOException ex) {
							LoggingService.Warn("Ignoring error while scanning local assemblies from " + baseDirectory, ex);
						} catch (UnauthorizedAccessException ex) {
							LoggingService.Warn("Ignoring error while scanning local assemblies from " + baseDirectory, ex);
						}
					}
					
					return list;
				}
			);
		}
		
		public FileName FindAssembly(DomAssemblyName fullName)
		{
			// look for the assembly in the current loaded assembly lists
			var classBrowser = SD.GetRequiredService<IClassBrowser>();
			var relevantClassBrowserAssemblies = classBrowser.AssemblyLists
				.Where(l => l.Assemblies.Any(a => a.Location == mainAssemblyFileName))
				.SelectMany(l => l.Assemblies);
			foreach (var asm in relevantClassBrowserAssemblies) {
				// TODO I am pretty sure we need the full name here as well...
				if (asm.AssemblyName == fullName.ShortName)
					return asm.Location;
			}
			
			// scan current directory
			var files = localAssemblies.Value;
			FileName file;
			if (files.TryGetValue(fullName, out file))
				return file;
			
			// look in GAC
			return SD.GlobalAssemblyCache.FindAssemblyInNetGac(fullName);
		}
	}
}


