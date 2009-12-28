// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;

namespace RubyBinding.Tests.Utils
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
