// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.ReflectionLayer;

namespace ICSharpCode.SharpDevelop.Dom
{
	public static class ProjectContentRegistry
	{
		static Dictionary<string, IProjectContent> contents = new Dictionary<string, IProjectContent>(StringComparer.InvariantCultureIgnoreCase);
		static ReflectionProjectContent mscorlibContent;
		
		public static void RunLocked(ThreadStart method)
		{
			lock (contents) {
				method();
			}
		}
		
		public static IProjectContent Mscorlib {
			get {
				if (mscorlibContent != null) return mscorlibContent;
				lock (contents) {
					if (contents.ContainsKey("mscorlib")) {
						mscorlibContent = (ReflectionProjectContent)contents["mscorlib"];
						return contents["mscorlib"];
					}
					int time = LoggingService.IsDebugEnabled ? Environment.TickCount : 0;
					LoggingService.Debug("Loading PC for mscorlib...");
					mscorlibContent = DomPersistence.LoadProjectContentByAssemblyName(MscorlibAssembly.FullName);
					if (mscorlibContent == null) {
						mscorlibContent = new ReflectionProjectContent(MscorlibAssembly);
						//mscorlibContent = CecilReader.LoadAssembly(MscorlibAssembly.Location);
						if (time != 0) {
							LoggingService.Debug("Loaded mscorlib with Reflection in " + (Environment.TickCount - time) + " ms");
						}
						DomPersistence.SaveProjectContent(mscorlibContent);
						LoggingService.Debug("Saved mscorlib to cache");
					} else {
						if (time != 0) {
							LoggingService.Debug("Loaded mscorlib from cache in " + (Environment.TickCount - time) + " ms");
						}
					}
					contents["mscorlib"] = mscorlibContent;
					return mscorlibContent;
				}
			}
		}
		
		public static ICollection<IProjectContent> GetLoadedProjectContents()
		{
			lock (contents) { // we need to return a copy because we have to lock
				return new List<IProjectContent>(contents.Values);
			}
		}
		
		public static IProjectContent GetExistingProjectContent(AssemblyName assembly)
		{
			lock (contents) {
				if (contents.ContainsKey(assembly.FullName)) {
					return contents[assembly.FullName];
				}
				if (contents.ContainsKey(assembly.Name)) {
					return contents[assembly.Name];
				}
				return null;
			}
		}
		
		public static IProjectContent GetExistingProjectContentForReference(string itemInclude, string itemFileName)
		{
			lock (contents) {
				if (contents.ContainsKey(itemFileName)) {
					return contents[itemFileName];
				}
				if (contents.ContainsKey(itemInclude)) {
					return contents[itemInclude];
				}
			}
			return null;
		}
		
		public static IProjectContent GetProjectContentForReference(string itemInclude, string itemFileName)
		{
			lock (contents) {
				if (contents.ContainsKey(itemFileName)) {
					return contents[itemFileName];
				}
				if (contents.ContainsKey(itemInclude)) {
					return contents[itemInclude];
				}
				
				
				LoggingService.Debug("Loading PC for " + itemInclude);
				
				string shortName = itemInclude;
				int pos = shortName.IndexOf(',');
				if (pos > 0)
					shortName = shortName.Substring(0, pos);
				
				HostCallback.BeginAssemblyLoad(shortName);
				#if DEBUG
				int time = Environment.TickCount;
				#endif
				try {
					Assembly assembly = GetDefaultAssembly(shortName);
					ReflectionProjectContent pc;
					if (assembly != null) {
						pc = DomPersistence.LoadProjectContentByAssemblyName(assembly.FullName);
						if (pc == null) {
							pc = new ReflectionProjectContent(assembly);
							DomPersistence.SaveProjectContent(pc);
						}
					} else {
						//pc = LoadProjectContent(itemFileName, itemInclude);
						
						// find real file name for cecil:
						if (File.Exists(itemFileName)) {
							pc = CecilReader.LoadAssembly(itemFileName);
						} else {
							pc = null;
							AssemblyName asmName = GacInterop.FindBestMatchingAssemblyName(itemInclude);
							if (asmName != null) {
								string subPath = Path.Combine(asmName.Name, GetVersion__Token(asmName));
								subPath = Path.Combine(subPath, asmName.Name + ".dll");
								foreach (string dir in Directory.GetDirectories(GacInterop.GacRootPath, "GAC*")) {
									itemFileName = Path.Combine(dir, subPath);
									if (File.Exists(itemFileName)) {
										pc = CecilReader.LoadAssembly(itemFileName);
										break;
									}
								}
							}
							if (pc == null) {
								HostCallback.ShowAssemblyLoadErrorInternal("?", itemInclude, "Could not find assembly file.");
							}
						}
					}
					if (pc != null) {
						contents[itemInclude] = pc;
						contents[shortName] = pc;
						contents[pc.AssemblyFullName] = pc;
					}
					return pc;
				} catch (Exception ex) {
					HostCallback.ShowAssemblyLoadErrorInternal(itemFileName, itemInclude, "Error loading assembly:\n" + ex.ToString());
					return null;
				} finally {
					#if DEBUG
					LoggingService.Debug(string.Format("Loaded {0} in {1}ms", itemInclude, Environment.TickCount - time));
					#endif
					HostCallback.FinishAssemblyLoad();
				}
			}
		}
		
