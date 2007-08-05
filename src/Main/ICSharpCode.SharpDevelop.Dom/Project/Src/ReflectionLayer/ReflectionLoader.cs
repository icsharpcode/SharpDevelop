// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Reflection;

namespace ICSharpCode.SharpDevelop.Dom
{
	public sealed class ReflectionLoader : MarshalByRefObject
	{
		public override string ToString()
		{
			return "ReflectionLoader in " + AppDomain.CurrentDomain.FriendlyName;
		}
		
		public static Assembly ReflectionLoadGacAssembly(string partialName, bool reflectionOnly)
		{
			if (reflectionOnly) {
				DomAssemblyName name = GacInterop.FindBestMatchingAssemblyName(partialName);
				if (name == null)
					return null;
				return Assembly.ReflectionOnlyLoad(name.FullName);
			} else {
				#pragma warning disable 618
				return Assembly.LoadWithPartialName(partialName);
				#pragma warning restore 618
			}
		}
		
		public string LoadAndCreateDatabase(string fileName, string include, string cacheDirectory)
		{
			try {
				ReflectionProjectContent content = LoadProjectContent(fileName, include, new ProjectContentRegistry());
				if (content == null)
					return null;
				return new DomPersistence(cacheDirectory, null).SaveProjectContent(content);
			} catch (Exception ex) {
				if (ex is FileLoadException) {
					LoggingService.Info(ex);
				} else {
					LoggingService.Error(ex);
				}
				throw;
			}
		}
		
		ReflectionProjectContent LoadProjectContent(string fileName, string include, ProjectContentRegistry registry)
		{
			fileName = Path.GetFullPath(fileName);
			LoggingService.Debug("Trying to load " + fileName);
			Assembly assembly;
			AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += AssemblyResolve;
			lookupDirectory = Path.GetDirectoryName(fileName);
			try {
				if (File.Exists(fileName)) {
					assembly = Assembly.ReflectionOnlyLoadFrom(fileName);
					return new ReflectionProjectContent(assembly, fileName, registry);
				}
				assembly = ReflectionLoadGacAssembly(include, true);
				if (assembly != null)
					return new ReflectionProjectContent(assembly, registry);
				else
					throw new FileLoadException("Assembly not found.");
			} catch (BadImageFormatException ex) {
				LoggingService.Warn("BadImageFormat: " + include);
				throw new FileLoadException(ex.Message, ex);
			} finally {
				AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= AssemblyResolve;
				lookupDirectory = null;
			}
		}
		
		string lookupDirectory;
		
		Assembly AssemblyResolve(object sender, ResolveEventArgs e)
		{
			AssemblyName name = new AssemblyName(e.Name);
			LoggingService.Debug("ProjectContentRegistry.AssemblyResolve " + e.Name);
			string path = Path.Combine(lookupDirectory, name.Name);
			if (File.Exists(path + ".dll")) {
				return Assembly.ReflectionOnlyLoadFrom(path + ".dll");
			}
			if (File.Exists(path + ".exe")) {
				return Assembly.ReflectionOnlyLoadFrom(path + ".exe");
			}
			if (File.Exists(path)) {
				return Assembly.ReflectionOnlyLoadFrom(path);
			}
			try {
				LoggingService.Debug("AssemblyResolve trying ReflectionOnlyLoad");
				return Assembly.ReflectionOnlyLoad(e.Name);
			} catch (FileNotFoundException) {
				LoggingService.Warn("AssemblyResolve: ReflectionOnlyLoad failed for " + e.Name);
				// We can't get the assembly we want.
				// But propably we can get a similar version of it.
				DomAssemblyName fixedName = GacInterop.FindBestMatchingAssemblyName(e.Name);
				LoggingService.Info("AssemblyResolve: FixedName: " + fixedName);
				return Assembly.ReflectionOnlyLoad(fixedName.FullName);
			}
		}
	}
}
