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
using System.IO;
using System.Reflection;
using ICSharpCode.Core;

namespace ICSharpCode.Core.Tests.Utils
{
	public class DerivedRuntime : Runtime
	{
		string assemblyNamePassedToLoadAssembly;
		string assemblyFileNamePassedToLoadAssemblyFrom;
		Dictionary<string, Assembly> assemblyNames = new Dictionary<string, Assembly>();
		Dictionary<string, Assembly> assemblyFileNames = new Dictionary<string, Assembly>();
		string errorMessageDisplayed;
		
		public DerivedRuntime(IAddInTree addInTree, string assembly, string hintPath)
			: base(addInTree, assembly, hintPath)
		{
		}
		
		public string AssemblyNamePassedToLoadAssembly {
			get { return assemblyNamePassedToLoadAssembly; }
		}
		
		public Exception LoadAssemblyExceptionToThrow { get; set; }
		
		public Assembly CallLoadAssembly(string assemblyString)
		{
			return LoadAssembly(assemblyString);
		}
		
		protected override Assembly LoadAssembly(string assemblyString)
		{
			assemblyNamePassedToLoadAssembly = assemblyString;
			
			if (LoadAssemblyExceptionToThrow != null) {
				throw LoadAssemblyExceptionToThrow;
			}
			
			Assembly assembly;
			if (assemblyNames.TryGetValue(assemblyString, out assembly)) {
				return assembly;
			}
			return typeof(DerivedRuntime).Assembly;
		}
		
		public Dictionary<string, Assembly> AssemblyNames {
			get { return assemblyNames; }
		}
		
		public string AssemblyFileNamePassedToLoadAssemblyFrom {
			get { return assemblyFileNamePassedToLoadAssemblyFrom; }
		}
		
		public Exception LoadAssemblyFromExceptionToThrow { get; set; }

		public Assembly CallLoadAssemblyFrom(string assemblyFile)
		{
			return LoadAssemblyFrom(assemblyFile);
		}
		
		protected override Assembly LoadAssemblyFrom(string assemblyFile)
		{
			assemblyFileNamePassedToLoadAssemblyFrom = assemblyFile;
			
			if (LoadAssemblyFromExceptionToThrow != null) {
				throw LoadAssemblyFromExceptionToThrow;
			}
			
			string fileName = Path.GetFileName(assemblyFile);
			Assembly assembly;
			if (assemblyFileNames.TryGetValue(fileName, out assembly)) {
				return assembly;
			}
			return typeof(DerivedRuntime).Assembly;
		}
		
		public Dictionary<string, Assembly> AssemblyFileNames {
			get { return assemblyFileNames; }
		}
		
		public string ErrorMessageDisplayed {
			get { return errorMessageDisplayed; }
		}
		
		protected override void ShowError(string message)
		{
			errorMessageDisplayed = message;
		}
	}
}