		static string GetVersion__Token(AssemblyName asmName)
		{
			StringBuilder b = new StringBuilder(asmName.Version.ToString());
			b.Append("__");
			foreach (byte by in asmName.GetPublicKeyToken()) {
				b.Append(by.ToString("x2"));
			}
			return b.ToString();
		}
		
		/// <summary>
		/// Load a project content using Reflection in a separate AppDomain.
		/// </summary>
		static ReflectionProjectContent LoadProjectContent(string filename, string include)
		{
			ReflectionProjectContent pc = DomPersistence.LoadProjectContentByAssemblyName(filename);
			if (pc != null) {
				return pc;
			}
			pc = DomPersistence.LoadProjectContentByAssemblyName(include);
			if (pc != null) {
				return pc;
			}
			AppDomainSetup setup = new AppDomainSetup();
			setup.DisallowCodeDownload = true;
			setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
			AppDomain domain = AppDomain.CreateDomain("AssemblyLoadingDomain", AppDomain.CurrentDomain.Evidence, setup);
			string database;
			try {
				object o = domain.CreateInstanceAndUnwrap(typeof(ReflectionLoader).Assembly.FullName, typeof(ReflectionLoader).FullName);
				ReflectionLoader loader = (ReflectionLoader)o;
				database = loader.LoadAndCreateDatabase(filename, include);
			} catch (FileLoadException e) {
				database = null;
				HostCallback.ShowAssemblyLoadErrorInternal(filename, include, e.Message);
			} catch (Exception e) {
				database = null;
				HostCallback.ShowError("Error loading code-completion information for " + include + " from " + filename, e);
			} finally {
				AppDomain.Unload(domain);
			}
			if (database == null) {
				LoggingService.Debug("AppDomain finished but returned null...");
				return null;
			} else {
				LoggingService.Debug("AppDomain finished, loading cache...");
				return DomPersistence.LoadProjectContent(database);
			}
		}
		
		public static Assembly MscorlibAssembly {
			get {
				return typeof(object).Assembly;
			}
		}
		
		public static Assembly SystemAssembly {
			get {
				return typeof(Uri).Assembly;
			}
		}
		
		static Assembly GetDefaultAssembly(string shortName)
		{
			// These assemblies are already loaded by SharpDevelop, so we don't need to load
			// them in a separate AppDomain.
			switch (shortName) {
				case "System": // System != mscorlib !!!
					return SystemAssembly;
				/*case "System.Data":
				case "System.Design":
				case "System.Drawing":
				case "System.Web.Services":
				case "System.Windows.Forms":
				case "Microsoft.Build.Engine":
				case "Microsoft.Build.Framework":
					return Assembly.Load(shortName);*/
				case "System.Xml":
				case "System.XML":
					return typeof(XmlReader).Assembly;
				default:
					return null;
			}
		}
		
		
		public static Assembly LoadGacAssembly(string partialName, bool reflectionOnly)
		{
			if (reflectionOnly) {
				AssemblyName name = GacInterop.FindBestMatchingAssemblyName(partialName);
				if (name == null)
					return null;
				return Assembly.ReflectionOnlyLoad(name.FullName);
			} else {
				#pragma warning disable 618
				return Assembly.LoadWithPartialName(partialName);
				#pragma warning restore 618
			}
		}
	}
}
