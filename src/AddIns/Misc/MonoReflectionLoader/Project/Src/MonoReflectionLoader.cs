// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Build.Tasks;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using System;
using System.IO;
using System.Reflection;

namespace ICSharpCode.MonoAddIn
{
	public sealed class MonoReflectionLoader : MarshalByRefObject
	{
		public MonoReflectionLoader()
		{
		}
		
		public string LoadAndCreateDatabase(string fileName, string include)
		{
			try {
				LoggingService.Debug("MonoReflectionLoader: Load: " + include);
				ReflectionProjectContent content = LoadProjectContent(fileName, include);
				if (content == null)
					return null;
				return DomPersistence.SaveProjectContent(content);
			} catch (Exception ex) {
				LoggingService.Error(ex);
				return null;
			}
		}
		
		ReflectionProjectContent LoadProjectContent(string fileName, string include)
		{
			fileName = Path.GetFullPath(fileName);
			Assembly assembly;
			AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += AssemblyResolve;
			try {
				if (File.Exists(fileName)) {
					assembly = Assembly.ReflectionOnlyLoadFrom(fileName);
					return new ReflectionProjectContent(assembly, fileName);
				}
				assembly = LoadMonoGacAssembly(include);
				if (assembly != null)
					return new ReflectionProjectContent(assembly);
				else
					return null;
			} catch (BadImageFormatException) {
				return null;
			} finally {
				AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= AssemblyResolve;
			}
		}
		
		Assembly AssemblyResolve(object sender, ResolveEventArgs e)
		{
			return LoadMonoGacAssembly(e.Name);
		}
		
		Assembly LoadMonoGacAssembly(string name)
		{
			MonoAssemblyName assemblyName = MonoGlobalAssemblyCache.FindAssemblyName(name);
			if (assemblyName != null && assemblyName.FileName != null) {
				return Assembly.ReflectionOnlyLoadFrom(assemblyName.FileName);
			}
			return null;
		}
	}
}
