// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ICSharpCode.Python.Build.Tasks
{
	/// <summary>
	/// Python compiler interface.
	/// </summary>
	public interface IPythonCompiler : IDisposable
	{
		/// <summary>
		/// Gets or sets the source files to compile.
		/// </summary>
		IList<string> SourceFiles { get; set; }
		
		/// <summary>
		/// Gets or sets the filenames of the referenced assemblies.
		/// </summary>
		IList<string> ReferencedAssemblies { get; set; }
		
		/// <summary>
		/// Gets or sets the resources to be compiled.
		/// </summary>
		IList<ResourceFile> ResourceFiles { get; set; }

		/// <summary>
		/// Executes the compiler.
		/// </summary>
		void Compile();
		
		/// <summary>
		/// Gets or sets the type of the compiled assembly (e.g. windows app,
		/// console app or dll).
		/// </summary>
		PEFileKinds TargetKind { get; set; }
		
		/// <summary>
		/// Gets or sets the nature of the code in the executable produced by the compiler.
		/// </summary>
		PortableExecutableKinds ExecutableKind { get; set; }
		
		/// <summary>
		/// Gets or sets the machine that will be targeted by the compiler.
		/// </summary>
		ImageFileMachine Machine { get; set; }
		
		/// <summary>
		/// Gets or sets the file that contains the main entry point.
		/// </summary>
		string MainFile { get; set; }
		
		/// <summary>
		/// Gets or sets the output assembly filename.
		/// </summary>
		string OutputAssembly { get; set; }	
		
		/// <summary>
		/// Gets or sets whether the compiler should include debug
		/// information in the created assembly.
		/// </summary>
		bool IncludeDebugInformation { get; set; }
	}
}
