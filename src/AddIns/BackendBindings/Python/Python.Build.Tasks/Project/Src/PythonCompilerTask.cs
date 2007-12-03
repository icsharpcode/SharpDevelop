// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using IronPython.Hosting;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ICSharpCode.Python.Build.Tasks
{
	/// <summary>
	/// Python compiler task.
	/// </summary>
	public class PythonCompilerTask : Task
	{
		IPythonCompiler compiler;
		ITaskItem[] sources;
		ITaskItem[] references;
		ITaskItem[] resources;
		string targetType;
		string mainFile;
		string outputAssembly;
		bool emitDebugInformation;
		
		public PythonCompilerTask() 
			: this(new PythonCompiler())
		{
		}
		
		public PythonCompilerTask(IPythonCompiler compiler)
		{
			this.compiler = compiler;
		}
		
		/// <summary>
		/// Gets or sets the source files that will be compiled.
		/// </summary>
		public ITaskItem[] Sources {
			get { return sources; }
			set { sources = value; }
		}		
		
		/// <summary>
		/// Gets or sets the resources to be compiled.
		/// </summary>
		public ITaskItem[] Resources {
			get { return resources; }
			set { resources = value; }
		}

		/// <summary>
		/// Gets or sets the output assembly type.
		/// </summary>
		public string TargetType {
			get { return targetType; }
			set { targetType = value; }
		}
		
		/// <summary>
		/// Gets or sets the file that contains the main entry point.
		/// </summary>
		public string MainFile { 
			get { return mainFile; }
			set { mainFile = value; }
		}
		
		/// <summary>
		/// Gets or sets the output assembly filename.
		/// </summary>
		public string OutputAssembly { 
			get { return outputAssembly; }
			set { outputAssembly = value; }
		}
		
		/// <summary>
		/// Gets or sets whether the compiler should include debug
		/// information in the created assembly.
		/// </summary>
		public bool EmitDebugInformation { 
			get { return emitDebugInformation; }
			set { emitDebugInformation = value; }
		}
		
		/// <summary>
		/// Gets or sets the assembly references.
		/// </summary>
		public ITaskItem[] References {
			get { return references; }
			set { references = value; }
		}		
		
		/// <summary>
		/// Executes the compiler.
		/// </summary>
		public override bool Execute()
		{
			using (compiler) {
				// Set what sort of assembly we are generating
				// (e.g. WinExe, Exe or Dll)
				compiler.TargetKind = GetPEFileKind(targetType);

				compiler.SourceFiles = GetFiles(sources);
				compiler.ReferencedAssemblies = GetFiles(references);
				compiler.ResourceFiles = GetResourceFiles(resources);
				compiler.MainFile = mainFile;
				compiler.OutputAssembly = outputAssembly;
				compiler.IncludeDebugInformation = emitDebugInformation;
				
				// Compile the code.
				compiler.Compile();
			}
			return true;
		}
		
		/// <summary>
		/// Maps from the target type string to the PEFileKind 
		/// needed by the compiler.
		/// </summary>
		static PEFileKinds GetPEFileKind(string targetType)
		{
			if (targetType != null) {
				switch (targetType.ToLowerInvariant()) {
					case "winexe":
						return PEFileKinds.WindowApplication;
					case "library":
						return PEFileKinds.Dll;
				}
			}
			return PEFileKinds.ConsoleApplication;
		}
		
		/// <summary>
		/// Converts from an array of ITaskItems to a list of
		/// strings, each containing the ITaskItem filename.
		/// </summary>
		IList<string> GetFiles(ITaskItem[] taskItems)
		{			
			List<string> files = new List<string>();
			if (taskItems != null) {
				foreach (ITaskItem item in taskItems) {
					files.Add(item.ItemSpec);
				}
			}
			return files;
		}
		
		/// <summary>
		/// Converts from an array of ITaskItems to a list of
		/// ResourceFiles.
		/// </summary>
		/// <remarks>
		/// The resource name is the filename without any preceding
		/// path information.
		/// </remarks>
		IList<ResourceFile> GetResourceFiles(ITaskItem[] taskItems)
		{
			List<ResourceFile> files = new List<ResourceFile>();
			if (taskItems != null) {
				foreach (ITaskItem item in taskItems) {
					string resourceName = Path.GetFileName(item.ItemSpec);
					ResourceFile resourceFile = new ResourceFile(resourceName, item.ItemSpec);
					files.Add(resourceFile);
				}
			}
			return files;
		}		
	}
}
