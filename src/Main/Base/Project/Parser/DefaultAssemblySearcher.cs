// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
