// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;

namespace ICSharpCode.SharpDevelop.Tests.Utils
{
	public class MockSite : ISite
	{
		string name = String.Empty;
		
		public MockSite()
		{
		}
		
		public override string ToString()
		{
			return name;
		}
		
		public IComponent Component {
			get { return null; }
		}
		
		public IContainer Container {
			get { return null; }
		}
		
		public bool DesignMode {
			get { return false; }
		}
		
		public string Name {
			get { return name; }
			set { name = value; }
		}
		
		public object GetService(Type serviceType)
		{
			return null;
		}		
	}
}
