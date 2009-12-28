// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;

namespace RubyBinding.Tests.Utils
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
