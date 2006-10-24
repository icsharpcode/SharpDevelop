// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.Build.Tasks;

namespace ICSharpCode.MonoAddIn
{
	public class MonoProjectContentRegistry : ProjectContentRegistry
	{
		protected override IProjectContent LoadProjectContent(string itemInclude, string itemFileName)
		{
			if (File.Exists(itemFileName)) {
				return ParserService.DefaultProjectContentRegistry.GetProjectContentForReference(itemInclude, itemFileName);
			}
			MonoAssemblyName assemblyName = MonoGlobalAssemblyCache.FindAssemblyName(itemInclude);
			if (assemblyName != null && assemblyName.FileName != null) {
				return CecilReader.LoadAssembly(assemblyName.FileName, this);
			} else {
				if (MonoToolLocationHelper.IsMonoInstalled) {
					HostCallback.ShowAssemblyLoadError(itemFileName, itemInclude, "Could not find assembly in Mono's GAC.");
				} else {
					HostCallback.ShowAssemblyLoadError(itemFileName, itemInclude, "Could not find assembly in Mono's GAC - Mono is not installed.");
				}
				return null;
			}
		}
	}
}
