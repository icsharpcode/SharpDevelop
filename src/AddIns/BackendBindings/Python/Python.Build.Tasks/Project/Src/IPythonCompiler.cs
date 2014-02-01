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
