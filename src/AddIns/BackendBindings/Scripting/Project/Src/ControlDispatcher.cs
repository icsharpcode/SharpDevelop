// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Threading;

namespace ICSharpCode.Scripting
{
	public class ControlDispatcher : IControlDispatcher
	{
		Dispatcher dispatcher;
		
		public ControlDispatcher(UIElement uiElement)
		{
			dispatcher = uiElement.Dispatcher;
		}
		
		public bool CheckAccess()
		{
			return dispatcher.CheckAccess();
		}
		
		public object Invoke(Delegate method, params object[] args)
		{
			return dispatcher.Invoke(method, args);
		}
	}
}
