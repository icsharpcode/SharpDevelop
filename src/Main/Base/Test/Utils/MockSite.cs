// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
