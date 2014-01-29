// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.ClassBrowser;

namespace ICSharpCode.SharpDevelop.Parser
{
	public class DefaultAssemblySearcher : IAssemblySearcher
	{
		FileName mainAssemblyFileName;
		DirectoryName baseDirectory;
		
		public DefaultAssemblySearcher(FileName mainAssemblyFileName)
		{
			if (mainAssemblyFileName == null)
				throw new ArgumentNullException("mainAssemblyFileName");
			this.mainAssemblyFileName = mainAssemblyFileName;
			this.baseDirectory = mainAssemblyFileName.GetParentDirectory();
		}
		
		public FileName FindAssembly(DomAssemblyName fullName)
		{
			// look for the assembly in the current loaded assembly lists
			var classBrowser = SD.GetRequiredService<IClassBrowser>();
			var relevantClassBrowserAssemblies = classBrowser.AssemblyLists
				.Where(l => l.Assemblies.Any(a => a.Location == mainAssemblyFileName))
				.SelectMany(l => l.Assemblies);
			foreach (var asm in relevantClassBrowserAssemblies) {
				if (asm.FullAssemblyName == fullName.FullName)
					return asm.Location;
			}
			
			// look in GAC
			var gacFileName = SD.GlobalAssemblyCache.FindAssemblyInNetGac(fullName);
			if (gacFileName != null)
				return gacFileName;
			
			// scan current directory
			var fileName = baseDirectory.CombineFile(fullName.ShortName + ".dll");
			if (File.Exists(fileName))
				return fileName;
			
			fileName = baseDirectory.CombineFile(fullName.ShortName + ".exe");
			if (File.Exists(fileName))
				return fileName;
			return null;
		}
	}
}


