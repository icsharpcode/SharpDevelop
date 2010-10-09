// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using IronPython.Hosting;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Scripting;

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
		string platform;
		
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
		/// Gets or sets the platform that will be targeted by the compiler (e.g. x86).
		/// </summary>
		public string Platform {
			get { return platform; }
			set { platform = value; }
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

				compiler.ExecutableKind = GetExecutableKind(platform);
				compiler.Machine = GetMachine(platform);
				
				compiler.SourceFiles = GetFiles(sources, false);
				compiler.ReferencedAssemblies = GetFiles(references, true);
				compiler.ResourceFiles = GetResourceFiles(resources);
				compiler.MainFile = mainFile;
				compiler.OutputAssembly = outputAssembly;
				compiler.IncludeDebugInformation = emitDebugInformation;
				
				// Compile the code.
				try {
					compiler.Compile();
					return true;
				} catch (SyntaxErrorException ex) {
					LogSyntaxError(ex);
				} catch (IOException ex) {
					LogError(ex.Message);
				} catch (PythonCompilerException ex) {
					LogError(ex.Message);
				}
			}
			return false;
		}

		/// <summary>
		/// Gets the current folder where this task is being executed from.
		/// </summary>
		protected virtual string GetCurrentFolder()
		{
			return Directory.GetCurrentDirectory();
		}
		
		/// <summary>
		/// Logs any error message that occurs during compilation. Default implementation
		/// is to use the MSBuild task's base.Log.LogError(...)
		/// </summary>
		protected virtual void LogError(string message, string errorCode, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber)
		{
			Log.LogError(null, errorCode, null, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message);
		}
		
		void LogError(string message)
		{
			LogError(message, null, null, 0, 0, 0, 0);
		}
		
		void LogSyntaxError(SyntaxErrorException ex)
		{
			string fileName = GetFileName(ex, sources);
			LogError(ex.Message, ex.ErrorCode.ToString(), fileName, ex.Line, ex.Column, ex.RawSpan.End.Line, ex.RawSpan.End.Column);
		}
		
		/// <summary>
		/// Matches the syntax exception SourcePath against filenames being compiled. The 
		/// syntax exception only contains the file name without its path and without its file extension.
		/// </summary>
		string GetFileName(SyntaxErrorException ex, ITaskItem[] sources)
		{
			if (ex.SourcePath == null) {
				return null;
			}
			
			string sourcePath = ex.SourcePath.Replace('\\', '.');
			foreach (ITaskItem item in sources) {
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item.ItemSpec);
				if (fileNameWithoutExtension == sourcePath) {
					return item.ItemSpec;
				}
			}
			string fileName = sourcePath + ".py";
			return Path.Combine(GetCurrentFolder(), fileName);
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
		IList<string> GetFiles(ITaskItem[] taskItems, bool fullPath)
		{			
			List<string> files = new List<string>();
			if (taskItems != null) {
				foreach (ITaskItem item in taskItems) {
					string fileName = item.ItemSpec;
					if (fullPath) {
						fileName = GetFullPath(item.ItemSpec);
					}
					files.Add(fileName);
				}
			}
			return files;
		}
		
		/// <summary>
		/// Converts the string into a PortableExecutableKinds enum.
		/// </summary>
		PortableExecutableKinds GetExecutableKind(string platform)
		{
			switch (platform) {
				case "x86":
					return PortableExecutableKinds.ILOnly | PortableExecutableKinds.Required32Bit;
				case "Itanium":
				case "x64":
					return PortableExecutableKinds.ILOnly | PortableExecutableKinds.PE32Plus;
			}
			return PortableExecutableKinds.ILOnly;
		}
		
		/// <summary>
		/// Gets the machine associated with a PortalExecutableKind.
		/// </summary>
		ImageFileMachine GetMachine(string platform)
		{
			switch (platform) {
				case "Itanium":
					return ImageFileMachine.IA64;
				case "x64":
					return ImageFileMachine.AMD64;
			}
			return ImageFileMachine.I386;
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
					string resourceFileName = GetFullPath(item.ItemSpec);
					string resourceName = GetResourceName(item);
					ResourceFile resourceFile = new ResourceFile(resourceName, resourceFileName);
					files.Add(resourceFile);
				}
			}
			return files;
		}
		
		string GetResourceName(ITaskItem item)
		{
			string logicalResourceName = item.GetMetadata("LogicalName");
			if (!String.IsNullOrEmpty(logicalResourceName)) {
				return logicalResourceName;
			}
			return Path.GetFileName(item.ItemSpec);
		}
	
		/// <summary>
		/// Takes a relative path to a file and turns it into the full path using the current folder
		/// as the base directory.
		/// </summary>
		string GetFullPath(string fileName)
		{
			if (!Path.IsPathRooted(fileName)) {
				return Path.GetFullPath(Path.Combine(GetCurrentFolder(), fileName));
			}
			return fileName;
		}
	}
}
