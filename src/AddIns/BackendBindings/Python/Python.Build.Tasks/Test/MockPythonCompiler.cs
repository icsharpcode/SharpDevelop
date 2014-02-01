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
using ICSharpCode.Python.Build.Tasks;
using IronPython.Hosting;

namespace Python.Build.Tasks.Tests
{
	/// <summary>
	/// Implements the IPythonCompiler interface so the 
	/// PythonCompiler task can be tested.
	/// </summary>
	public class MockPythonCompiler : IPythonCompiler
	{
		IList<string> sourceFiles;
		bool compileCalled;
		bool disposeCalled;
		PEFileKinds targetKind;
		PortableExecutableKinds executableKind;
		ImageFileMachine machine;
		string mainFile;
		string outputAssembly;
		bool includeDebugInformation;
		IList<string> referencedAssemblies;
		IList<ResourceFile> resourceFiles;
		Exception throwExceptionAtCompile;
		
		public MockPythonCompiler()
		{
		}
		
		/// <summary>
		/// Gets or sets the source files to compiler.
		/// </summary>
		public IList<string> SourceFiles { 
			get { return sourceFiles; }
			set { sourceFiles = value; }
		}
		
		/// <summary>
		/// Gets or sets the filenames of the referenced assemblies.
		/// </summary>
		public IList<string> ReferencedAssemblies { 
			get { return referencedAssemblies; }
			set { referencedAssemblies = value; }
		}

		/// <summary>
		/// Gets or sets the resources to be compiled.
		/// </summary>
		public IList<ResourceFile> ResourceFiles { 
			get { return resourceFiles; }
			set { resourceFiles = value; }
		}
		
		/// <summary>
		/// Gets or sets the exception that will be thrown when the Compile method is called.
		/// </summary>
		public Exception ThrowExceptionAtCompile {
			get { return throwExceptionAtCompile; }
			set { throwExceptionAtCompile = value; }
		}
	
		/// <summary>
		/// Compiles the source code.
		/// </summary>
		public void Compile()
		{
			compileCalled = true;
			
			if (throwExceptionAtCompile != null) {
				throw throwExceptionAtCompile;
			}
		}
		
		/// <summary>
		/// Disposes the compiler.
		/// </summary>
		public void Dispose()
		{
			disposeCalled = true;
		}
		
		/// <summary>
		/// Gets or sets the type of the compiled assembly.
		/// </summary>
		public PEFileKinds TargetKind {
			get { return targetKind; }
			set { targetKind = value; }
		}
		
		public PortableExecutableKinds ExecutableKind { 
			get { return executableKind; }
			set { executableKind = value; }
		}
		
		public ImageFileMachine Machine {
			get { return machine; }
			set { machine = value; }
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
		public bool IncludeDebugInformation {
			get { return includeDebugInformation; }
			set { includeDebugInformation = value; }
		}
		
		/// <summary>
		/// Gets whether the Compile method has been called.
		/// </summary>
		public bool CompileCalled {
			get { return compileCalled;	}
		}

		/// <summary>
		/// Gets whether the Dispose method has been called.
		/// </summary>
		public bool DisposeCalled {
			get { return disposeCalled;	}
		}
	}
}
