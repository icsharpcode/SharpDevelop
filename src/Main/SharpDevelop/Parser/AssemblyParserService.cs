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
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Documentation;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.ClassBrowser;
using ICSharpCode.SharpDevelop.Project;
using Mono.Cecil;

namespace ICSharpCode.SharpDevelop.Parser
{
	/// <summary>
	/// Portions of parser service that deal with loading external assemblies for code completion.
	/// </summary>
	sealed class AssemblyParserService : IAssemblyParserService
	{
		#region Get Assembly By File Name
		[Serializable]
		[FastSerializerVersion(1)]
		sealed class LoadedAssembly
		{
			public readonly IUnresolvedAssembly ProjectContent;
			public readonly DateTime AssemblyFileLastWriteTime;
			public readonly bool HasInternalMembers;
			public readonly IReadOnlyList<DomAssemblyName> References;
			[NonSerialized]
			public IAssemblyModel Model;
			
			public LoadedAssembly(IUnresolvedAssembly projectContent, DateTime assemblyFileLastWriteTime, bool hasInternalMembers, IEnumerable<DomAssemblyName> references)
			{
				this.ProjectContent = projectContent;
				this.AssemblyFileLastWriteTime = assemblyFileLastWriteTime;
				this.HasInternalMembers = hasInternalMembers;
				this.References = references.ToArray();
			}
		}
		
		// TODO: use weak reference to IProjectContent (not to LoadedAssembly!) so that unused assemblies can be unloaded
		Dictionary<FileName, LoadedAssembly> projectContentDictionary = new Dictionary<FileName, LoadedAssembly>();
		
		[ThreadStatic] static Dictionary<FileName, LoadedAssembly> up2dateProjectContents;
		
		public IUnresolvedAssembly GetAssembly(FileName fileName, bool includeInternalMembers = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			// We currently do not support cancelling the load operation itself, because another GetAssembly() call
			// with a different cancellation token might request the same assembly.
			return GetLoadedAssembly(fileName, includeInternalMembers).ProjectContent;
		}
		
		/// <summary>
		/// "using (AssemblyParserService.AvoidRedundantChecks())"
		/// Within the using block, the AssemblyParserService will only check once per assembly if the
		/// existing cached project content (if any) is up to date.
		/// Any additional accesses will return that cached project content without causing an update check.
		/// This applies only to the thread that called AvoidRedundantChecks() - other threads will
		/// perform update checks as usual.
		/// </summary>
		public IDisposable AvoidRedundantChecks()
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
		
		void CleanWeakDictionary()
		{
//			Debug.Assert(Monitor.IsEntered(projectContentDictionary));
//			List<FileName> removed = new List<FileName>();
//			foreach (var pair in projectContentDictionary) {
//				if (!pair.Value.IsAlive)
//					removed.Add(pair.Key);
//			}
//			foreach (var key in removed)
//				projectContentDictionary.Remove(key);
		}
		
		LoadedAssembly GetLoadedAssembly(FileName fileName, bool includeInternalMembers)
		{
			LoadedAssembly asm;
			var up2dateProjectContents = AssemblyParserService.up2dateProjectContents;
			if (up2dateProjectContents != null) {
				if (up2dateProjectContents.TryGetValue(fileName, out asm))
					return asm;
			}
			DateTime lastWriteTime = File.GetLastWriteTimeUtc(fileName);
			lock (projectContentDictionary) {
				if (projectContentDictionary.TryGetValue(fileName, out asm)) {
					if (asm.AssemblyFileLastWriteTime == lastWriteTime) {
						if (!includeInternalMembers || includeInternalMembers == asm.HasInternalMembers)
							return asm;
					}
				} else {
					asm = null;
				}
				asm = LoadAssembly(fileName, CancellationToken.None, includeInternalMembers);
				if (up2dateProjectContents == null)
					CleanWeakDictionary();
				// The assembly might already be in the dictionary if we had loaded it before,
				// but now the lastWriteTime changed.
				projectContentDictionary[fileName] = asm;
				
				return asm;
			}
		}
		#endregion
		
