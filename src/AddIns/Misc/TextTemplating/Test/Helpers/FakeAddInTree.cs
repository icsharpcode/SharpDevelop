// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Reflection;

using ICSharpCode.TextTemplating;

namespace TextTemplating.Tests.Helpers
{
	public class FakeAddInTree : IAddInTree
	{
		List<FakeAddIn> FakeAddIns = new List<FakeAddIn>();
		
		public FakeAddIn AddFakeAddIn(string id)
		{
			var fakeAddIn = new FakeAddIn(id);
			FakeAddIns.Add(fakeAddIn);
			return fakeAddIn;
		}
		
		public IEnumerable<IAddIn> GetAddIns()
		{
			return FakeAddIns;
		}
		
		public List<IServiceProvider> BuildServiceProviders(string path)
		{
			return null;
		}
	}
}
