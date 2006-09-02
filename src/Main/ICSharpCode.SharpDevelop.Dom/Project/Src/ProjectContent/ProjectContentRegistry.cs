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
	/// <summary>
	/// Contains project contents read from external assemblies.
	/// Caches loaded assemblies in memory and optionally also to disk.
	/// </summary>
	public class ProjectContentRegistry : IDisposable
	{
		internal DomPersistence persistence;
		internal Dictionary<string, IProjectContent> contents = new Dictionary<string, IProjectContent>(StringComparer.InvariantCultureIgnoreCase);
		
		/// <summary>
		/// Disposes all project contents stored in this registry.
		/// </summary>
		public virtual void Dispose()
		{
			List<IProjectContent> list;
			lock (contents) {
				list = new List<IProjectContent>(contents.Values);
				contents.Clear();
			}
			// dispose outside the lock
			foreach (IProjectContent pc in list) {
				pc.Dispose();
			}
		}
		
		/// <summary>
		/// Activate caching assemblies to disk.
		/// Cache files will be saved in the specified directory.
		/// </summary>
		public DomPersistence ActivatePersistence(string cacheDirectory)
		{
			if (cacheDirectory == null) {
				throw new ArgumentNullException("cacheDirectory");
			} else if (persistence != null && cacheDirectory == persistence.CacheDirectory) {
				return persistence;
			} else {
				persistence = new DomPersistence(cacheDirectory, this);
				return persistence;
			}
		}
		
		/// <summary>
		/// Load a project content using Reflection in a separate AppDomain.
		/// This method first tries to load the assembly from the disk cache, if it exists there.
		/// If it does not exist in disk cache, it creates a new AppDomain, instanciates a ReflectionLoader in it
		/// and loads the assembly <paramref name="filename"/>.
		/// If the file does not exist, <paramref name="include"/> is loaded from GAC.
		/// </summary>
		public ReflectionProjectContent ReflectionLoadProjectContent(string filename, string include)
		{
			DomPersistence persistence;
			bool tempPersistence;
			if (this.persistence == null) {
				tempPersistence = true;
				persistence = new DomPersistence(Path.GetTempPath(), this);
			} else {
				// non-temp persistence
				tempPersistence = false;
				persistence = this.persistence;
				
				ReflectionProjectContent pc = persistence.LoadProjectContentByAssemblyName(filename);
				if (pc != null) {
					return pc;
				}
				pc = persistence.LoadProjectContentByAssemblyName(include);
				if (pc != null) {
					return pc;
				}
			}
			
			AppDomainSetup setup = new AppDomainSetup();
			setup.DisallowCodeDownload = true;
			setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
			AppDomain domain = AppDomain.CreateDomain("AssemblyLoadingDomain", AppDomain.CurrentDomain.Evidence, setup);
			
			string database;
			try {
				object o = domain.CreateInstanceAndUnwrap(typeof(ReflectionLoader).Assembly.FullName, typeof(ReflectionLoader).FullName);
				ReflectionLoader loader = (ReflectionLoader)o;
				database = loader.LoadAndCreateDatabase(filename, include, persistence.CacheDirectory);
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
				try {
					return persistence.LoadProjectContent(database);
				} finally {
					if (tempPersistence) {
						try {
							File.Delete(database);
						} catch {}
					}
				}
			}
		}
		
		
		ReflectionProjectContent mscorlibContent;
		
		public void RunLocked(ThreadStart method)
		{
			lock (contents) {
				method();
			}
		}
		
		public virtual IProjectContent Mscorlib {
			get {
				if (mscorlibContent != null) return mscorlibContent;
				lock (contents) {
					if (contents.ContainsKey("mscorlib")) {
						mscorlibContent = (ReflectionProjectContent)contents["mscorlib"];
						return contents["mscorlib"];
					}
					int time = LoggingService.IsDebugEnabled ? Environment.TickCount : 0;
					LoggingService.Debug("Loading PC for mscorlib...");
					if (persistence != null) {
						mscorlibContent = persistence.LoadProjectContentByAssemblyName(MscorlibAssembly.FullName);
						if (mscorlibContent != null) {
							if (time != 0) {
								LoggingService.Debug("Loaded mscorlib from cache in " + (Environment.TickCount - time) + " ms");
							}
						}
					}
					if (mscorlibContent == null) {
						// TODO: switch back to reflection for mscorlib
						// We're using Cecil now for everything to find bugs in CecilReader faster
						mscorlibContent = CecilReader.LoadAssembly(MscorlibAssembly.Location, this);
						//mscorlibContent = new ReflectionProjectContent(MscorlibAssembly);
						if (time != 0) {
							LoggingService.Debug("Loaded mscorlib with Cecil in " + (Environment.TickCount - time) + " ms");
							//LoggingService.Debug("Loaded mscorlib with Reflection in " + (Environment.TickCount - time) + " ms");
						}
						if (persistence != null) {
							persistence.SaveProjectContent(mscorlibContent);
							LoggingService.Debug("Saved mscorlib to cache");
						}
					}
					contents["mscorlib"] = mscorlibContent;
					contents[mscorlibContent.AssemblyFullName] = mscorlibContent;
					contents[mscorlibContent.AssemblyLocation] = mscorlibContent;
					return mscorlibContent;
				}
			}
		}
		
		public virtual ICollection<IProjectContent> GetLoadedProjectContents()
		{
			lock (contents) { // we need to return a copy because we have to lock
				return new List<IProjectContent>(contents.Values);
			}
		}
		
		public IProjectContent GetExistingProjectContent(AssemblyName assembly)
		{
			return GetExistingProjectContent(assembly.FullName, assembly.FullName);
		}
		
		public virtual IProjectContent GetExistingProjectContent(string itemInclude, string itemFileName)
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
		
		public virtual IProjectContent GetProjectContentForReference(string itemInclude, string itemFileName)
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
				
				IProjectContent pc = null;
				try {
					pc = LoadProjectContent(itemInclude, itemFileName);
				} catch (Exception ex) {
					HostCallback.ShowAssemblyLoadErrorInternal(itemFileName, itemInclude, "Error loading assembly:\n" + ex.ToString());
				} finally {
					#if DEBUG
					LoggingService.Debug(string.Format("Loaded {0} in {1}ms", itemInclude, Environment.TickCount - time));
					#endif
					HostCallback.FinishAssemblyLoad();
				}
				
				if (pc != null) {
					contents[itemInclude] = pc;
					contents[itemFileName] = pc;
				}
				return pc;
			}
		}
		
		protected virtual IProjectContent LoadProjectContent(string itemInclude, string itemFileName)
		{
			string shortName = itemInclude;
			int pos = shortName.IndexOf(',');
			if (pos > 0)
				shortName = shortName.Substring(0, pos);
			
			Assembly assembly = GetDefaultAssembly(shortName);
			ReflectionProjectContent pc = null;
			if (assembly != null) {
				if (persistence != null) {
					pc = persistence.LoadProjectContentByAssemblyName(assembly.FullName);
				}
				if (pc == null) {
					pc = new ReflectionProjectContent(assembly, this);
					persistence.SaveProjectContent(pc);
				}
			} else {
				// find real file name for cecil:
				if (File.Exists(itemFileName)) {
					pc = CecilReader.LoadAssembly(itemFileName, this);
				} else {
					AssemblyName asmName = GacInterop.FindBestMatchingAssemblyName(itemInclude);
					if (asmName != null) {
						string subPath = Path.Combine(asmName.Name, GetVersion__Token(asmName));
						subPath = Path.Combine(subPath, asmName.Name + ".dll");
						foreach (string dir in Directory.GetDirectories(GacInterop.GacRootPath, "GAC*")) {
							itemFileName = Path.Combine(dir, subPath);
							if (File.Exists(itemFileName)) {
								pc = CecilReader.LoadAssembly(itemFileName, this);
								break;
							}
						}
					}
					if (pc == null) {
						HostCallback.ShowAssemblyLoadErrorInternal("?", itemInclude, "Could not find assembly file.");
					}
				}
			}
			return pc;
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
			return null; // TODO: remove this line when CecilReader was tested enough and we can use Reflection again for the BCL
			
			// These assemblies are already loaded by SharpDevelop, so we don't need to load
			// them in a separate AppDomain.
			switch (shortName) {
				case "System": // System != mscorlib !!!
					return SystemAssembly;
				case "System.Data":
				case "System.Design":
				case "System.Drawing":
				case "System.Web.Services":
				case "System.Windows.Forms":
					return Assembly.Load(shortName);
				case "System.Xml":
				case "System.XML":
					return typeof(XmlReader).Assembly;
				default:
					return null;
			}
		}
	}
}
