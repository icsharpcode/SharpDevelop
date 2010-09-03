// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
