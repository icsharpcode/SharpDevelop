// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Resources;

namespace PythonBinding.Tests.Utils
{
	public class MockResourceWriter : IResourceWriter
	{
		bool disposed;
		
		public MockResourceWriter()
		{
		}
		
		public void AddResource(string name, string value)
		{
			throw new NotImplementedException();
		}
		
		public void AddResource(string name, object value)
		{
			throw new NotImplementedException();
		}
		
		public void AddResource(string name, byte[] value)
		{
			throw new NotImplementedException();
		}
		
		public void Close()
		{
			throw new NotImplementedException();
		}
		
		public void Generate()
		{
			throw new NotImplementedException();
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
