// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.Diagnostics;
using System.ComponentModel;

using ICSharpCode.SharpDevelop.Internal.Project;

namespace VBBinding {
	
	public enum CompileTarget
	{
		Exe,
		WinExe,
		Library,
		Module
	};
	
	/// <summary>
	/// This class handles project specific compiler parameters
	/// </summary>
	public class VBCompilerParameters : AbstractProjectConfiguration
	{
		[XmlNodeName("CodeGeneration")]
		class CodeGeneration 
		{
			[XmlAttribute("compilerversion")]
			public string vbCompilerVersion = String.Empty;
			
			[XmlAttribute("includedebuginformation")]
			public bool debugmode = true;
			
			[XmlAttribute("optimize")]
			public bool optimize = true;
			
			[XmlAttribute("generateoverflowchecks")]
			public bool generateOverflowChecks = true;
			
			[XmlAttribute("rootnamespace")]
			public string rootnamespace = String.Empty;
			
			[XmlAttribute("mainclass")]
			public string mainclass = null;
			
			[XmlAttribute("target")]
			public CompileTarget  compiletarget = CompileTarget.Exe;
			
			[XmlAttribute("definesymbols")]
			public string definesymbols = String.Empty;
			
			[XmlAttribute("optionexplicit")]
			public bool optionExplicit = true;
			
			[XmlAttribute("optionstrict")]
			public bool optionStrict = false;
			
			[ConvertToRelativePathAttribute()]
			[XmlAttribute("win32Icon")]
			public string win32Icon = String.Empty;
			
			[XmlAttribute("imports")]
			public string imports = String.Empty;
		}
		
		[XmlNodeName("Execution")]
		class Execution
		{
			[XmlAttribute("consolepause")]
			public bool pauseconsoleoutput = true;
			
			[XmlAttribute("commandlineparameters")]
			public string commandLineParameters = String.Empty;
			
		}
		
		[XmlNodeName("VBDOC")]
		class VBDOC
		{
			[XmlAttribute("outputfile")]
			[ConvertToRelativePathAttribute()]
			public string outputfile = String.Empty;
			
			[XmlAttribute("filestoparse")]
			public string filestoparse = String.Empty;
			
			[XmlAttribute("commentprefix")]
			public string commentprefix = "'";
		}
		
		CodeGeneration codeGeneration = new CodeGeneration();
		VBDOC		   vbdoc		  = new VBDOC();
		Execution      execution      = new Execution();
		
		[Browsable(false)]
		public string VBCompilerVersion
		{
			get {
				return codeGeneration.vbCompilerVersion;
			}
			set {
				codeGeneration.vbCompilerVersion = value;
			}
		}
		
		public string CommandLineParameters
		{
			get {
				return execution.commandLineParameters;
			}
			set {
				execution.commandLineParameters = value;
			}
		}
		public bool GenerateOverflowChecks
		{
			get {
				return codeGeneration.generateOverflowChecks;
			}
			set {
				codeGeneration.generateOverflowChecks = value;
			}
		}
		
		public string Imports
		{
			get {
				return codeGeneration.imports;
			}
			set {
				codeGeneration.imports = value;
			}
		}
		
		public string Win32Icon
		{
			get {
				return codeGeneration.win32Icon;
			}
			set {
				codeGeneration.win32Icon = value;
			}
		}
		
		public string RootNamespace
		{
			get {
				return codeGeneration.rootnamespace;
			}
			set {
				codeGeneration.rootnamespace = value;
			}
		}
		
		public string DefineSymbols
		{
			get {
				return codeGeneration.definesymbols;
			}
			set {
				codeGeneration.definesymbols = value;
			}
		}
		
		public bool PauseConsoleOutput
		{
			get {
				return execution.pauseconsoleoutput;
			}
			set {
				execution.pauseconsoleoutput = value;
			}
		}
		
		public bool Debugmode
		{
			get {
				return codeGeneration.debugmode;
			}
			set {
				codeGeneration.debugmode = value;
			}
		}
		
		public bool Optimize
		{
			get {
				return codeGeneration.optimize;
			}
			set {
				codeGeneration.optimize = value;
			}
		}
		
		public string MainClass
		{
			get {
				return codeGeneration.mainclass;
			}
			set {
				codeGeneration.mainclass = value;
			}
		}
		
		public CompileTarget CompileTarget
		{
			get {
				return codeGeneration.compiletarget;
			}
			set {
				codeGeneration.compiletarget = value;
			}
		}
		
		public bool OptionExplicit
		{
			get {
				return codeGeneration.optionExplicit;
			}
			set {
				codeGeneration.optionExplicit = value;
			}
		}
		
		public bool OptionStrict
		{
			get {
				return codeGeneration.optionStrict;
			}
			set {
				codeGeneration.optionStrict = value;
			}
		}
		
		public string VBDOCOutputFile
		{
			get {
				return vbdoc.outputfile;
			}
			set {
				vbdoc.outputfile = value;
			}
		}
		
		public string[] VBDOCFiles
		{
			get {
				return vbdoc.filestoparse.Split(';');
			}
			set {
				vbdoc.filestoparse = System.String.Join(";", value);
			}
		}
		
		public string VBDOCCommentPrefix
		{
			get {
				return vbdoc.commentprefix;
			}
			set {
				vbdoc.commentprefix = value;
			}
		}
		
		public VBCompilerParameters()
		{
		}
		
		public VBCompilerParameters(string name)
		{
			this.name = name;
		}
	}
}
