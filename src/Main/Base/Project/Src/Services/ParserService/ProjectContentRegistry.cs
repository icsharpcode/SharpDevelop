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
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.ReflectionLayer;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using MSjogren.GacTool.FusionNative;

namespace ICSharpCode.Core
{
	public static class ProjectContentRegistry
	{
		static Dictionary<string, IProjectContent> contents = new Dictionary<string, IProjectContent>(StringComparer.InvariantCultureIgnoreCase);
		static ReflectionProjectContent mscorlibContent;
		
		public static void RunLocked(MethodInvoker method)
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
		
		public static IEnumerable<IProjectContent> LoadedProjectContents {
			get {
				return contents.Values;
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
		
		public static IProjectContent GetExistingProjectContentForReference(ReferenceProjectItem item)
		{
			if (item is ProjectReferenceProjectItem) {
				if (((ProjectReferenceProjectItem)item).ReferencedProject == null)
				{
					return null;
				}
				return ParserService.GetProjectContent(((ProjectReferenceProjectItem)item).ReferencedProject);
			}
			lock (contents) {
				string itemInclude = item.Include;
				string itemFileName = item.FileName;
				if (contents.ContainsKey(itemFileName)) {
					return contents[itemFileName];
				}
				if (contents.ContainsKey(itemInclude)) {
					return contents[itemInclude];
				}
			}
			return null;
		}
		
		public static IProjectContent GetProjectContentForReference(ReferenceProjectItem item)
		{
			if (item is ProjectReferenceProjectItem) {
				if (((ProjectReferenceProjectItem)item).ReferencedProject == null)
				{
					return null;
				}
				return ParserService.GetProjectContent(((ProjectReferenceProjectItem)item).ReferencedProject);
			}
			lock (contents) {
				string itemInclude = item.Include;
				string itemFileName = item.FileName;
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
				
				StatusBarService.ProgressMonitor.BeginTask("Loading " + shortName + "...", 100);
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
							AssemblyName asmName = FindBestMatchingAssemblyName(itemInclude);
							if (asmName != null) {
								string subPath = FileUtility.Combine(asmName.Name,
								                                     GetVersion__Token(asmName),
								                                     asmName.Name + ".dll");
								foreach (string dir in Directory.GetDirectories(Fusion.GetGacPath(), "GAC*")) {
									itemFileName = Path.Combine(dir, subPath);
									if (File.Exists(itemFileName)) {
										pc = CecilReader.LoadAssembly(itemFileName);
										break;
									}
								}
							}
							if (pc == null) {
								WorkbenchSingleton.SafeThreadAsyncCall((Action3<string, string, string>)ShowErrorMessage,
								                                       new object[] { "?", itemInclude, "Could not find assembly file." });
							}
						}
					}
					if (pc != null) {
						contents[item.Include] = pc;
						contents[shortName] = pc;
						contents[pc.AssemblyFullName] = pc;
					}
					return pc;
				} finally {
					#if DEBUG
					LoggingService.DebugFormatted("Loaded {0} in {1}ms", itemInclude, Environment.TickCount - time);
					#endif
					StatusBarService.ProgressMonitor.Done();
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
				WorkbenchSingleton.SafeThreadAsyncCall((Action3<string, string, string>)ShowErrorMessage,
				                                       new object[] { filename, include, e.Message });
			} catch (Exception e) {
				database = null;
				MessageService.ShowError(e, "Error loading code-completion information for " + include + " from " + filename);
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
		
		delegate void Action3<A, B, C>(A a, B b, C c);
		
		static void ShowErrorMessage(string filename, string include, string message)
		{
			WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).BringPadToFront();
			TaskService.BuildMessageViewCategory.AppendText("Error loading code-completion information for "
			                                                + include + " from " + filename
			                                                + ":\r\n" + message + "\r\n");
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
				case "System.Data":
					return typeof(System.Data.DataException).Assembly;
				case "System.Design":
					return typeof(System.ComponentModel.Design.DesignSurface).Assembly;
				case "System.Drawing":
					return typeof(System.Drawing.Color).Assembly;
				case "System.Web.Services":
					return typeof(System.Web.Services.WebService).Assembly;
				case "System.Windows.Forms":
					return typeof(System.Windows.Forms.Control).Assembly;
				case "System.Xml":
				case "System.XML":
					return typeof(XmlReader).Assembly;
				case "Microsoft.Build.Engine":
					return typeof(Microsoft.Build.BuildEngine.BuildSettings).Assembly;
				case "Microsoft.Build.Framework":
					return typeof(Microsoft.Build.Framework.LoggerVerbosity).Assembly;
				default:
					return null;
			}
		}
		
		
		public static Assembly LoadGACAssembly(string partialName, bool reflectionOnly)
		{
			if (reflectionOnly) {
				AssemblyName name = FindBestMatchingAssemblyName(partialName);
				if (name == null)
					return null;
				return Assembly.ReflectionOnlyLoad(name.FullName);
			} else {
				#pragma warning disable 618
				return Assembly.LoadWithPartialName(partialName);
				#pragma warning restore 618
			}
		}
		
		public static AssemblyName FindBestMatchingAssemblyName(string name)
		{
			string[] info = name.Split(',');
			string version = (info.Length > 1) ? info[1].Substring(info[1].LastIndexOf('=') + 1) : null;
			string publicKey = (info.Length > 3) ? info[3].Substring(info[3].LastIndexOf('=') + 1) : null;
			
			IApplicationContext applicationContext = null;
			IAssemblyEnum assemblyEnum = null;
			IAssemblyName assemblyName;
			Fusion.CreateAssemblyNameObject(out assemblyName, info[0], 0, 0);
			Fusion.CreateAssemblyEnum(out assemblyEnum, null, assemblyName, 2, 0);
			List<string> names = new List<string>();
			
			while (assemblyEnum.GetNextAssembly(out applicationContext, out assemblyName, 0) == 0) {
				uint nChars = 0;
				assemblyName.GetDisplayName(null, ref nChars, 0);
				
				StringBuilder sb = new StringBuilder((int)nChars);
				assemblyName.GetDisplayName(sb, ref nChars, 0);
				
				string fullName = sb.ToString();
				if (publicKey != null) {
					info = fullName.Split(',');
					if (publicKey != info[3].Substring(info[3].LastIndexOf('=') + 1)) {
						// Assembly has wrong public key
						continue;
					}
				}
				names.Add(fullName);
			}
			if (names.Count == 0)
				return null;
			string best = null;
			Version bestVersion = null;
			Version currentVersion;
			if (version != null) {
				// use assembly with lowest version higher or equal to required version
				Version requiredVersion = new Version(version);
				for (int i = 0; i < names.Count; i++) {
					info = names[i].Split(',');
					currentVersion = new Version(info[1].Substring(info[1].LastIndexOf('=') + 1));
					if (currentVersion.CompareTo(requiredVersion) < 0)
						continue; // version not good enough
					if (best == null || currentVersion.CompareTo(bestVersion) < 0) {
						bestVersion = currentVersion;
						best = names[i];
					}
				}
				if (best != null)
					return new AssemblyName(best);
			}
			// use assembly with highest version
			best = names[0];
			info = names[0].Split(',');
			bestVersion = new Version(info[1].Substring(info[1].LastIndexOf('=') + 1));
			for (int i = 1; i < names.Count; i++) {
				info = names[i].Split(',');
				currentVersion = new Version(info[1].Substring(info[1].LastIndexOf('=') + 1));
				if (currentVersion.CompareTo(bestVersion) > 0) {
					bestVersion = currentVersion;
					best = names[i];
				}
			}
			return new AssemblyName(best);
		}
	}
}
