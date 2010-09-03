// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class MockSite : ISite
	{
		public MockSite()
		{
			Name = String.Empty;
		}
		
		public IComponent Component {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IContainer Container {
			get { return null; }
		}
		
		public bool DesignMode {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string Name { get; set; }
		
		public object GetService(Type serviceType)
		{
			return null;
		}
	}
}
