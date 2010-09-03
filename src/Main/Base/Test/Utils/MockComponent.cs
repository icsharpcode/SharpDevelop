// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
