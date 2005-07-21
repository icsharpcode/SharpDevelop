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
					#if DEBUG
					Console.WriteLine("Loading mscorlib...");
					int time = Environment.TickCount;
					#endif
					
					mscorlibContent = new ReflectionProjectContent(typeof(object).Assembly);
					contents["mscorlib"] = mscorlibContent;
					
					#if DEBUG
					Console.WriteLine("mscorlib loaded in {0} ms", Environment.TickCount - time);
					#endif
					return mscorlibContent;
				}
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
				return ParserService.GetProjectContent(((ProjectReferenceProjectItem)item).ReferencedProject);
			}
			lock (contents) {
				if (contents.ContainsKey(item.FileName)) {
					return contents[item.FileName];
				}
				if (contents.ContainsKey(item.Include)) {
					return contents[item.Include];
				}
				
				string shortName = item.Include;
				int pos = shortName.IndexOf(',');
				if (pos > 0)
					shortName = shortName.Substring(0, pos);

				StatusBarService.ProgressMonitor.BeginTask("Loading " + shortName + "...", 100);
				#if DEBUG
				int time = Environment.TickCount;
				string how = "??";
				#endif
				Assembly assembly = GetDefaultAssembly(shortName);
				try {
					if (assembly != null) {
						contents[item.Include] = new ReflectionProjectContent(assembly);
						#if DEBUG
						how = "typeof";
						#endif
						return contents[item.Include];
					}
					lookupDirectory = Path.GetDirectoryName(item.FileName);
					AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += AssemblyResolve;
					try {
						assembly = Assembly.ReflectionOnlyLoadFrom(item.FileName);
						if (assembly != null) {
							contents[item.FileName] = new ReflectionProjectContent(assembly);
							#if DEBUG
							how = "ReflectionOnly";
							#endif
							return contents[item.FileName];
						}
					} finally {
						AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= AssemblyResolve;
						lookupDirectory = null;
					}
				} catch (FileNotFoundException) {
					try {
						assembly = LoadGACAssembly(item.Include, true);
						if (assembly != null) {
							contents[item.Include] = new ReflectionProjectContent(assembly);
							#if DEBUG
							how = "PartialName";
							#endif
							return contents[item.Include];
						}
					} catch (Exception e) {
						Console.WriteLine("Can't load assembly '{0}' : " + e.Message, item.Include);
					}
				} catch (BadImageFormatException) {
					Console.WriteLine("BadImageFormat: " + shortName);
				} finally {
					#if DEBUG
					Console.WriteLine("Loaded {0} with {2} in {1}ms", item.Include, Environment.TickCount - time, how);
					#endif
					StatusBarService.ProgressMonitor.Done();
				}
			}
			return null;
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
			string shortName = e.Name;
			int pos = shortName.IndexOf(',');
			if (pos > 0)
				shortName = shortName.Substring(0, pos);
			string path = Path.Combine(lookupDirectory, shortName);
			if (File.Exists(path + ".dll"))
				return Assembly.ReflectionOnlyLoadFrom(path + ".dll");
			if (File.Exists(path + ".exe"))
				return Assembly.ReflectionOnlyLoadFrom(path + ".exe");
			if (File.Exists(path))
				return Assembly.ReflectionOnlyLoadFrom(path);
			return null;
		}
		
		public static Assembly LoadGACAssembly(string partialName, bool reflectionOnly)
		{
			#pragma warning disable 618
			return Assembly.LoadWithPartialName(partialName);
			#pragma warning restore 618
		}
	}
}
