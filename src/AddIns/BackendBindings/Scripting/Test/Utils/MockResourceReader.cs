// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class MockResourceReader : IResourceReader
	{
		bool disposed;
		Dictionary<string, object> resources = new Dictionary<string, object>();
		
		public MockResourceReader()
		{
		}
		
		public void AddResource(string name, object value)
		{
			resources.Add(name, value);
		}
		
		public void Close()
		{
		}
		
		public IDictionaryEnumerator GetEnumerator()
		{
			return resources.GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return resources.GetEnumerator();
		}
		
		public void Dispose()
		{
			disposed = true;
		}
		
		public bool IsDisposed {
			get { return disposed; }
		}		
	}
}
