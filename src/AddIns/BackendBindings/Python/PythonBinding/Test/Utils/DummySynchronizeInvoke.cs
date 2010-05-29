// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;

namespace PythonBinding.Tests.Utils
{
	public class DummySynchronizeInvoke : ISynchronizeInvoke
	{
		public DummySynchronizeInvoke()
		{
		}
		
		public bool InvokeRequired { 
			get { return false; }
		}
		
		public IAsyncResult BeginInvoke(Delegate method, object[] args)
		{
			return null;
		}
		
		public object EndInvoke(IAsyncResult result)
		{
			return null;
		}
		
		public object Invoke(Delegate method, object[] args)
		{
			return null;
		}
	}
}
