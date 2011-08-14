// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Documentation;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Tasks;
using Mono.Cecil;

namespace ICSharpCode.SharpDevelop.Parser
{
	/// <summary>
	/// Portions of parser service that deal with loading external assemblies for code completion.
	/// </summary>
	public static class AssemblyParserService
	{
		#region Get Assembly By File Name
		sealed class LoadedAssembly
		{
			public readonly Task<IProjectContent> ProjectContent;
			public readonly DateTime AssemblyFileLastWriteTime;
			
			public LoadedAssembly(Task<IProjectContent> projectContent, DateTime assemblyFileLastWriteTime)
			{
				this.ProjectContent = projectContent;
				this.AssemblyFileLastWriteTime = assemblyFileLastWriteTime;
			}
		}
		
		static Dictionary<FileName, WeakReference> projectContentDictionary = new Dictionary<FileName, WeakReference>();
		
		[ThreadStatic] static Dictionary<FileName, LoadedAssembly> up2dateProjectContents;
		
		public static IProjectContent GetAssembly(FileName fileName, CancellationToken cancellationToken = default(CancellationToken))
		{
			// We currently do not support cancelling the load operation itself, because another GetAssembly() call
			// with a different cancellation token might request the same assembly.
			bool isNewTask;
			LoadedAssembly asm = GetLoadedAssembly(fileName, out isNewTask);
			if (isNewTask)
				asm.ProjectContent.RunSynchronously();
			else
				asm.ProjectContent.Wait(cancellationToken);
			return asm.ProjectContent.Result;
		}
		
		public static Task<IProjectContent> GetAssemblyAsync(FileName fileName, CancellationToken cancellationToken = default(CancellationToken))
		{
			bool isNewTask;
			LoadedAssembly asm = GetLoadedAssembly(fileName, out isNewTask);
			if (isNewTask)
				asm.ProjectContent.Start();
			return asm.ProjectContent;
		}
		
		/// <summary>
		/// "using (AssemblyParserService.AvoidRedundantChecks())"
		/// Within the using block, the AssemblyParserService will only check once per assembly if the
		/// existing cached project content (if any) is up to date.
		/// Any additional accesses will return that cached project content without causing an update check.
		/// This applies only to the thread that called AvoidRedundantChecks() - other threads will
		/// perform update checks as usual.
		/// </summary>
		public static IDisposable AvoidRedundantChecks()
		{
			if (up2dateProjectContents != null)
				return null;
			up2dateProjectContents = new Dictionary<FileName, LoadedAssembly>();
			return new CallbackOnDispose(
				delegate {
					up2dateProjectContents = null;
					lock (projectContentDictionary) {
						CleanWeakDictionary();
					}
				});
		}
		
		static void CleanWeakDictionary()
		{
			List<FileName> removed = new List<FileName>();
			foreach (var pair in projectContentDictionary) {
				if (!pair.Value.IsAlive)
					removed.Add(pair.Key);
			}
			foreach (var key in removed)
				projectContentDictionary.Remove(key);
		}
		
		static LoadedAssembly GetLoadedAssembly(FileName fileName, out bool isNewTask)
		{
			isNewTask = false;
			LoadedAssembly asm;
			var up2dateProjectContents = AssemblyParserService.up2dateProjectContents;
			if (up2dateProjectContents != null) {
				if (up2dateProjectContents.TryGetValue(fileName, out asm))
					return asm;
			}
			DateTime lastWriteTime = File.GetLastWriteTimeUtc(fileName);
			lock (projectContentDictionary) {
				WeakReference wr;
				if (projectContentDictionary.TryGetValue(fileName, out wr)) {
					asm = (LoadedAssembly)wr.Target;
					if (asm != null && asm.AssemblyFileLastWriteTime == lastWriteTime) {
						return asm;
					}
				} else {
					wr = null;
				}
				var task = new Task<IProjectContent>(() => LoadAssembly(fileName, CancellationToken.None));
				isNewTask = true;
				asm = new LoadedAssembly(task, lastWriteTime);
				if (wr != null) {
					wr.Target = asm;
				} else {
					if (up2dateProjectContents == null)
						CleanWeakDictionary();
					wr = new WeakReference(asm);
					projectContentDictionary.Add(fileName, wr);
				}
				return asm;
			}
		}
		#endregion
		
		#region Load Assembly + XML documentation
		static IProjectContent LoadAssembly(string fileName, CancellationToken cancellationToken)
		{
			var param = new ReaderParameters();
			param.AssemblyResolver = new DummyAssemblyResolver();
			AssemblyDefinition asm = AssemblyDefinition.ReadAssembly(fileName, param);
			
			CecilLoader l = new CecilLoader();
			string xmlDocFile = FindXmlDocumentation(fileName, asm.MainModule.Runtime);
			if (xmlDocFile != null) {
				try {
					l.DocumentationProvider = new XmlDocumentationProvider(xmlDocFile);
				} catch (XmlException ex) {
					LoggingService.Warn("Ignoring error while reading xml doc from " + xmlDocFile, ex);
				} catch (IOException ex) {
					LoggingService.Warn("Ignoring error while reading xml doc from " + xmlDocFile, ex);
				} catch (UnauthorizedAccessException ex) {
					LoggingService.Warn("Ignoring error while reading xml doc from " + xmlDocFile, ex);
				}
			}
			l.InterningProvider = new SimpleInterningProvider();
			l.CancellationToken = cancellationToken;
			return l.LoadAssembly(asm);
		}
		
		// used to prevent Cecil from loading referenced assemblies
		sealed class DummyAssemblyResolver : IAssemblyResolver
		{
			public AssemblyDefinition Resolve(AssemblyNameReference name)
			{
				return null;
			}
			
			public AssemblyDefinition Resolve(string fullName)
			{
				return null;
			}
			
			public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
			{
				return null;
			}
			
			public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
			{
				return null;
			}
		}
		
		static readonly string referenceAssembliesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Reference Assemblies\Microsoft\\Framework");
		static readonly string frameworkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework");
		
		static string FindXmlDocumentation(string assemblyFileName, TargetRuntime runtime)
		{
			string fileName;
			switch (runtime) {
				case TargetRuntime.Net_1_0:
					fileName = LookupLocalizedXmlDoc(Path.Combine(frameworkPath, "v1.0.3705", assemblyFileName));
					break;
				case TargetRuntime.Net_1_1:
					fileName = LookupLocalizedXmlDoc(Path.Combine(frameworkPath, "v1.1.4322", assemblyFileName));
					break;
				case TargetRuntime.Net_2_0:
					fileName = LookupLocalizedXmlDoc(Path.Combine(frameworkPath, "v2.0.50727", assemblyFileName))
						?? LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, "v3.5"))
						?? LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, "v3.0"))
						?? LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, @".NETFramework\v3.5\Profile\Client"));
					break;
				case TargetRuntime.Net_4_0:
				default:
					fileName = LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, @".NETFramework\v4.0", assemblyFileName))
						?? LookupLocalizedXmlDoc(Path.Combine(frameworkPath, "v4.0.30319", assemblyFileName));
					break;
			}
			return fileName;
		}
		
		static string LookupLocalizedXmlDoc(string fileName)
		{
			return XmlDocumentationProvider.LookupLocalizedXmlDoc(fileName);
		}
		#endregion
		
		internal static string FindReferenceAssembly(string shortName)
		{
			string path = Path.Combine(referenceAssembliesPath, @".NETFramework\v4.0", shortName + ".dll");
			if (File.Exists(path))
				return path;
			else
				return null;
		}
	}
}