		#region Load Assembly
		LoadedAssembly LoadAssembly(FileName fileName, CancellationToken cancellationToken, bool includeInternalMembers)
		{
			DateTime lastWriteTime = File.GetLastWriteTimeUtc(fileName);
			string cacheFileName = GetCacheFileName(fileName);
			LoadedAssembly pc = TryReadFromCache(cacheFileName, lastWriteTime);
			if (pc != null) {
				if (!includeInternalMembers || includeInternalMembers == pc.HasInternalMembers)
					return pc;
			}
			
			//LoggingService.Debug("Loading " + fileName);
			cancellationToken.ThrowIfCancellationRequested();
			var param = new ReaderParameters();
			param.AssemblyResolver = new DummyAssemblyResolver();
			ModuleDefinition module = ModuleDefinition.ReadModule(fileName, param);
			
			CecilLoader l = new CecilLoader();
			l.IncludeInternalMembers = includeInternalMembers;
			string xmlDocFile = FindXmlDocumentation(fileName, module.Runtime);
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
			l.CancellationToken = cancellationToken;
			var references = module.AssemblyReferences
				.Select(anr => new DomAssemblyName(anr.FullName));
			pc = new LoadedAssembly(l.LoadModule(module), lastWriteTime, includeInternalMembers, references);
			SaveToCacheAsync(cacheFileName, lastWriteTime, pc).FireAndForget();
			//SaveToCache(cacheFileName, lastWriteTime, pc);
			return pc;
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
		#endregion
		
		#region Lookup XML documentation
		static readonly string referenceAssembliesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Reference Assemblies\Microsoft\\Framework");
		static readonly string frameworkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework");
		
		static string FindXmlDocumentation(FileName assemblyFileName, TargetRuntime runtime)
		{
			// First, look in the same directory as the assembly.
			// This usually works for .NET reference assembies and 3rd party libraries
			string xmlFileName = LookupLocalizedXmlDoc(assemblyFileName);
			if (xmlFileName != null)
				return xmlFileName;
			// Otherwise, assume the assembly is part of .NET and try looking
			// in the framework directory.
			// (if the assembly is not part of .NET, we won't find anything there)
			string name = assemblyFileName.GetFileName();
			switch (runtime) {
				case TargetRuntime.Net_1_0:
					xmlFileName = LookupLocalizedXmlDoc(Path.Combine(frameworkPath, "v1.0.3705", name));
					break;
				case TargetRuntime.Net_1_1:
					xmlFileName = LookupLocalizedXmlDoc(Path.Combine(frameworkPath, "v1.1.4322", name));
					break;
				case TargetRuntime.Net_2_0:
					xmlFileName = LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, "v3.5", name))
						?? LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, @".NETFramework\v3.5\Profile\Client", name))
						?? LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, "v3.0", name))
						?? LookupLocalizedXmlDoc(Path.Combine(frameworkPath, "v2.0.50727", name));
					break;
				case TargetRuntime.Net_4_0:
				default:
					xmlFileName = LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, @".NETFramework\v4.5.1", name))
						?? LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, @".NETFramework\v4.5", name))
						?? LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, @".NETFramework\v4.0", name))
						?? LookupLocalizedXmlDoc(Path.Combine(frameworkPath, "v4.0.30319", name));
					break;
			}
			return xmlFileName;
		}
		
		static string LookupLocalizedXmlDoc(string fileName)
		{
			return XmlDocumentationProvider.LookupLocalizedXmlDoc(fileName);
		}
		#endregion
		
		#region DomPersistence
		/// <summary>
		/// Gets/Sets the directory for cached project contents.
		/// </summary>
		public string DomPersistencePath { get; set; }
		
		string GetCacheFileName(FileName assemblyFileName)
		{
			if (DomPersistencePath == null)
				return null;
			string cacheFileName = Path.GetFileNameWithoutExtension(assemblyFileName);
			if (cacheFileName.Length > 32)
				cacheFileName = cacheFileName.Substring(cacheFileName.Length - 32); // use 32 last characters
			cacheFileName = Path.Combine(DomPersistencePath, cacheFileName + "." + assemblyFileName.ToString().ToUpperInvariant().GetStableHashCode().ToString("x8") + ".dat");
			return cacheFileName;
		}
		
		static LoadedAssembly TryReadFromCache(string cacheFileName, DateTime lastWriteTime)
		{
			if (cacheFileName == null || !File.Exists(cacheFileName))
				return null;
			//LoggingService.Debug("Deserializing " + cacheFileName);
			try {
				using (FileStream fs = new FileStream(cacheFileName, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete, 4096, FileOptions.SequentialScan)) {
					using (BinaryReader reader = new BinaryReaderWith7BitEncodedInts(fs)) {
						if (reader.ReadInt64() != lastWriteTime.Ticks) {
							LoggingService.Debug("Timestamp mismatch, deserialization aborted. (" + cacheFileName + ")");
							return null;
						}
						FastSerializer s = new FastSerializer();
						return s.Deserialize(reader) as LoadedAssembly;
					}
				}
			} catch (IOException ex) {
				LoggingService.Warn(ex);
				return null;
			} catch (UnauthorizedAccessException ex) {
				LoggingService.Warn(ex);
				return null;
			} catch (SerializationException ex) {
				LoggingService.Warn(ex);
				return null;
			}
		}
		
		Task SaveToCacheAsync(string cacheFileName, DateTime lastWriteTime, LoadedAssembly asm)
		{
			if (cacheFileName == null)
				return Task.FromResult<object>(null);
			
			// Call SaveToCache on a background task:
			var shutdownService = SD.ShutdownService;
			var task = IOTaskScheduler.Factory.StartNew(delegate { SaveToCache(cacheFileName, lastWriteTime, asm); }, shutdownService.ShutdownToken);
			shutdownService.AddBackgroundTask(task);
			return task;
		}
		
		void SaveToCache(string cacheFileName, DateTime lastWriteTime, LoadedAssembly asm)
		{
			if (cacheFileName == null)
				return;
			LoggingService.Debug("Serializing to " + cacheFileName);
			try {
				Directory.CreateDirectory(DomPersistencePath);
				using (FileStream fs = new FileStream(cacheFileName, FileMode.Create, FileAccess.Write)) {
					using (BinaryWriter writer = new BinaryWriterWith7BitEncodedInts(fs)) {
						writer.Write(lastWriteTime.Ticks);
						FastSerializer s = new FastSerializer();
						s.Serialize(writer, asm);
					}
				}
			} catch (IOException ex) {
				LoggingService.Warn(ex);
				// Can happen if two SD instances are trying to access the file at the same time.
				// We'll just let one of them win, and instance that got the exception won't write to the cache at all.
				// Similarly, we also ignore the other kinds of IO exceptions.
			} catch (UnauthorizedAccessException ex) {
				LoggingService.Warn(ex);
			}
		}
		#endregion
		
		#region Refresh
		public event EventHandler<RefreshAssemblyEventArgs> AssemblyRefreshed;
		
		public void RefreshAssembly(FileName fileName)
		{
			LoadedAssembly asm;
			lock (projectContentDictionary) {
				if (!projectContentDictionary.TryGetValue(fileName, out asm)) {
					// Assembly is not loaded; nothing to refresh
					return;
				}
			}
			var oldAssembly = asm.ProjectContent;
			var newAssembly = GetAssembly(fileName);
			if (oldAssembly != newAssembly) {
				var handler = AssemblyRefreshed;
				if (handler != null)
					handler(this, new RefreshAssemblyEventArgs(fileName, oldAssembly, newAssembly));
			}
		}
		#endregion
		
		public IAssemblyModel GetAssemblyModel(FileName fileName, bool includeInternalMembers = false)
		{
			LoadedAssembly assembly = GetLoadedAssembly(fileName, includeInternalMembers);
			if (assembly.Model == null) {
				// Get references
				DefaultAssemblySearcher assemblySearcher = new DefaultAssemblySearcher(fileName);
				var referencedAssemblies = new List<IUnresolvedAssembly>();
				foreach (var referencedAssemblyName in assembly.References) {
					var assemblyFileName = assemblySearcher.FindAssembly(referencedAssemblyName);
					if (assemblyFileName != null) {
						var loadedRefAssembly = GetLoadedAssembly(assemblyFileName, includeInternalMembers);
						if (loadedRefAssembly != null) {
							referencedAssemblies.Add(loadedRefAssembly.ProjectContent);
						}
					}
				}
				
				IEntityModelContext context = new AssemblyEntityModelContext(assembly.ProjectContent, referencedAssemblies.ToArray());
				IUpdateableAssemblyModel model = SD.GetService<IModelFactory>().CreateAssemblyModel(context);
				
				model.Update(EmptyList<IUnresolvedTypeDefinition>.Instance, assembly.ProjectContent.TopLevelTypeDefinitions.ToList());
				model.AssemblyName = assembly.ProjectContent.AssemblyName;
				model.FullAssemblyName = assembly.ProjectContent.FullAssemblyName;
				model.UpdateReferences(assembly.References);
				assembly.Model = model;
			}
			
			return assembly.Model;
		}
		
		public IAssemblyModel GetAssemblyModelSafe(FileName fileName, bool includeInternalMembers = false)
		{
			try {
				return GetAssemblyModel(fileName, includeInternalMembers);
			} catch (BadImageFormatException) {
				SD.MessageService.ShowWarningFormatted("${res:ICSharpCode.SharpDevelop.Dom.AssemblyInvalid}", Path.GetFileName(fileName));
			} catch (FileNotFoundException) {
				SD.MessageService.ShowWarningFormatted("${res:ICSharpCode.SharpDevelop.Dom.AssemblyNotAccessible}", fileName);
			}
			
			return null;
		}
	}
}
