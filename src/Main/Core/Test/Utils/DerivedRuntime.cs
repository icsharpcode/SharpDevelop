// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		public DerivedRuntime(string assembly, string hintPath)
			: this(assembly, hintPath, new List<AddIn>())
		{
		}
		public DerivedRuntime(string assembly, string hintPath, IList<AddIn> addIns)
			: base(assembly, hintPath, addIns)
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
