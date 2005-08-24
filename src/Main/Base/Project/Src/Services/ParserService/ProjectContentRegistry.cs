// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Dom;
using MSjogren.GacTool.FusionNative;

namespace ICSharpCode.Core
{
	public static class ProjectContentRegistry
	{
		static Dictionary<string, IProjectContent> contents = new Dictionary<string, IProjectContent>(StringComparer.InvariantCultureIgnoreCase);
		static IProjectContent mscorlibContent;
		
		public static IProjectContent Mscorlib {
			get {
				if (mscorlibContent != null) return mscorlibContent;
				lock (contents) {
					if (contents.ContainsKey("mscorlib")) {
						mscorlibContent = contents["mscorlib"];
						return contents["mscorlib"];
					}
					LoggingService.Debug("Loading PC for mscorlib...");
					mscorlibContent = new ReflectionProjectContent(typeof(object).Assembly);
					contents["mscorlib"] = mscorlibContent;
					LoggingService.Debug("Finished loading mscorlib");
					return mscorlibContent;
				}
			}
		}
		
		public static IProjectContent WinForms {
			get {
				lock (contents) {
					if (contents.ContainsKey("System.Windows.Forms")) {
						return contents["System.Windows.Forms"];
					}
				}
				return GetProjectContentForReference(new ReferenceProjectItem(null, "System.Windows.Forms"));
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
		
		static string lookupDirectory;
		
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
				string how = "??";
				#endif
				try {
					Assembly assembly = GetDefaultAssembly(shortName);
					if (assembly != null) {
						contents[item.Include] = new ReflectionProjectContent(assembly);
						#if DEBUG
						how = "typeof";
						#endif
						return contents[itemInclude];
					}
					lookupDirectory = Path.GetDirectoryName(itemFileName);
					AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += AssemblyResolve;
					try {
						try {
							if (File.Exists(itemFileName)) {
								assembly = LoadReflectionOnlyAssemblyFrom(itemFileName);
								if (assembly != null) {
									contents[itemFileName] = new ReflectionProjectContent(assembly, itemFileName);
									contents[assembly.FullName] = contents[itemFileName];
									#if DEBUG
									how = "ReflectionOnly";
									#endif
									return contents[itemFileName];
								}
							}
						} catch (FileNotFoundException) {
							// ignore and try loading with LoadGACAssembly
						}
						try {
							assembly = LoadGACAssembly(itemInclude, true);
							if (assembly != null) {
								contents[itemInclude] = new ReflectionProjectContent(assembly);
								contents[assembly.FullName] = contents[itemInclude];
								#if DEBUG
								how = "PartialName";
								#endif
								return contents[itemInclude];
							}
						} catch (Exception e) {
							LoggingService.Warn("Can't load assembly '" + itemInclude + "' : " + e.Message);
						}
					} catch (BadImageFormatException) {
						LoggingService.Warn("BadImageFormat: " + itemInclude);
					} finally {
						AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= AssemblyResolve;
						lookupDirectory = null;
					}
				} finally {
					#if DEBUG
					LoggingService.DebugFormatted("Loaded {0} with {2} in {1}ms", itemInclude, Environment.TickCount - time, how);
					#endif
					StatusBarService.ProgressMonitor.Done();
				}
				return null;
			}
		}
		
		static Assembly GetDefaultAssembly(string shortName)
		{
			// GAC Assemblies take some time because first the non-GAC try
			// has to fail.
			// Therefore, assemblies already in use by SharpDevelop are used directly.
			switch (shortName) {
				case "System": // System != mscorlib !!!
					return typeof(Uri).Assembly;
				case "System.Data":
					return typeof(System.Data.DataException).Assembly;
				case "System.Design":
					return typeof(System.ComponentModel.Design.DesignSurface).Assembly;
				case "System.DirectoryServices":
					return typeof(System.DirectoryServices.AuthenticationTypes).Assembly;
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
		
		static Assembly AssemblyResolve(object sender, ResolveEventArgs e)
		{
			AssemblyName name = new AssemblyName(e.Name);
			LoggingService.Debug("ProjectContentRegistry.AssemblyResolve " + e.Name);
			string path = Path.Combine(lookupDirectory, name.Name);
			if (File.Exists(path + ".dll")) {
				LoggingService.Debug("AssemblyResolve ReflectionOnlyLoadFrom .dll file");
				return LoadReflectionOnlyAssemblyFrom(path + ".dll");
			}
			if (File.Exists(path + ".exe")) {
				LoggingService.Debug("AssemblyResolve ReflectionOnlyLoadFrom .exe file");
				return LoadReflectionOnlyAssemblyFrom(path + ".exe");
			}
			if (File.Exists(path)) {
				LoggingService.Debug("AssemblyResolve ReflectionOnlyLoadFrom file");
				return LoadReflectionOnlyAssemblyFrom(path);
			}
			try {
				LoggingService.Debug("AssemblyResolve trying ReflectionOnlyLoad");
				return Assembly.ReflectionOnlyLoad(e.Name);
			} catch (FileNotFoundException) {
				LoggingService.Warn("AssemblyResolve: ReflectionOnlyLoad failed for " + e.Name);
				// We can't get the assembly we want.
				// But propably we can get a similar version of it.
				AssemblyName fixedName = FindBestMatchingAssemblyName(e.Name);
				LoggingService.Info("AssemblyResolve: FixedName: " + fixedName);
				return Assembly.ReflectionOnlyLoad(fixedName.FullName);
			}
		}
		
		static Dictionary<string, Assembly> loadFromCache;
		
		static Assembly LoadReflectionOnlyAssemblyFrom(string fileName)
		{
			if (loadFromCache == null) {
				loadFromCache = new Dictionary<string, Assembly>(StringComparer.InvariantCultureIgnoreCase);
			}
			if (loadFromCache.ContainsKey(fileName))
				return loadFromCache[fileName];
			Assembly asm = InternalLoadReflectionOnlyAssemblyFrom(fileName);
			if (loadFromCache.ContainsKey(asm.FullName)) {
				loadFromCache.Add(fileName, loadFromCache[asm.FullName]);
				return loadFromCache[asm.FullName];
			}
			loadFromCache.Add(fileName, asm);
			loadFromCache.Add(asm.FullName, asm);
			return asm;
		}
		
		static Assembly InternalLoadReflectionOnlyAssemblyFrom(string fileName)
		{
			byte[] data;
			using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
				if (fs.Length > 10 * 1024 * 1024) {
					// more than 10 MB? Do not hold bytes in memory
					return Assembly.ReflectionOnlyLoadFrom(fileName);
				}
				data = new byte[fs.Length];
				for (int i = 0; i < data.Length;) {
					int c = fs.Read(data, i, data.Length - i);
					i += c;
					if (c <= 0) {
						throw new IOException("Read returned " + c);
					}
				}
			}
			return Assembly.ReflectionOnlyLoad(data);
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
	}
}
