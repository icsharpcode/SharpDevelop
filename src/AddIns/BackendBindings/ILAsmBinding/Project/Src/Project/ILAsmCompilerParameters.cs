// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.ComponentModel;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.ILAsmBinding
{
	public enum CompilationTarget {
		Exe, 
		Dll,
		NetModule
	}
	
	public enum ILAsmCompiler {
		Microsoft,
		Mono
	};
	
	public enum NetRuntime {
		Mono,
		MonoInterpreter,
		MsNet
	};
	
	/// <summary>
	/// This class handles project specific compiler parameters
	/// </summary>
	public class ILAsmCompilerParameters : AbstractProjectConfiguration
	{
		CompilerOptions compilerOptions = new CompilerOptions();
		
		[Browsable(false)]
		public CompilerOptions CurrentCompilerOptions {
			get {
				return compilerOptions;
			}
		}
		
		[LocalizedProperty("Output path",
			               Description = "The path where the assembly is created.")]
		public string OutputPath {
			get {
				return OutputDirectory;
			}
			set {
				OutputDirectory = value;
			}
		}
		
		[LocalizedProperty("Output assembly",
		                   Description = "The assembly name.")]
		public string AssemblyName {
			get {
				return OutputAssembly;
			}
			set {
				OutputAssembly = value;
			}
		}
		
		[DefaultValue(CompilationTarget.Exe)]
		[LocalizedProperty("Compilation Target",
		                   Description = "The compilation target of the source code. (/DLL, /EXE)")]
		public CompilationTarget CompilationTarget {
			get {
				return compilerOptions.compilationTarget;
			}
			set {
				compilerOptions.compilationTarget = value;
			}
		}
		
		[LocalizedProperty("Key File",
		                   Description = "Compile with strong signature.")]
		public string KeyFile {
			get {
				return compilerOptions.keyFile;
			}
			set {
				compilerOptions.keyFile = value;
			}
		}
		
		[DefaultValue(false)]
		[LocalizedProperty("Include debug information",
		                   Description = "Specifies if debug information should be omited. (/DEBUG)")]
		public bool IncludeDebugInformation {
			get {
				return compilerOptions.includeDebugInformation;
			}
			set {
				compilerOptions.includeDebugInformation = value;
			}
		}
		
		[Browsable(false)]
		public string ILAsmCompilerVersion {
			get {
				return compilerOptions.ilasmCompilerVersion;
			}
			set {
				compilerOptions.ilasmCompilerVersion = value;
			}
		}
		
		[Browsable(false)]
		public ILAsmCompiler ILAsmCompiler {
			get {
				return compilerOptions.ilasmCompiler;
			}
			set {
				compilerOptions.ilasmCompiler = value;
			}
		}
		
		[Browsable(false)]
		public NetRuntime NetRuntime {
			get {
				return compilerOptions.netRuntime;
			}
			set {
				compilerOptions.netRuntime = value;
			}
		}
		
		public ILAsmCompilerParameters()
		{
		}
		
		public ILAsmCompilerParameters(string name)
		{
			this.name = name;
		}
		
		[XmlNodeName("CompilerOptions")]
		public class CompilerOptions
		{
			[XmlAttribute("runtime")]
			public NetRuntime netRuntime         = NetRuntime.MsNet;
			
			[XmlAttribute("compiler")]
			public ILAsmCompiler ilasmCompiler = ILAsmCompiler.Microsoft;
			
			[XmlAttribute("compilerversion")]
			public string ilasmCompilerVersion = String.Empty;
			
			[XmlAttribute("keyFile")]
			public string keyFile = String.Empty;
			
			[XmlAttribute("compilationTarget")]
			internal CompilationTarget compilationTarget = CompilationTarget.Exe;
			
			[XmlAttribute("includeDebugInformation")]
			internal bool includeDebugInformation = false;
			
			public string GenerateOptions()
			{
				StringBuilder options = new StringBuilder();
				switch (compilationTarget) {
					case CompilationTarget.Dll:
					case CompilationTarget.NetModule:
						options.Append("/DLL ");
						break;
					case CompilationTarget.Exe:
						options.Append("/EXE ");
						break;
					default:
						throw new System.NotSupportedException("Unsupported compilation target : " + compilationTarget);
				}
				
				if (includeDebugInformation) {
					options.Append("/DEBUG ");
				}
				
				if (keyFile != null && keyFile.Length > 0) {
					options.Append("/KEY=\"" + keyFile + "\" ");
				}
			
				
				return options.ToString();
			}
		}
	}
}
