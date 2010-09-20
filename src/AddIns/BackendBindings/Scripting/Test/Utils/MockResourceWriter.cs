// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Resources;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class MockResourceWriter : IResourceWriter
	{
		bool disposed;
		Dictionary<string, object> resources = new Dictionary<string, object>();
		
		public MockResourceWriter()
		{
		}
		
		public object GetResource(string name)
		{
			if (resources.ContainsKey(name)) {
				return resources[name];
			}
			return null;
		}
		
		public void AddResource(string name, string value)
		{
			resources.Add(name, value);
		}
		
		public void AddResource(string name, object value)
		{
			resources.Add(name, value);
		}
		
		public void AddResource(string name, byte[] value)
		{
			throw new NotImplementedException();
		}
		
		public void Close()
		{
		}
		
		public void Generate()
		{
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
