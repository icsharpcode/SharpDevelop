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
	public class MockComponent : IComponent
	{
		ISite site;

		public MockComponent()
		{
		}
		
		public override string ToString()
		{
			if (site != null) {
				return site.Name;
			}
			return base.ToString();
		}
				
		public event EventHandler Disposed;
				
		public ISite Site {
			get { return site; }
			set { site = value; }
		}
		
		public void Dispose()
		{
		}		
		
		protected virtual void OnDisposed(EventArgs e)
		{
			if (Disposed != null) {
				Disposed(this, e);
			}
		}		
	}
}
