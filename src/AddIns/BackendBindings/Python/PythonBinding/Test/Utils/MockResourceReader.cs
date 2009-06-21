// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Resources;

namespace PythonBinding.Tests.Utils
{
	public class MockResourceReader : IResourceReader
	{
		bool disposed;
		
		public MockResourceReader()
		{
		}
		
		public void Close()
		{
			throw new NotImplementedException();
		}
		
		public IDictionaryEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return null;
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
