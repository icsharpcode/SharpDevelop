// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Diagnostics;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace CPPBinding
{
	#region OPTIMIZATION
	public enum OptimizeLevel {
		Deactivated,      // /Od
		MinimizeSize,     // /O1
		MaximizeSpeed,    // /O2
		CompleteOptimize  // /Ox
	}
	
	public enum FloatingPointConsistency {
		Standard,
		Enhanced  // /Op
	}
	
	public enum InlineFunctionExpansion 
	{
		Standard,  // /Ob0
		Manual,    // /Ob1
		Automatic  // /Ob2
	}
	
	public enum SizeOrSpeedOptimization
	{
		Neither,
		Size,
		Speed
	}

	#endregion
	
	#region CODE GENERATION
	public enum Architecture {
		Mixed,      
		i386,       // /G3
		i486,       // /G4
		Pentium,    // /G5
		PentiumPRO, // /G6
		Pentium4    // /G7
	}
	
	public enum CallingConvention {
		__cdecl,    // default (/Gd)
		__fastcall, // /Gr
		__stdcall   // /Gz
	}

	public enum EnhancedInstructionSet {
		NotSpecified,
		SSE,          // /arch:SSE
		SSE2          // /arch:SSE2
	}
	
	public enum RunTimeObservation {
		Standard,
		Stack,                  // /RTCs
		UninitializedVariables, // /RTCu
		Both                    // /RTCs /RTCu
	}
	
	public enum ExceptionHandling {
		None,
		SynchronousExceptionHandling, // /GX
		AsynchronousExceptionHandling // /EHa
	}
	
	public enum StringPooling {
		Disabled,
		Enabled,
		EnabledReadOnlyPooling
	}

	#endregion

	#region DEBUGGING
	public enum DebugInformationFormat {
		Deactivated,
		C7,								   // /Z7
		OnlyLineNumbers,				   // /Zd
		ProgramDatabase,				   // /Zi
		ProgramDatabaseWithEditAndContinue // /ZI
	}
	
	#endregion

	#region PREPROCESSOR
	public enum PreProcessorRun {
		No,
		WithLineNumbers,    // /E
		WithoutLineNumbers  // /EP
	}
	#endregion
	
	#region LANGUAGE
	public enum StructMemberAlignment {
		Standard,
		Byte1,    // /Zp1
		Byte2,    // /Zp2
		Byte4,    // /Zp4
		Byte8,    // /Zp8
		Byte16,   // /Zp16
	}

	public enum LanguageExtensions {
		Enable, // /Ze
		Disable // /Za
	}
	
	#endregion
	
	#region LINKING
	public enum RunTimeLibrary {
		MultiThreaded,         // /MT
		MultiThreadedDebug,    // /MTd
		MultiThreadedDLL,      // /MD
		MultiThreadedDebugDLL, // /MDd
		SingleThreaded,        // /ML
		SingleThreadedDebug    // /MLd
	}
	
	public enum ConfigurationType {
		Exe, 
		Dll
	}
	
	#endregion

	#region PRECOMPILED HEADER	
	public enum PreCompiledHeader {
		DontUsePre,          // /Y-
		Create,              // /Yc
		CreateAutomatically, // /YX
		Use                  // /Yu
	}
	#endregion

	#region MISCELLANEOUS
	public enum CompileType {
		Standard,
		C,        // /TC
		CPP       // /TP
	}
	
	#endregion
	
	
	
	public enum AssemblyOutput {
		NoList,
		ListAssembly,
		ListAssemblyWithSource,
		ListAssemblyWithCode,
		ListAssemblyWithCodeAndSource,
	}
	
	public enum ShowLinkerStatus {
		Unselected,
		ShowAll,
		ShowSome
	}
	public enum IncrementalLinking {
		Standard,
		Yes,
		No
	}
	
	public enum DebuggableAssembly {
		DontEmitDebuggable,
		EnableDebugToRuntimeDisableOptimization,
		DisableDebugToRuntimEnableOptimization
	}
	
	public enum LinkerSubSystem {
		Unselected,
		Console,
		Windows
	}
	public enum ActivateBigAddresses {
		Standard,
		NoSupport,
		Supported
	}
	public enum TerminalServer {
		Standard,
		NotBound,
		Bound
	}
	
	/// <summary>
	/// This class handles project specific compiler parameters
	/// </summary>
	public class CPPCompilerParameters : AbstractProjectConfiguration
	{

		private static bool IsNotEmpty(string arg) 
		{
			return arg != null && arg.Length > 0;
		}
		
		private static void AppendOption(StringBuilder sb, String opt, String val) 
		{
			AppendOption(sb, opt, val, true);
		}
		
		private static void AppendOption(StringBuilder sb, String opt, String val, bool quote) 
		{
			
			sb.Append(opt);
			if (quote) 
				sb.Append('"');
			sb.Append(val);
			if (quote) 
				sb.Append('"');
			sb.Append("\n");
		}
		
		private static void AppendList(StringBuilder sb, String opt, String values)
		{
			AppendList(sb, opt, values, true);
		}
	
		private static void AppendList(StringBuilder sb, String opt, String values, bool quote)
		{
			foreach (string val in values.Split(';'))
			{
				AppendOption(sb, opt, val, quote);
			}
		}
		
		#region Misc Options
		[XmlNodeName("MiscOptions")]
		public class MiscCPPOptions 
		{
			[ConvertToRelativePath()]
			[XmlAttribute("OutputDirectory")]
			public string outputDirectory       = "";
			
			[ConvertToRelativePath()]
			[XmlAttribute("IntermediateDirectory")]
			public string intermediateDirectory = "";
			
			[XmlAttribute("ConfigurationType")]
			public ConfigurationType configurationType   = ConfigurationType.Exe;
			
			[XmlAttribute("UseManagedExtensions")]
			public bool useManagedExtensions = true;
			
			[XmlAttribute("additionalCompilerOptions")]
			public string additionalCompilerOptions = "";
		}
		#endregion
	
		#region General Options
		[XmlNodeName("GeneralCPPOptions")]
		public class GeneralCPPOptions
		{
			[XmlAttribute("additionalIncludeDirectories")]
			public string additionalIncludeDirectories = "";
			
			[XmlAttribute("additionalAssemblySearchPaths")]
			public string additionalAssemblySearchPaths = "";
			
			[XmlAttribute("debugInformationFormat")]
			public DebugInformationFormat debugInformationFormat = DebugInformationFormat.Deactivated;
			
			[XmlAttribute("noStartLogo")]
			public bool noStartLogo = true;
			
			[XmlAttribute("warningLevel")]
			public int warningLevel = 4;
			
			[XmlAttribute("search64BitPortabilityProblems")]
			public bool search64BitPortabilityProblems = false;
			
//			[XmlAttribute("treatWarningsAsErrors")]
//			public bool treatWarningsAsErrors = false;
			
			[DefaultValue("")]
			[LocalizedProperty("Additional include paths",
			                   Description = "Specifies one or more semi-colon delimited additonal paths to search for includes. (/I[path])")]
			public string AdditionalIncludeDirectories {
				get {
					return additionalIncludeDirectories;
				}
				set {
					additionalIncludeDirectories = value;
				}
			}
			
			[DefaultValue("")]
			[LocalizedProperty("Additional assembly search",
			                   Description = "Specifies one or more semi-colon delimited additonal paths to search for #using assemblies. (/AI[path])")]
			public string AdditionalAssemblySearchPaths {
				get {
					return additionalAssemblySearchPaths;
				}
				set {
					additionalAssemblySearchPaths = value;
				}
			}
			
			[LocalizedProperty("Debug information format",
			                   Description = "(/Z7, /Zd, /Zi. /ZI)")]
			public DebugInformationFormat DebugInformationFormat {
				get {
					return debugInformationFormat;
				}
				set {
					debugInformationFormat = value;
				}
			}
			
			[DefaultValue(true)]
			[LocalizedProperty("Surpress Startup Logo",
			                   Description = "Surpress the display of the startup logo and information messages. (/nologo)")]
			public bool NoStartLogo {
				get {
					return noStartLogo;
				}
				set {
					noStartLogo = value;
				}
			}
			
			[DefaultValue(4)]
			[LocalizedProperty("Warning level",
			                   Description = "(/W0 - /W4)")]
			public int WarningLevel {
				get {
					return warningLevel;
				}
				set {
					warningLevel = value;
				}
			}
			
			[DefaultValue(false)]
			[LocalizedProperty("Search for 64-Bit portability problems",
			                   Description = "(/Wp64)")]
			public bool Search64BitPortabilityProblems {
				get {
					return search64BitPortabilityProblems;
				}
				set {
					search64BitPortabilityProblems = value;
				}
			}
			
//			[DefaultValue(false)]
//			[LocalizedProperty("Treat warnings as errors",
//			                   Description = "(/WX)")]
//			public bool TreatWarningsAsErrors {
//				get {
//					return treatWarningsAsErrors;
//				}
//				set {
//					treatWarningsAsErrors = value;
//				}
//			}
			
			public string GetCommandLineParameters()
			{
				StringBuilder result = new StringBuilder();
				if (IsNotEmpty(AdditionalIncludeDirectories)) {
					AppendList(result, "/I", AdditionalIncludeDirectories, true);
				}
				if (IsNotEmpty(AdditionalAssemblySearchPaths)) {
					AppendList(result, "/AI", AdditionalAssemblySearchPaths, true);
				}
				switch (DebugInformationFormat) {
					case DebugInformationFormat.Deactivated:
						break;
					case DebugInformationFormat.C7:
						result.Append("/Z7\n");
						break;
					case DebugInformationFormat.OnlyLineNumbers:
						result.Append("/Zd\n");
						break;
					case DebugInformationFormat.ProgramDatabase:
						result.Append("/Zi\n");
						break;
					case DebugInformationFormat.ProgramDatabaseWithEditAndContinue:
						result.Append("/ZI\n");
						break;
					
				}
				if (NoStartLogo) {
					result.Append("/nologo\n");
				}
				AppendOption(result, "/W", WarningLevel.ToString());
				if (Search64BitPortabilityProblems) {
					result.Append("/Wp64\n");
				}
//				if (TreatWarningsAsErrors) {
//					result.Append("/WX\n");
//				}
				
				return result.ToString();
			}
		}
		#endregion
		
		#region Optimize Options
		[XmlNodeName("OptimizeCPPOptions")]
		public class OptimizeCPPOptions
		{
			[XmlAttribute("optimizeLevel")]
			public OptimizeLevel optimizeLevel = OptimizeLevel.Deactivated;
			
			[XmlAttribute("useGlobalOptimize")]
			public bool useGlobalOptimize = false;
			
			[XmlAttribute("inlineFunctionExtension")]
			public InlineFunctionExpansion inlineFunctionExpansion = InlineFunctionExpansion.Standard;
			
			[XmlAttribute("activateSysInternalFunctions")]
			public bool activateSysInternalFunctions = false;
			
			[XmlAttribute("floatingPointConsistency")]
			public FloatingPointConsistency floatingPointConsistency = FloatingPointConsistency.Standard;
			
			[XmlAttribute("sizeOrSpeedOptimization")]
			public SizeOrSpeedOptimization sizeOrSpeedOptimization = SizeOrSpeedOptimization.Neither;
			
			[XmlAttribute("surpressFramePointer")]
			public bool surpressFramePointer = false;
			
			[XmlAttribute("enableFiberSaveOptimize")]
			public bool enableFiberSaveOptimize = false;
			
			[XmlAttribute("architecture")]
			public Architecture architecture = Architecture.Mixed;
			
			[XmlAttribute("optimizeForWindowsExecutable")]
			public bool optimizeForWindowsExecutable = false;
			
			[LocalizedProperty("Optimize Level",
			                   Description = "/Od,/O1,/O2,/Ox")]
			public OptimizeLevel OptimizeLevel {
				get {
					return optimizeLevel;
				}
				set {
					optimizeLevel = value;
				}
			}
			
			[DefaultValue(false)]
			[LocalizedProperty("Use Global Optimization",
			                   Description = "/Og")]
			public bool UseGlobalOptimize {
				get {
					return useGlobalOptimize;
				}
				set {
					useGlobalOptimize = value;
				}
			}
			[LocalizedProperty("Inline Functions Expansion",
			                   Description = "/Ob1,/Ob2")]
			public InlineFunctionExpansion InlineFunctionExpansion {
				get {
					return inlineFunctionExpansion;
				}
				set {
					inlineFunctionExpansion = value;
				}
			}
//			[DefaultValue(false)]
//			[LocalizedProperty("",
//			                   Description = "")]
//			public bool ActivateSysInternalFunctions {
//				get {
//					return activateSysInternalFunctions;
//				}
//				set {
//					activateSysInternalFunctions = value;
//				}
//			}
			[LocalizedProperty("Floating Point Consistency",
			                   Description = "/Op")]
			public FloatingPointConsistency FloatingPointConsistency {
				get {
					return floatingPointConsistency;
				}
				set {
					floatingPointConsistency = value;
				}
			}
			[LocalizedProperty("Size Or Speed Optimization",
			                   Description = "/Ot,/Os")]
			public SizeOrSpeedOptimization SizeOrSpeedOptimization {
				get {
					return sizeOrSpeedOptimization;
				}
				set {
					sizeOrSpeedOptimization = value;
				}
			}
			[DefaultValue(false)]
			[LocalizedProperty("Suppress Frame Pointer",
			                   Description = "/Oy")]
			public bool SurpressFramePointer {
				get {
					return surpressFramePointer;
				}
				set {
					surpressFramePointer = value;
				}
			}
			[DefaultValue(false)]
			[LocalizedProperty("Fiber Safety Support",
			                   Description = "/GT")]
			public bool EnableFiberSaveOptimize {
				get {
					return enableFiberSaveOptimize;
				}
				set {
					enableFiberSaveOptimize = value;
				}
			}
			[LocalizedProperty("Optimize for Processor",
			                   Description = "/G3,/G4,/G5,/G6,/G7")]
			public Architecture Architecture {
				get {
					return architecture;
				}
				set {
					architecture = value;
				}
			}
			[DefaultValue(false)]
			[LocalizedProperty("Optimizes for Windows Application",
			                   Description = "/GA")]
			public bool OptimizeForWindowsExecutable {
				get {
					return optimizeForWindowsExecutable;
				}
				set {
					optimizeForWindowsExecutable = value;
				}
			}
			public string GetCommandLineParameters()
			{
				StringBuilder result = new StringBuilder();
				switch (OptimizeLevel) {
					case OptimizeLevel.CompleteOptimize:
						result.Append("/Ox\n");
						break;
					case OptimizeLevel.Deactivated:
//						result.Append("/Od\n");
						break;
					case OptimizeLevel.MaximizeSpeed:
						result.Append("/O2\n");
						break;
					case OptimizeLevel.MinimizeSize:
						result.Append("/O1\n");
						break;
				}
				switch (FloatingPointConsistency) {
					case FloatingPointConsistency.Enhanced:
						result.Append("/Op\n");
						break;
				}
				switch (architecture) {
					case Architecture.Mixed:
						break;
					case Architecture.i386:
						result.Append("/G3\n");
						break;
					case Architecture.i486:
						result.Append("/G4\n");
						break;
					case Architecture.Pentium:
						result.Append("/G5\n");
						break;
					case Architecture.PentiumPRO:
						result.Append("/G6\n");
						break;
					case Architecture.Pentium4:
						result.Append("/G7\n");
						break;
				}
				if (UseGlobalOptimize) {
					result.Append("/Og\n");
				}
//				if (activateSysInternalFunctions) {
//					result.Append("/\n");
//				}
				if (surpressFramePointer) {
					result.Append("/Oy\n");
				}
				if (enableFiberSaveOptimize) {
					result.Append("/GT\n");
				}
				if (optimizeForWindowsExecutable) {
					result.Append("/GA\n");
				}
				switch (InlineFunctionExpansion) {
					case InlineFunctionExpansion.Automatic:
						result.Append("/Ob2\n");
						break;
					case InlineFunctionExpansion.Manual:
						result.Append("/Ob1\n");
						break;
					case InlineFunctionExpansion.Standard:
						break;
				}
				switch (SizeOrSpeedOptimization) {
					case SizeOrSpeedOptimization.Neither:
						break;
					case SizeOrSpeedOptimization.Size:
						result.Append("/Os\n");
						break;
					case SizeOrSpeedOptimization.Speed:
						result.Append("/Ot\n");
						break;
				}
				return result.ToString();
			}
		}
		#endregion
		
		#region Preprocessor Options
		[XmlNodeName("PreProcessorCPPOptions")]
		public class PreProcessorCPPOptions
		{
			[XmlAttribute("additionalDirectives")]
			public string additionalDirectives = "";
			
			[XmlAttribute("ignoreStandardIncludePath")]
			public bool ignoreStandardIncludePath = false;
			
			[XmlAttribute("preProcessorRun")]
			public PreProcessorRun preProcessorRun = PreProcessorRun.No;
			
			[XmlAttribute("keepComments")]
			public bool keepComments = true;
			
			[LocalizedProperty("Pre Processor Directives",
			                   Description = "Specifies additional pre processor directives. (/D[macro])")]
			public string AdditionalDirectives {
				get {
					return additionalDirectives;
				}
				set {
					additionalDirectives = value;
				}
			}
			
			[LocalizedProperty("Ignore standard search paths",
			                   Description = "If true, standard search paths are ignored. (/X)")]
			public bool IgnoreStandardIncludePath {
				get {
					return ignoreStandardIncludePath;
				}
				set {
					ignoreStandardIncludePath = value;
				}
			}
			
			[LocalizedProperty("Pre Processor Run",
			                   Description = "Specifies the pre processor options for this configuration. (/E, /P, /EP)")]
			public PreProcessorRun PreProcessorRun {
				get {
					return preProcessorRun;
				}
				set {
					preProcessorRun = value;
				}
			}
			
			[LocalizedProperty("Keep comments",
			                   Description = "Specifies if comments should be removed from the source code. (/C)")]
			public bool KeepComments {
				get {
					return keepComments;
				}
				set {
					keepComments = value;
				}
			}
			public string GetCommandLineParameters()
			{
				StringBuilder result = new StringBuilder();
				if (IsNotEmpty(additionalDirectives)) {
					AppendList(result, "/D", additionalDirectives);
				}
				if (ignoreStandardIncludePath) {
					result.Append("/X\n");
				}
				switch (preProcessorRun) {
					case PreProcessorRun.No:
						break;
					case PreProcessorRun.WithLineNumbers:
						result.Append("/P\n");
						if (keepComments) {
							result.Append("/C\n");
						}
						break;
					case PreProcessorRun.WithoutLineNumbers:
						result.Append("/EP\n/P\n");
						if (keepComments) {
							result.Append("/C\n");
						}
						break;
				}
				return result.ToString();
			}
		}
		#endregion
		
		#region Code Generation Options
		[XmlNodeName("CodeGenerationCPPOptions")]
		public class CodeGenerationCPPOptions
		{
			[XmlAttribute("activateStringPooling")]
			public bool activateStringPooling = false;
			
			[XmlAttribute("activateMinimalRecompilation")]
			public bool activateMinimalRecompilation = false;
			
			[XmlAttribute("activateCPPExceptions")]
			public bool activateCPPExceptions = true;
			
			[XmlAttribute("observeSmallTypes")]
			public bool observeSmallTypes = false;
			
			[XmlAttribute("runTimeObservation")]
			public RunTimeObservation runTimeObservation = RunTimeObservation.Standard;
			
			[XmlAttribute("runTimeLibrary")]
			public RunTimeLibrary runTimeLibrary = RunTimeLibrary.MultiThreaded;
			
			[XmlAttribute("structMemberAlignment")]
			public StructMemberAlignment structMemberAlignment = StructMemberAlignment.Standard;
			
			[XmlAttribute("bufferOverflowCheck")]
			public bool bufferOverflowCheck = false;
			
			[XmlAttribute("functionLevelLinking")]
			public bool functionLevelLinking = false;
			
			[XmlAttribute("enhancedInstructionSet")]
			public EnhancedInstructionSet enhancedInstructionSet = EnhancedInstructionSet.NotSpecified;
			
			[LocalizedProperty("Activate String Pooling",
			                   Description = "(/GF)")]
			public bool ActivateStringPooling {
				get {
					return activateStringPooling;
				}
				set {
					activateStringPooling = value;
				}
			}
			
			[LocalizedProperty("Activate minimal recompilation",
			                   Description = "(/Gm)")]
			public bool ActivateMinimalRecompilation {
				get {
					return activateMinimalRecompilation;
				}
				set {
					activateMinimalRecompilation = value;
				}
			}
			
			[LocalizedProperty("Activate C++ exceptions",
			                   Description = "(/EHsc)")]
			public bool ActivateCPPExceptions {
				get {
					return activateCPPExceptions;
				}
				set {
					activateCPPExceptions = value;
				}
			}
			
			[LocalizedProperty("Observe small types",
			                   Description = "(/RTCc)")]
			public bool ObserveSmallTypes {
				get {
					return observeSmallTypes;
				}
				set {
					observeSmallTypes = value;
				}
			}
			
			[LocalizedProperty("Full Runtimeobservation",
			                   Description = "(/RTCs, /RTCu, /RTC1)")]
			public RunTimeObservation RunTimeObservation {
				get {
					return runTimeObservation;
				}
				set {
					runTimeObservation = value;
				}
			}
			
			[LocalizedProperty("Runtime library",
			                   Description = "(/MT, /MTd, /MD, /MDd, /ML, /MLd)")]
			public RunTimeLibrary RunTimeLibrary {
				get {
					return runTimeLibrary;
				}
				set {
					runTimeLibrary = value;
				}
			}
			
			[LocalizedProperty("Struct member alignment",
			                   Description = "1, 2, 4, 8 or 16 byte. (/Zp[number])")]
			public StructMemberAlignment StructMemberAlignment {
				get {
					return structMemberAlignment;
				}
				set {
					structMemberAlignment = value;
				}
			}
			
			[LocalizedProperty("Buffer overwflow check",
			                   Description = "(/GS)")]
			public bool BufferOverflowCheck {
				get {
					return bufferOverflowCheck;
				}
				set {
					bufferOverflowCheck = value;
				}
			}
			
			[LocalizedProperty("Activate function level linking",
			                   Description = "(/Gy)")]
			public bool FunctionLevelLinking {
				get {
					return functionLevelLinking;
				}
				set {
					functionLevelLinking = value;
				}
			}
			
			[LocalizedProperty("Activate enhanced instruction set",
			                   Description = "(/arch:SSE, /arch:SSE2)")]
			public EnhancedInstructionSet EnhancedInstructionSet {
				get {
					return enhancedInstructionSet;
				}
				set {
					enhancedInstructionSet = value;
				}
			}
			
			public string GetCommandLineParameters()
			{
				StringBuilder result = new StringBuilder();
				if (activateStringPooling) {
					result.Append("/GF\n");
				}
				if (activateMinimalRecompilation) {
					result.Append("/Gm\n");
				}
				if (activateCPPExceptions) {
					result.Append("/EHsc\n");
				}
			
				if (observeSmallTypes) {
					result.Append("/RTCc\n");
				}
				switch (runTimeObservation) {
					case RunTimeObservation.Both:
						result.Append("/RTCsu\n");
						break;
					case RunTimeObservation.Stack:
						result.Append("/RTCs\n");
						break;
					case RunTimeObservation.UninitializedVariables:
						result.Append("/RTCu\n");
						break;
				}
				switch (runTimeLibrary) {
					case RunTimeLibrary.MultiThreaded:
						result.Append("/MT\n");
						break;
					case RunTimeLibrary.MultiThreadedDebug:
						result.Append("/MTd\n");
						break;
					case RunTimeLibrary.MultiThreadedDLL:
						result.Append("/MD\n");
						break;
					case RunTimeLibrary.MultiThreadedDebugDLL:
						result.Append("/MDd\n");
						break;
					case RunTimeLibrary.SingleThreaded:
						result.Append("/ML\n");
						break;
					case RunTimeLibrary.SingleThreadedDebug:
						result.Append("/MLd\n");
						break;
				}

				switch (structMemberAlignment) {
					case StructMemberAlignment.Standard:
						break;
					case StructMemberAlignment.Byte1:
						result.Append("/Zp1\n");
						break;
					case StructMemberAlignment.Byte2:
						result.Append("/Zp2\n");
						break;
					case StructMemberAlignment.Byte4:
						result.Append("/Zp4\n");
						break;
					case StructMemberAlignment.Byte8:
						result.Append("/Zp8\n");
						break;
					case StructMemberAlignment.Byte16:
						result.Append("/Zp16\n");
						break;
				}

				if (bufferOverflowCheck) {
					result.Append("/GS\n");
				}
				if (functionLevelLinking) {
					result.Append("/Gy\n");
				}
			
				switch (EnhancedInstructionSet) {
					case EnhancedInstructionSet.NotSpecified:
						break;
					case EnhancedInstructionSet.SSE:
						result.Append("/arch:SSE\n");
						break;
					case EnhancedInstructionSet.SSE2:
						result.Append("/arch:SSE2\n");
						break;
				}
				return result.ToString();
			}
		}
		#endregion
		
		#region Language Options
		[XmlNodeName("LanguageCPPOptions")]
		public class LanguageCPPOptions
		{
			[XmlAttribute("deactivateLanuageExtensions")]
			public bool deactivateLanuageExtensions = false;
			
			[XmlAttribute("standardCharTypeIsUnsigned")]
			public bool standardCharTypeIsUnsigned = true;
			
			[XmlAttribute("wchar_tIsBuiltIn")]
			public bool wchar_tIsBuiltIn = false;
			
			[XmlAttribute("forceForScope")]
			public bool forceForScope = false;
			
			[XmlAttribute("addRuntimeTypeInformation")]
			public bool addRuntimeTypeInformation = true;
			
			public bool DeactivateLanuageExtensions {
				get {
					return deactivateLanuageExtensions;
				}
				set {
					deactivateLanuageExtensions = value;
				}
			}
			public bool StandardCharTypeIsUnsigned {
				get {
					return standardCharTypeIsUnsigned;
				}
				set {
					standardCharTypeIsUnsigned = value;
				}
			}
			public bool Wchar_tIsBuiltIn {
				get {
					return wchar_tIsBuiltIn;
				}
				set {
					wchar_tIsBuiltIn = value;
				}
			}
			public bool ForceForScope {
				get {
					return forceForScope;
				}
				set {
					forceForScope = value;
				}
			}
			public bool AddRuntimeTypeInformation {
				get {
					return addRuntimeTypeInformation;
				}
				set {
					addRuntimeTypeInformation = value;
				}
			}
			public string GetCommandLineParameters()
			{
				StringBuilder result = new StringBuilder();
				
				if (deactivateLanuageExtensions) {
					result.Append("/Za\n");
				}
			
				if (standardCharTypeIsUnsigned) { 
					result.Append("/J\n");
				}
			
				if (wchar_tIsBuiltIn) { 
					result.Append("/Zc:wchar_t\n");
				}
			
				if (forceForScope) {
					result.Append("/Zc:forScope\n");
				}
			
				if (addRuntimeTypeInformation) { 
					result.Append("/GR\n");
				}
				
				return result.ToString();
			}
		}
		#endregion
		
		#region PreCompiler Header Options
		[XmlNodeName("PreCompiledHeaderCPPOptions")]
		public class PreCompiledHeaderCPPOptions
		{
			[XmlAttribute("preCompiledHeader")]
			public PreCompiledHeader preCompiledHeader = PreCompiledHeader.DontUsePre;
			
			[XmlAttribute("headerFile")]
			public string headerFile = "Stdafx.H";
			
			[ConvertToRelativePath()]
			[XmlAttribute("preCompiledHeaderFile")]
			public string preCompiledHeaderFile = "";
			
			public PreCompiledHeader PreCompiledHeader {
				get {
					return preCompiledHeader;
				}
				set {
					preCompiledHeader = value;
				}
			}
			public string HeaderFile {
				get {
					return headerFile;
				}
				set {
					headerFile = value;
				}
			}
			public string PreCompiledHeaderFile 
			{
				get 
				{
					return preCompiledHeaderFile;
				}
				set 
				{
					preCompiledHeaderFile = value;
				}
			}
			
			private void AppendHeaderFile(StringBuilder result) 
			{
				if (IsNotEmpty(headerFile)) {
					result.Append("\"");
					result.Append(headerFile);
					result.Append("\"");
				}
				result.Append("\n");
			}
			
			public string GetCreatePreCompiledHeaderParameter()
			{
				StringBuilder result = new StringBuilder();
				result.Append("/Yc");
				AppendHeaderFile(result);
				
				if (IsNotEmpty(preCompiledHeaderFile)) {
					AppendOption(result, "/Fp", preCompiledHeaderFile);
				}
							
				return result.ToString();
			}
			
			public bool PreCompileHeader {
				get {
					return preCompiledHeader == PreCompiledHeader.Create ||
					       preCompiledHeader == PreCompiledHeader.Use;
				}
			}
			
			public string GetCommandLineParameters()
			{
				StringBuilder result = new StringBuilder();
				switch (preCompiledHeader) {
					case PreCompiledHeader.DontUsePre:
						result.Append("/Y-\n");
						break;
					case PreCompiledHeader.Create:
					case PreCompiledHeader.Use:
						result.Append("/Yu");
						AppendHeaderFile(result);
						break;
					case PreCompiledHeader.CreateAutomatically:
						result.Append("/YX");
						AppendHeaderFile(result);
						break;

//					case PreCompiledHeader.Create:
//						result.Append("/Yc");
//						AppendHeaderFile(result);
//						break;
//					case PreCompiledHeader.Use:
//						result.Append("/Yu");
//						AppendHeaderFile(result);
//						break;
//					case PreCompiledHeader.CreateAutomatically:
//						result.Append("/YX");
//						AppendHeaderFile(result);
//						break;
				}
				
				if (IsNotEmpty(preCompiledHeaderFile)) {
					AppendOption(result, "/Fp", preCompiledHeaderFile);
				}
			
				return result.ToString();
			}
		}
		#endregion
		
		#region Output File Options
		[XmlNodeName("OutputFileCPPOptions")]
		public class OutputFileCPPOptions
		{
			[XmlAttribute("extendSourceWithAttributes")]
			public bool extendSourceWithAttributes = false;
			 
			[XmlAttribute("assemblyOutput")]
			public AssemblyOutput assemblyOutput = AssemblyOutput.NoList;
			
			[XmlAttribute("assemblerListSaveLocation")]
			public string assemblerListSaveLocation = "";
			
			[XmlAttribute("objectName")]
			public string objectName = "";
			
			[XmlAttribute("programDatabaseName")]
			public string programDatabaseName = "";
			
			public bool ExtendSourceWithAttributes {
				get {
					return extendSourceWithAttributes;
				}
				set {
					extendSourceWithAttributes = value;
				}
			}
			public AssemblyOutput AssemblyOutput {
				get {
					return assemblyOutput;
				}
				set {
					assemblyOutput = value;
				}
			}
			public string AssemblerListSaveLocation {
				get {
					return assemblerListSaveLocation;
				}
				set {
					assemblerListSaveLocation = value;
				}
			}
			public string ObjectName {
				get {
					return objectName;
				}
				set {
					objectName = value;
				}
			}
			public string ProgramDatabaseName {
				get {
					return programDatabaseName;
				}
				set {
					programDatabaseName = value;
				}
			}
			public string GetCommandLineParameters()
			{
				StringBuilder result = new StringBuilder();
				
				if (extendSourceWithAttributes) {
					result.Append("/Fx\n");
				}
				
				switch (assemblyOutput) {
					case AssemblyOutput.ListAssembly:
						result.Append("/FA\n");
						break;
					case AssemblyOutput.ListAssemblyWithCode:
						result.Append("/FAc\n");
						break;
					case AssemblyOutput.ListAssemblyWithCodeAndSource:
						result.Append("/FAcs\n");
						break;
					case AssemblyOutput.ListAssemblyWithSource:
						result.Append("/FAs\n");
						break;
					case AssemblyOutput.NoList:
						break;
				}
			
				if (IsNotEmpty(assemblerListSaveLocation))
				{
					AppendOption(result, "/Fa", assemblerListSaveLocation);
				}
			
				if (IsNotEmpty(objectName)) 
				{
					AppendOption(result, "/Fo", objectName);
				}
			
				if (IsNotEmpty(programDatabaseName)) 
				{
					AppendOption(result, "/Fd", programDatabaseName);
				}
				return result.ToString();
			}
		}
		#endregion
		
		#region Information Search Options
		[XmlNodeName("InformationSearchCPPOptions")]
		public class InformationSearchCPPOptions
		{
			[XmlAttribute("activateBrowseInformation")]
			public bool activateBrowseInformation = false;
			
			[XmlAttribute("browseFile")]
			public string  browseFile = "";
			
			public bool ActivateBrowseInformation {
				get {
					return activateBrowseInformation;
				}
				set {
					activateBrowseInformation = value;
				}
			}
			public string BrowseFile {
				get {
					return browseFile;
				}
				set {
					browseFile = value;
				}
			}
			public string GetCommandLineParameters()
			{
				StringBuilder result = new StringBuilder();
				if (activateBrowseInformation) {
					result.Append("/FR");
					if (IsNotEmpty(browseFile)) 
					{
						result.Append(browseFile);
					}
					result.Append("\n");
				}
				return result.ToString();
			}
		}
		#endregion
		
		#region Extended Options
		[XmlNodeName("ExtendedCPPOptions")]
		public class ExtendedCPPOptions
		{
			[XmlAttribute("callingConvention")]
			public CallingConvention callingConvention = CallingConvention.__cdecl;
			
			[XmlAttribute("compileType")]
			public CompileType compileType = CompileType.CPP;
			
			[XmlAttribute("surpressedWarnings")]
			public string surpressedWarnings = "";
			
			[XmlAttribute("forcedIncludes")]
			public string forcedIncludes = "";
			
			[XmlAttribute("forcedUsings")]
			public string forcedUsings = "";
			
			[XmlAttribute("showIncludes")]
			public bool showIncludes = false;
			
			[XmlAttribute("overridePreProcessorDirectives")]
			public string overridePreProcessorDirectives = "";
			
			[XmlAttribute("overrideAllPreProcessorDirectives")]
			public bool overrideAllPreProcessorDirectives = false;
			
			public CallingConvention CallingConvention {
				get {
					return callingConvention;
				}
				set {
					callingConvention = value;
				}
			}
			public CompileType CompileType {
				get {
					return compileType;
				}
				set {
					compileType = value;
				}
			}
			public string SurpressedWarnings {
				get {
					return surpressedWarnings;
				}
				set {
					surpressedWarnings = value;
				}
			}
			public string ForcedIncludes {
				get {
					return forcedIncludes;
				}
				set {
					forcedIncludes = value;
				}
			}
			public string ForcedUsings {
				get {
					return forcedUsings;
				}
				set {
					forcedUsings = value;
				}
			}
			public bool ShowIncludes {
				get {
					return showIncludes;
				}
				set {
					showIncludes = value;
				}
			}
			public string OverridePreProcessorDirectives {
				get {
					return overridePreProcessorDirectives;
				}
				set {
					overridePreProcessorDirectives = value;
				}
			}
			public bool OverrideAllPreProcessorDirectives {
				get {
					return overrideAllPreProcessorDirectives;
				}
				set {
					overrideAllPreProcessorDirectives = value;
				}
			}
			public string GetCommandLineParameters()
			{
				StringBuilder result = new StringBuilder();
				switch (CallingConvention) {
					case CallingConvention.__cdecl:
						result.Append("/Gd\n");
						break;
					case CallingConvention.__fastcall:
						result.Append("/Gr\n");
						break;
					case CallingConvention.__stdcall:
						result.Append("/Gz\n");
						break;
				}
				
				switch (CompileType) {
					case CompileType.C:
						result.Append("/TC\n");
						break;
					case CompileType.CPP:
						result.Append("/TP\n");
						break;
					case CompileType.Standard:
						break;
				}
			
				if (IsNotEmpty(surpressedWarnings))
				{
					AppendList(result, "/wd", surpressedWarnings);
				}
			
				if (IsNotEmpty(forcedIncludes))
				{
					AppendList(result, "/FI", forcedIncludes, true);
				}
			
				if (IsNotEmpty(forcedUsings))
				{
					AppendList(result, "/FU", forcedUsings, true);
				}
			
				if (showIncludes)
				{
					result.Append("/showIncludes\n");
				}
			
				if (overrideAllPreProcessorDirectives)
				{
					result.Append("/u\n");
				}
				else
				{
					if (IsNotEmpty(overridePreProcessorDirectives))
					{
						AppendList(result, "/U", overridePreProcessorDirectives);
					}
				}
			
				return result.ToString();
			}
		}
		#endregion

		#region General Linker Options
		[XmlNodeName("GeneralLinkerOptions")]
		public class GeneralLinkerOptions : LocalizedObject
		{
			[XmlAttribute("outputFile")]
			[ConvertToRelativePath()]
			public string outputFile = "a.exe";
			
			[XmlAttribute("showLinkerStatus")]
			public ShowLinkerStatus showLinkerStatus = ShowLinkerStatus.Unselected;
			
			[XmlAttribute("version")]
			public string version = "";
			
			[XmlAttribute("incrementalLinking")]
			public IncrementalLinking incrementalLinking = IncrementalLinking.Standard;
			
			[XmlAttribute("surpressStartLogo")]
			public bool surpressStartLogo = false;
			
			[XmlAttribute("additionalLinkerOptions")]
			public string additionalLinkerOptions = "";
			
//			[XmlAttribute("ignoreImportLibrary")]
//			public bool ignoreImportLibrary = true;
			
//			[XmlAttribute("registerOutput")]
//			public bool registerOutput = false;
			
			[XmlAttribute("additionalLibraryDirectories")]
			public string additionalLibraryDirectories = "";
			
			public string GetCommandLineParameters()
			{
				StringBuilder result = new StringBuilder();
//				result.Append("/OUT:");result.Append(outputFile);result.Append("\n");
				switch (ShowLinkerStatus) {
					case ShowLinkerStatus.ShowAll:
						result.Append("/VERBOSE\n");
						break;
					case ShowLinkerStatus.ShowSome:
						result.Append("/VERBOSE:LIB\n");
						break;
					
				}
				if (IsNotEmpty(version)) {
					result.Append("/VERSION:").Append(version).Append("\n");
				}
				
				switch (IncrementalLinking) {
					case IncrementalLinking.Standard:
						break;
					case IncrementalLinking.No:
						result.Append("/INCREMENTAL:NO\n");
						break;
					case IncrementalLinking.Yes:
						result.Append("/INCREMENTAL\n");
						break;
				}
				
				if (SurpressStartLogo) {
					result.Append("/NOLOGO\n");
				}
				
				if (IsNotEmpty(AdditionalLibraryDirectories)) {
					AppendList(result, "/LIBPATH:", AdditionalLibraryDirectories, true);
				}
				result.Append(additionalLinkerOptions);
				result.Append("\n");
				return result.ToString();
			}
			
			[LocalizedProperty("Output File",
			                   Description = "Specifies the name of the output file. (/OUT:[file])")]
			public string OutputFile {
				get {
					return outputFile;
				}
				set {
					outputFile = value;
				}
			}
			
			[DefaultValue(ShowLinkerStatus.Unselected)]
			[LocalizedProperty("Show Status",
			                   Description = "Shows detailed progress status. (/VERBOSE, /VERBOSE:LIB)")]
			public ShowLinkerStatus ShowLinkerStatus {
				get {
					return showLinkerStatus;
				}
				set {
					showLinkerStatus = value;
				}
			}
			
			[DefaultValue("")]
			[LocalizedProperty("Version",
			                   Description = "Use this value as the version number in created image header. (/VERSION:[version])")]
			public string Version {
				get {
					return version;
				}
				set {
					version = value;
				}
			}
			
			[DefaultValue(IncrementalLinking.Standard)]
			[LocalizedProperty("Enable Incremental Linking",
			                   Description = "Enables incremental linking. (/INCREMENTAL, /INCREMENTAL:NO)")]
			public IncrementalLinking IncrementalLinking {
				get {
					return incrementalLinking;
				}
				set {
					incrementalLinking = value;
				}
			}
			
			[DefaultValue(false)]
			[LocalizedProperty("Surpress Startup Logo",
			                   Description = "Surpress the display of the startup logo and information messages. (/NOLOGO)")]
			public bool SurpressStartLogo {
				get {
					return surpressStartLogo;
				}
				set {
					surpressStartLogo = value;
				}
			}
			
//			[DefaultValue(true)]			
//			[LocalizedProperty("Ignore Import Library",
//			                   Description = "Specifies that the import library generated by this configuration should not be imported into dependent projects.")]
//			public bool IgnoreImportLibrary {
//				get {
//					return ignoreImportLibrary;
//				}
//				set {
//					ignoreImportLibrary = value;
//				}
//			}
			
//			[LocalizedProperty("Register Output",
//			                   Description = "Specifies whether to register the primary output of this build.")]
//			public bool RegisterOutput {
//				get {
//					return registerOutput;
//				}
//				set {
//					registerOutput = value;
//				}
//			}
			
			[DefaultValue("")]
			[LocalizedProperty("Additional Library Directories",
			                   Description = "Specifies one or more semi-colon delimited additonal paths to search for libraries. (/LIBPATH:[path])")]
			public string AdditionalLibraryDirectories {
				get {
					return additionalLibraryDirectories;
				}
				set {
					additionalLibraryDirectories = value;
				}
			}
			
			[DefaultValue("")]
			[LocalizedProperty("Additional Linker Options",
			                   Description = "Specifies additional options given to the linker.")]
			public string AdditionalLinkerOptions {
				get {
					return additionalLinkerOptions;
				}
				set {
					additionalLinkerOptions = value;
				}
			}
			
			
		}
		#endregion
		
		#region Input Linker Options
		[XmlNodeName("InputLinkerOptions")]
		public class InputLinkerOptions
		{
			[XmlAttribute("additionalDependencies")]
			public string additionalDependencies = "";
			
			[XmlAttribute("ignoreStandardLibrary")]
			public bool ignoreStandardLibrary = false;
			
			[XmlAttribute("ignoreLibrary")]
			public string ignoreLibrary = "";
			
			[XmlAttribute("moduleDefinition")]
			public string moduleDefinition = "";
			
			[XmlAttribute("addModuleToAssembly")]
			public string addModuleToAssembly = "";
			
			[XmlAttribute("embedResourceToAssembly")]
			public string embedResourceToAssembly = "";
			
			[XmlAttribute("forcedSymbolLinks")]
			public string forcedSymbolLinks = "";
			
			[XmlAttribute("laterLoadedDLLs")]
			public string laterLoadedDLLs = "";
			
			public string GetCommandLineParameters()
			{
				StringBuilder result = new StringBuilder();
				if (ignoreStandardLibrary) 
				{
					result.Append("/NODEFAULTLIB\n");	
				}
				else 
				{
					if (IsNotEmpty(ignoreLibrary)) 
					{
						AppendList(result, "/NODEFAULTLIB:", ignoreLibrary);
					}
				}
				if (IsNotEmpty(additionalDependencies)) 
				{
					AppendList(result, "", additionalDependencies);
				}
				if (IsNotEmpty(moduleDefinition))
				{
					result.Append("/DEF:");	
					result.Append(moduleDefinition);
					result.Append("\n");	
				}
				if (IsNotEmpty(addModuleToAssembly)) 
				{
					AppendList(result, "/ASSEMBLYMODULE:", addModuleToAssembly);
				}
				if (IsNotEmpty(embedResourceToAssembly)) 
				{
					AppendList(result, "/ASSEMBLYRESOURCE:", embedResourceToAssembly);
				}
				if (IsNotEmpty(forcedSymbolLinks)) 
				{
					AppendList(result, "/INCLUDE:", forcedSymbolLinks);
				}
				if (IsNotEmpty(laterLoadedDLLs)) 
				{
					AppendList(result, "/DELAYLOAD:", laterLoadedDLLs);
				}
				return result.ToString();
			}
			
			public string AdditionalDependencies {
				get {
					return additionalDependencies;
				}
				set {
					additionalDependencies = value;
				}
			}
			
			public bool IgnoreStandardLibrary {
				get {
					return ignoreStandardLibrary;
				}
				set {
					ignoreStandardLibrary = value;
				}
			}
			public string IgnoreLibrary {
				get {
					return ignoreLibrary;
				}
				set {
					ignoreLibrary = value;
				}
			}
			public string ModuleDefinition {
				get {
					return moduleDefinition;
				}
				set {
					moduleDefinition = value;
				}
			}
			public string AddModuleToAssembly {
				get {
					return addModuleToAssembly;
				}
				set {
					addModuleToAssembly = value;
				}
			}
			public string EmbedResourceToAssembly {
				get {
					return embedResourceToAssembly;
				}
				set {
					embedResourceToAssembly = value;
				}
			}
			public string ForcedSymbolLinks {
				get {
					return forcedSymbolLinks;
				}
				set {
					forcedSymbolLinks = value;
				}
			}
			public string LaterLoadedDLLs {
				get {
					return laterLoadedDLLs;
				}
				set {
					laterLoadedDLLs = value;
				}
			}
			
		}
		#endregion
		
		#region Debug Linker Options
		[XmlNodeName("DebugLinkerOptions")]
		public class DebugLinkerOptions
		{
			[XmlAttribute("generateDebugInfo")]
			public bool generateDebugInfo = false;
			
			[XmlAttribute("generatedProgramDatabase")]
			public string generatedProgramDatabase = "";
			
			[XmlAttribute("removePrivateSymbols")]
			public string removePrivateSymbols = "";
			
			[XmlAttribute("generateMapFile")]
			public bool generateMapFile = false;
			
			[XmlAttribute("mapFile")]
			public string mapFile = "";
			
			[XmlAttribute("mapExport")]
			public bool mapExport = false;
			
			[XmlAttribute("mapLines")]
			public bool mapLines = false;
			
			[XmlAttribute("debuggableAssembly")]
			public DebuggableAssembly debuggableAssembly;
			
			public bool GenerateDebugInfo {
				get {
					return generateDebugInfo;
				}
				set {
					generateDebugInfo = value;
				}
			}
			public string GeneratedProgramDatabase {
				get {
					return generatedProgramDatabase;
				}
				set {
					generatedProgramDatabase = value;
				}
			}
			public string RemovePrivateSymbols {
				get {
					return removePrivateSymbols;
				}
				set {
					removePrivateSymbols = value;
				}
			}
			public bool GenerateMapFile {
				get {
					return generateMapFile;
				}
				set {
					generateMapFile = value;
				}
			}
			public string MapFile {
				get {
					return mapFile;
				}
				set {
					mapFile = value;
				}
			}
			public bool MapExport {
				get {
					return mapExport;
				}
				set {
					mapExport = value;
				}
			}
			public bool MapLines {
				get {
					return mapLines;
				}
				set {
					mapLines = value;
				}
			}
			public DebuggableAssembly DebuggableAssembly {
				get {
					return debuggableAssembly;
				}
				set {
					debuggableAssembly = value;
				}
			}
			public string GetCommandLineParameters()
			{
				StringBuilder result = new StringBuilder();
				
				if (generateDebugInfo)
				{
					result.Append("/DEBUG\n");
				}
			
				if (IsNotEmpty(generatedProgramDatabase))
				{
					result.Append("/PDB:");
					result.Append(generatedProgramDatabase);
					result.Append("\n");
				}
			
				if (IsNotEmpty(removePrivateSymbols))
				{
					result.Append("/PDBSTRIPPED:");
					result.Append(removePrivateSymbols);
					result.Append("\n");
				}
			
				if (generateMapFile)
				{
					result.Append("/MAP");
					if (IsNotEmpty(mapFile))
					{
						result.Append(":");
						result.Append(mapFile);
					}
					result.Append("\n");
					
				}
				
				if (mapExport)
				{
					result.Append("/MAPINFO:EXPORTS\n");
				}
			
				if (mapLines)
				{
					result.Append("/MAPINFO:LINES\n");
				}
				
				switch (debuggableAssembly) {
					case DebuggableAssembly.DontEmitDebuggable:
						break;
					case DebuggableAssembly.DisableDebugToRuntimEnableOptimization:
						result.Append("/ASSEMBLYDEBUG:DISABLE\n");
						break;
					case DebuggableAssembly.EnableDebugToRuntimeDisableOptimization:
						result.Append("/ASSEMBLYDEBUG\n");
						break;
				}
			
				return result.ToString();
			}
			
		}
		#endregion

		#region System Linker Options
		[XmlNodeName("SystemLinkerOptions")]
		public class SystemLinkerOptions
		{
			[XmlAttribute("linkerSubSystem")]
			public LinkerSubSystem linkerSubSystem = LinkerSubSystem.Unselected;
			
			[XmlAttribute("heapAllocationSize")]
			public int heapAllocationSize = 0;
			
			[XmlAttribute("heapCommitSize")]
			public int heapCommitSize = 0;
			
			[XmlAttribute("stackAllocationSize")]
			public int stackAllocationSize = 0;
			
			[XmlAttribute("stackCommitSize")]
			public int stackCommitSize= 0;
			
			[XmlAttribute("activateBigAddresses")]
			public ActivateBigAddresses activateBigAddresses = ActivateBigAddresses.Standard;
			
			[XmlAttribute("terminalServer")]
			public TerminalServer terminalServer = TerminalServer.Standard;
			
			[XmlAttribute("runFromCDROM")]
			public bool runFromCDROM = false;
			
			[XmlAttribute("runFromNetwork")]
			public bool runFromNetwork = false;
			
			public LinkerSubSystem LinkerSubSystem {
				get {
					return linkerSubSystem;
				}
				set {
					linkerSubSystem = value;
				}
			}
			public int HeapAllocationSize {
				get {
					return heapAllocationSize;
				}
				set {
					heapAllocationSize = value;
				}
			}
			public int HeapCommitSize {
				get {
					return heapCommitSize;
				}
				set {
					heapCommitSize = value;
				}
			}
			public int StackAllocationSize {
				get {
					return stackAllocationSize;
				}
				set {
					stackAllocationSize = value;
				}
			}
			public int StackCommitSize {
				get {
					return stackCommitSize;
				}
				set {
					stackCommitSize = value;
				}
			}
			public ActivateBigAddresses ActivateBigAddresses {
				get {
					return activateBigAddresses;
				}
				set {
					activateBigAddresses = value;
				}
			}
			public TerminalServer TerminalServer {
				get {
					return terminalServer;
				}
				set {
					terminalServer = value;
				}
			}
			public bool RunFromCDROM {
				get {
					return runFromCDROM;
				}
				set {
					runFromCDROM = value;
				}
			}
			public bool RunFromNetwork {
				get {
					return runFromNetwork;
				}
				set {
					runFromNetwork = value;
				}
			}
			public string GetCommandLineParameters()
			{
				StringBuilder result = new StringBuilder();

				switch (LinkerSubSystem) {
					case LinkerSubSystem.Console:
						result.Append("/SUBSYSTEM:CONSOLE\n");
						break;
					case LinkerSubSystem.Unselected:
						break;
					case LinkerSubSystem.Windows:
						result.Append("/SUBSYSTEM:WINDOWS\n");
						break;
				}
			
				if (heapAllocationSize > 0) 
				{
					result.Append("/HEAP:");
					result.Append(heapAllocationSize);
					if (heapCommitSize > 0) 
					{
						result.Append(",");
						result.Append(heapCommitSize);
					}
					result.Append("\n");
				}

				if (stackAllocationSize > 0)
				{
					result.Append("/STACK:");
					result.Append(stackAllocationSize);
					if (stackCommitSize > 0) 
					{
						result.Append(",");
						result.Append(stackCommitSize);
					}
					result.Append("\n");
				}
				
				switch (ActivateBigAddresses) {
					case ActivateBigAddresses.NoSupport:
						result.Append("/LARGEADDRESSAWARE:NO\n");
						break;
					case ActivateBigAddresses.Standard:
						break;
					case ActivateBigAddresses.Supported:
						result.Append("/LARGEADDRESSAWARE\n");
						break;
				}
			
				switch (TerminalServer) {
					case TerminalServer.Bound:
						result.Append("/TSAWARE\n");
						break;
					case TerminalServer.NotBound:
						result.Append("/TSAWARE:NO\n");
						break;
					case TerminalServer.Standard:
						break;
				}
			
				if (runFromCDROM)
				{
					result.Append("/SWAPRUN:CD\n");
				}
			
				if (runFromNetwork)
				{
					result.Append("/SWAPRUN:NET\n");
				}
				
				return result.ToString();
			}
			
		}
		#endregion
		
		
		public bool PreCompileHeader {
			get {
				return preCompiledHeaderCPPOptions.PreCompileHeader;
			}
		}
			
		public string GetPreCompiledHeaderOptions()
		{
			return generalCPPOptions.GetCommandLineParameters() +
			       optimizeCPPOptions.GetCommandLineParameters() +
			       preProcessorCPPOptions.GetCommandLineParameters() +
			       codeGenerationCPPOptions.GetCommandLineParameters() +
			       languageCPPOptions.GetCommandLineParameters() +
			       preCompiledHeaderCPPOptions.GetCreatePreCompiledHeaderParameter() +
			       outputFileCPPOptions.GetCommandLineParameters() +
			       informationSearchCPPOptions.GetCommandLineParameters() +
			       extendedCPPOptions.GetCommandLineParameters();
		}
		
		public string GetCompilerOptions()
		{
			return generalCPPOptions.GetCommandLineParameters() +
			       optimizeCPPOptions.GetCommandLineParameters() +
			       preProcessorCPPOptions.GetCommandLineParameters() +
			       codeGenerationCPPOptions.GetCommandLineParameters() +
			       languageCPPOptions.GetCommandLineParameters() +
			       preCompiledHeaderCPPOptions.GetCommandLineParameters() +
			       outputFileCPPOptions.GetCommandLineParameters() +
			       informationSearchCPPOptions.GetCommandLineParameters() +
			       extendedCPPOptions.GetCommandLineParameters();
		}
		
		public string GetLinkerOptions()
		{
			return generalLinkerOptions.GetCommandLineParameters() +
			       inputLinkerOptions.GetCommandLineParameters() +
			       debugLinkerOptions.GetCommandLineParameters() +
			       systemLinkerOptions.GetCommandLineParameters();
		}
		
		public string GetLinkerOptionsForCompiler()
		{
			StringBuilder result = new StringBuilder();
			foreach (string val in GetLinkerOptions().Split('\n'))
			{
				result.Append(val);
				result.Append("\n");
			}
			return result.ToString();
		}
			
		public GeneralCPPOptions generalCPPOptions = new GeneralCPPOptions();
		public OptimizeCPPOptions optimizeCPPOptions = new OptimizeCPPOptions();
		public PreProcessorCPPOptions preProcessorCPPOptions     = new PreProcessorCPPOptions();
		public CodeGenerationCPPOptions codeGenerationCPPOptions = new CodeGenerationCPPOptions();
		public LanguageCPPOptions languageCPPOptions             = new LanguageCPPOptions();
		public PreCompiledHeaderCPPOptions preCompiledHeaderCPPOptions = new PreCompiledHeaderCPPOptions();
		public OutputFileCPPOptions outputFileCPPOptions = new OutputFileCPPOptions();
		public InformationSearchCPPOptions informationSearchCPPOptions = new InformationSearchCPPOptions();
		public ExtendedCPPOptions extendedCPPOptions = new ExtendedCPPOptions();
		
		public GeneralLinkerOptions generalLinkerOptions = new GeneralLinkerOptions();
		public InputLinkerOptions inputLinkerOptions = new InputLinkerOptions();
		public DebugLinkerOptions debugLinkerOptions = new DebugLinkerOptions();
		public SystemLinkerOptions systemLinkerOptions = new SystemLinkerOptions();
		
		MiscCPPOptions     miscCPPOptions                 = new MiscCPPOptions();
		
		public override string OutputDirectory {
			get {
				return miscCPPOptions.outputDirectory;
			}
			set {
				miscCPPOptions.outputDirectory = value;
			}
		}

		public string OutputFile {
			get {
				return generalLinkerOptions.OutputFile;
			}
			set {
				generalLinkerOptions.OutputFile = value;
			}
		}
		
		public string IntermediateDirectory {
			get {
				return miscCPPOptions.intermediateDirectory;
			}
			set {
				miscCPPOptions.intermediateDirectory = value;
			}
		}
		
		public ConfigurationType ConfigurationType {
			get {
				return miscCPPOptions.configurationType;
			}
			set {
				miscCPPOptions.configurationType = value;
			}
		}
		
		public bool UseManagedExtensions {
			get {
				return miscCPPOptions.useManagedExtensions;
			}
			set {
				miscCPPOptions.useManagedExtensions = value;
			}
		}
		
		public string AdditionalCompilerOptions {
			get {
				return miscCPPOptions.additionalCompilerOptions;
			}
			set {
				miscCPPOptions.additionalCompilerOptions = value;
			}
		}
		
//		public CPPCompilerParameters(string[] additionalCompilerOptions)
//		{
//			this.AdditionalCompilerOptions = additionalCompilerOptions;
//		}
		 
		public CPPCompilerParameters()
		{
		}
		
		public CPPCompilerParameters(string name)
		{
			this.name = name;
			
		}
	}
}
