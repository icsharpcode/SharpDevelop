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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop.Designer
{
	public class TypeResolutionService : IDesignerTypeResolutionService
	{
		readonly static List<Assembly> designerAssemblies = new List<Assembly>();
		// hash of file content -> Assembly
		readonly static Dictionary<string, Assembly> assemblyDict = new Dictionary<string, Assembly>();
		
		/// <summary>
		/// List of assemblies used by the form designer. This static list is not an optimal solution,
		/// but better than using AppDomain.CurrentDomain.GetAssemblies(). See SD2-630.
		/// </summary>
		public static List<Assembly> DesignerAssemblies {
			get {
				return designerAssemblies;
			}
		}
		
		static TypeResolutionService()
		{
			ClearMixedAssembliesTemporaryFiles();
			DesignerAssemblies.Add(typeof(object).Assembly);
			DesignerAssemblies.Add(typeof(Uri).Assembly);
			DesignerAssemblies.Add(typeof(System.Drawing.Point).Assembly);
			DesignerAssemblies.Add(typeof(System.Windows.Forms.Design.AnchorEditor).Assembly);
			RegisterVSDesignerWorkaround();
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveEventHandler;
			
		}
		
		[System.Runtime.InteropServices.DllImport("kernel32.dll")]
		private static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, int dwFlags);
		const int MOVEFILE_DELAY_UNTIL_REBOOT = 0x00000004;
		
		static void MarkFileToDeleteOnReboot(string fileName)
		{
			MoveFileEx(fileName, null, MOVEFILE_DELAY_UNTIL_REBOOT);
		}

		static void ClearMixedAssembliesTemporaryFiles()
		{
			string[] files = Directory.GetFiles(Path.GetTempPath(), "*.sd_forms_designer_mixed_assembly.dll");
			foreach (string fileName in files) {
				try {
					File.Delete(fileName);
				} catch {}
			}
			/* We don't need to debug controls inside the forms designer
			files = Directory.GetFiles(Path.GetTempPath(), "*.pdb");
			foreach (string fileName in files) {
				try {
					File.Delete(fileName);
				} catch {}
			}*/
		}

		
		FileName formSourceFileName;
		IProject callingProject;
		/// <summary>
		/// Dictionary of file name -> hash of loaded assemblies for the currently designed document.
		/// Used to detect changes in references assemblies.
		/// </summary>
		readonly Dictionary<string, string> loadedAssembliesForCurrentDocument = new Dictionary<string, string>(StringComparer.Ordinal);
		
		/// <summary>
		/// Gets the project content of the project that created this TypeResolutionService.
		/// Returns null when no calling project was specified.
		/// </summary>
		public IProject CallingProject {
			get {
				if (formSourceFileName != null) {
					callingProject = SD.ProjectService.FindProjectContainingFile(formSourceFileName);
					formSourceFileName = null;
				}
				return callingProject;
			}
		}
		
		public TypeResolutionService()
		{
		}
		
		public TypeResolutionService(FileName formSourceFileName)
		{
			this.formSourceFileName = formSourceFileName;
		}
		
		readonly HashSet<IProject> projectContentsCurrentlyLoadingAssembly = new HashSet<IProject>();
		
		/// <summary>
		/// Loads the assembly represented by the project content. Returns null on failure.
		/// </summary>
		public Assembly LoadAssembly(IProject pc)
		{
			SD.MainThread.VerifyAccess();
			// prevent StackOverflow when project contents have cyclic dependencies
			// Very popular example of cyclic dependency: System <-> System.Xml
			if (!projectContentsCurrentlyLoadingAssembly.Add(pc))
				return null;
			
			Assembly sdAssembly;
			if (IsSharpDevelopAssembly(pc.ProjectContent, out sdAssembly))
				return sdAssembly;
			
			ICompilation compilation = SD.ParserService.GetCompilation(pc);
			
			try {
				// load dependencies of current assembly
				foreach (IAssembly asm in compilation.ReferencedAssemblies) {
					LoadAssembly(asm);
				}
			} finally {
				projectContentsCurrentlyLoadingAssembly.Remove(pc);
			}
			
			return LoadAssembly(pc.OutputAssemblyFullPath);
		}
		
		public Assembly LoadAssembly(IAssembly asm)
		{
			var project = asm.GetProject();
			if (project != null)
				return LoadAssembly(project);
			if (asm.IsGacAssembly())
				return LoadAssembly(new AssemblyName(asm.FullAssemblyName), false);
			return LoadAssembly(asm.GetRuntimeAssemblyLocation());
		}
		
		readonly string sharpDevelopRoot = Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).FullName;
		
		bool IsSharpDevelopAssembly(IUnresolvedAssembly pc, out Assembly assembly)
		{
			assembly = null;
			foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
				if (!asm.IsDynamic && asm.Location.StartsWith(sharpDevelopRoot, StringComparison.OrdinalIgnoreCase) && pc.AssemblyName == asm.GetName().Name) {
					assembly = asm;
					return true;
				}
			}
			return false;
		}
		
		static string GetHash(string fileName)
		{
			return Path.GetFileName(fileName).ToLowerInvariant() + File.GetLastWriteTimeUtc(fileName).Ticks.ToString();
		}
		
		static string GetOriginalAssemblyFullPath(Assembly asm)
		{
			if (asm == null) throw new ArgumentNullException("asm");
			try {
				return new Uri(asm.CodeBase, UriKind.Absolute).LocalPath;
			} catch (UriFormatException ex) {
				LoggingService.Warn("Could not determine path for assembly '" + asm.ToString() + "', CodeBase='" + asm.CodeBase + "': " + ex.Message);
				return asm.Location;
			} catch (InvalidOperationException ex) {
				LoggingService.Warn("Could not determine path for assembly '" + asm.ToString() + "', CodeBase='" + asm.CodeBase + "': " + ex.Message);
				return asm.Location;
			}
		}
		
		/// <summary>
		/// Loads the file in none-locking mode. Returns null on failure.
		/// </summary>
		public Assembly LoadAssembly(string fileName)
		{
			if (!File.Exists(fileName))
				return null;
			
			// FIX for SD2-716, remove when designer gets its own AppDomain
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
				try {
					if (!asm.IsDynamic && string.Equals(GetOriginalAssemblyFullPath(asm), fileName, StringComparison.OrdinalIgnoreCase)) {
						RegisterAssembly(asm);
						return asm;
					}
				} catch (NotSupportedException) {
					// Fixes forum-12823
				}
			}
			
			string hash = GetHash(fileName);
			lock (assemblyDict) {
				Assembly asm;
				if (assemblyDict.TryGetValue(hash, out asm))
					return asm;
				LoggingService.Debug("Loading assembly " + fileName + " (hash " + hash + ")");
				try {
					asm = Assembly.Load(File.ReadAllBytes(fileName));
				} catch (BadImageFormatException e) {
					if (e.Message.Contains("HRESULT: 0x8013141D")) {
						LoggingService.Debug("Get HRESULt 0x8013141D, loading netmodule");
						//netmodule
						string tempPath = Path.GetTempFileName();
						File.Delete(tempPath);
						tempPath += ".sd_forms_designer_netmodule_assembly.dll";
						
						try {
							//convert netmodule to assembly
							Process p = new Process();
							p.StartInfo.UseShellExecute = false;
							p.StartInfo.FileName = Path.GetDirectoryName(typeof(object).Module.FullyQualifiedName) + Path.DirectorySeparatorChar + "al.exe";
							p.StartInfo.Arguments = "\"" + fileName +"\" /out:\"" + tempPath + "\"";
							p.StartInfo.CreateNoWindow = true;
							p.Start();
							p.WaitForExit();
							
							if(p.ExitCode == 0 && File.Exists(tempPath)) {
								byte[] asm_data = File.ReadAllBytes(tempPath);
								asm = Assembly.Load(asm_data);
								asm.LoadModule(Path.GetFileName(fileName), File.ReadAllBytes(fileName));
							}
						} catch (Exception ex) {
							MessageService.ShowException(ex, "Error calling linker for netmodule");
						}
						try {
							File.Delete(tempPath);
						} catch {}
					} else {
						// Show other load errors in the compiler message view,
						// but do not prevent the designer from loading.
						// The error might be caused by an assembly that is
						// not even needed for the designer to load.
						LoggingService.Error("Error loading assembly " + fileName, e);
						SD.Workbench.GetPad(typeof(CompilerMessageView)).BringPadToFront();
						TaskService.BuildMessageViewCategory.AppendText(
							StringParser.Parse("${res:FileUtilityService.ErrorWhileLoading}")
							+ "\r\n" + fileName + "\r\n" + e.Message + "\r\n"
						);
						return null;
					}
				} catch (FileLoadException e) {
					if (e.Message.Contains("HRESULT: 0x80131402")) {
						LoggingService.Debug("Get HRESULt 0x80131402, loading mixed modes asm from disk");
						//this is C++/CLI Mixed assembly which can only be loaded from disk, not in-memory
						string tempPath = Path.GetTempFileName();
						File.Delete(tempPath);
						tempPath += ".sd_forms_designer_mixed_assembly.dll";
						File.Copy(fileName, tempPath);
						
						/* We don't need to debug controls inside the forms designer
						string pdbpath = Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(fileName) + ".pdb";
						if (File.Exists(pdbpath)) {
							string newpdbpath = Path.GetTempPath() + Path.DirectorySeparatorChar + Path.GetFileName(pdbpath);
							try {
								File.Copy(pdbpath, newpdbpath);
								MarkFileToDeleteOnReboot(newpdbpath);
							} catch {
							}
						}
						 */
						asm = Assembly.LoadFile(tempPath);
						MarkFileToDeleteOnReboot(tempPath);
					} else if (e.Message.Contains("HRESULT: 0x80131019")) {
						LoggingService.Debug(e.Message);
						LoggingService.Debug("Attempting to load unverifiable assembly. Ignoring.");
						return null;
					} else {
						throw; // don't ignore other load errors
					}
				}
				
				lock (designerAssemblies) {
					if (!designerAssemblies.Contains(asm))
						designerAssemblies.Insert(0, asm);
				}
				lock (this.loadedAssembliesForCurrentDocument) {
					this.loadedAssembliesForCurrentDocument[fileName] = hash;
				}
				assemblyDict[hash] = asm;
				return asm;
			}
		}
		
		void RegisterAssembly(Assembly asm)
		{
			string file = GetOriginalAssemblyFullPath(asm);
			if (!String.IsNullOrEmpty(file)) {
				string hash = GetHash(file);
				lock (assemblyDict) {
					assemblyDict[hash] = asm;
				}
				lock (this.loadedAssembliesForCurrentDocument) {
					this.loadedAssembliesForCurrentDocument[file] = hash;
				}
			}
			lock (designerAssemblies) {
				if (!designerAssemblies.Contains(asm))
					designerAssemblies.Insert(0, asm);
			}
		}
		
		public Assembly GetAssembly(AssemblyName name)
		{
			return LoadAssembly(name, false);
		}
		
		public Assembly GetAssembly(AssemblyName name, bool throwOnError)
		{
			return LoadAssembly(name, throwOnError);
		}
		
		Assembly LoadAssembly(AssemblyName name, bool throwOnError)
		{
			try {
				Assembly asm = Assembly.Load(name);
				RegisterAssembly(asm);
				return asm;
			} catch (System.IO.FileLoadException) {
				if (throwOnError)
					throw;
				return null;
			}
		}
		
		public string GetPathOfAssembly(AssemblyName name)
		{
			Assembly assembly = GetAssembly(name);
			if (assembly != null) {
				return GetOriginalAssemblyFullPath(assembly);
			}
			return null;
		}
		
		public Type GetType(string name)
		{
			return GetType(name, false, false);
		}
		
		public Type GetType(string name, bool throwOnError)
		{
			return GetType(name, throwOnError, false);
		}
		
		#if DEBUG
		int count = 0;
		#endif
		
		Dictionary<string, Type> typeCache = new Dictionary<string, Type>(StringComparer.Ordinal);
		Dictionary<string, Type> typeCacheIgnoreCase = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
		
		public void ClearCaches()
		{
			typeCacheIgnoreCase.Clear();
			typeCache.Clear();
		}
		
		public Type GetType(string name, bool throwOnError, bool ignoreCase)
		{
			if (string.IsNullOrEmpty(name)) {
				return null;
			}
			if (IgnoreType(name)) {
				return null;
			}
			if (ignoreCase) {
				Type cachedType;
				if (typeCacheIgnoreCase.TryGetValue(name, out cachedType))
					return cachedType;
			} else {
				Type cachedType;
				if (typeCache.TryGetValue(name, out cachedType))
					return cachedType;
			}
			#if DEBUG
			if (!name.StartsWith("System.", StringComparison.Ordinal)) {
				count++;
				LoggingService.Debug(count + " TypeResolutionService: Looking for " + name);
			}
			#endif
			try {
				
				Type type = Type.GetType(name, false, ignoreCase);
				
				if (type == null) {
					IProject p = CallingProject;
					if (p != null) {
						ICompilation compilation = SD.ParserService.GetCompilation(p);
						ITypeDefinition definition = ReflectionHelper.ParseReflectionName(name)
							.Resolve(compilation).GetDefinition();
						if (definition != null) {
							using (var resolver = new ProjectAssemblyResolver(compilation, this)) {
								Assembly assembly = LoadAssembly(definition.ParentAssembly);
								if (assembly != null) {
									type = assembly.GetType(name, false, ignoreCase);
								}
							}
						}
					}
				}
				
				// type lookup for typename, assembly, xyz style lookups
				if (type == null) {
					int idx = name.IndexOf(",", StringComparison.Ordinal);
					if (idx > 0) {
						string[] splitName = name.Split(',');
						string typeName     = splitName[0];
						string assemblyName = splitName[1].Substring(1);
						Assembly assembly = null;
						try {
							assembly = Assembly.Load(assemblyName);
						} catch (Exception e) {
							LoggingService.Error(e);
						}
						if (assembly != null) {
							string fileName = GetOriginalAssemblyFullPath(assembly);
							if (!String.IsNullOrEmpty(fileName)) {
								lock (loadedAssembliesForCurrentDocument) {
									loadedAssembliesForCurrentDocument [fileName] = GetHash(fileName);
								}
							}
							lock (designerAssemblies) {
								if (!designerAssemblies.Contains(assembly))
									designerAssemblies.Add(assembly);
							}
							type = assembly.GetType(typeName, false, ignoreCase);
						} else {
							type = Type.GetType(typeName, false, ignoreCase);
						}
					}
				}

				if (type == null) {
					lock (designerAssemblies) {
						foreach (Assembly asm in DesignerAssemblies) {
							try {
								Type t = asm.GetType(name, false);
								if (t != null) {
									AddToCache(name, t, ignoreCase);
									return t;
								}
							} catch (FileNotFoundException) {
							} catch (FileLoadException) {
							} catch (BadImageFormatException) {
								// ignore assembly load errors
							}
						}
					}
				}
				
				if (throwOnError && type == null)
					throw new TypeLoadException(name + " not found by TypeResolutionService");
				AddToCache(name, type, ignoreCase);
				return type;
			} catch (Exception e) {
				LoggingService.Error(e);
			}
			return null;
		}
		
		void AddToCache(string name, Type type, bool ignoreCase)
		{
			if (type == null)
				return;
			if (ignoreCase) {
				typeCacheIgnoreCase.Add(name, type);
			} else {
				typeCache.Add(name, type);
			}
		}
		
		public void ReferenceAssembly(AssemblyName name)
		{
			ICSharpCode.Core.LoggingService.Warn("TODO: Add Assembly reference : " + name);
		}
		
		/// <summary>
		/// Gets whether an assembly referenced by the currently designed document
		/// has been changed since it has been loaded.
		/// </summary>
		public bool ReferencedAssemblyChanged {
			get {
				return this.loadedAssembliesForCurrentDocument.Any(
					pair => !File.Exists(pair.Key) || !String.Equals(pair.Value, GetHash(pair.Key), StringComparison.Ordinal)
				);
			}
		}
		
		#region VSDesigner workarounds
		/// <summary>
		/// HACK - Ignore any requests for types from the Microsoft.VSDesigner
		/// assembly.  There are smart tag problems if data adapter
		/// designers are used from this assembly.
		/// </summary>
		bool IgnoreType(string name)
		{
			int idx = name.IndexOf(",");
			if (idx > 0) {
				string[] splitName = name.Split(',');
				string assemblyName = splitName[1].Substring(1);
				if (assemblyName == "Microsoft.VSDesigner") {
					return true;
				}
			}
			return false;
		}
		
		static string vsDesignerIdeDir;
		
		static void RegisterVSDesignerWorkaround()
		{
			if (vsDesignerIdeDir == null) {
				vsDesignerIdeDir = "";
				try {
					using(RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\VisualStudio\8.0\Setup\VS")) {
						if (key != null) {
							vsDesignerIdeDir = key.GetValue("VS7CommonDir") as string ?? "";
						}
					}
				} catch (System.Security.SecurityException) {
					// registry access might be denied
				}
				if (vsDesignerIdeDir.Length > 0) {
					vsDesignerIdeDir = Path.Combine(vsDesignerIdeDir, "IDE");
					AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs args) {
						string shortName = args.Name;
						if (shortName.IndexOf(',') >= 0) {
							shortName = shortName.Substring(0, shortName.IndexOf(','));
						}
						if (shortName.StartsWith("Microsoft.")
						    && File.Exists(Path.Combine(vsDesignerIdeDir, shortName + ".dll")))
						{
							return Assembly.LoadFrom(Path.Combine(vsDesignerIdeDir, shortName + ".dll"));
						}
						return null;
					};
				}
			}
		}
		#endregion
		
		static Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
		{
			LoggingService.Debug("TypeResolutionService: AssemblyResolveEventHandler: " + args.Name);
			
			Assembly lastAssembly = null;
			
			foreach (Assembly asm in TypeResolutionService.DesignerAssemblies) {
				if (asm.FullName == args.Name) {
					return asm;
				}
			}
			
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
				if (asm.FullName == args.Name) {
					lastAssembly = asm;
				}
			}
			if (lastAssembly != null) {
				TypeResolutionService.DesignerAssemblies.Add(lastAssembly);
				LoggingService.Info("ICSharpAssemblyResolver found..." + args.Name);
			}
			return lastAssembly;
		}
		
		class ProjectAssemblyResolver : IDisposable
		{
			readonly ICompilation compilation;
			readonly TypeResolutionService typeResolutionService;
			
			public ProjectAssemblyResolver(ICompilation compilation, TypeResolutionService typeResolutionService)
			{
				this.compilation = compilation;
				this.typeResolutionService = typeResolutionService;
				AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
			}
			
			public void Dispose()
			{
				AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolve;
			}
			
			Assembly AssemblyResolve(object sender, ResolveEventArgs args)
			{
				try {
					IAssembly assembly = compilation.Assemblies
						.FirstOrDefault(asm => asm.FullAssemblyName == args.Name);
					if (assembly != null) {
						return typeResolutionService.LoadAssembly(assembly);
					}
				} catch (Exception ex) {
					LoggingService.Error(ex);
				}
				return null;
			}
		}
	}
}
