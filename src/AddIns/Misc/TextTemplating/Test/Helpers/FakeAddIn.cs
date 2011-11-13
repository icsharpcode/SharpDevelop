// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Reflection;
using ICSharpCode.TextTemplating;

namespace TextTemplating.Tests.Helpers
{
	public class FakeAddIn : IAddIn
	{
		public FakeAddIn()
			: this(String.Empty)
		{
		}
		
		public FakeAddIn(string id)
		{
			this.PrimaryIdentity = id;
		}
		
		public string PrimaryIdentity { get; set; }
		
		public List<FakeAddInRuntime> FakeAddInRuntimes = new List<FakeAddInRuntime>();
		
		public IEnumerable<IAddInRuntime> GetRuntimes()
		{
			return FakeAddInRuntimes;
		}
		
		public FakeAddInRuntime AddFakeAddInRuntime(string assemblyFileName)
		{
			return AddFakeAddInRuntime(assemblyFileName, null);
		}
		
		public FakeAddInRuntime AddFakeAddInRuntime(string assemblyFileName, Assembly loadedAssembly)
		{
			var runtime = new FakeAddInRuntime(assemblyFileName, loadedAssembly);
			FakeAddInRuntimes.Add(runtime);
			return runtime;
		}
	}
}
