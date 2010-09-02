// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
