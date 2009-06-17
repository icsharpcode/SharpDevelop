// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using ICSharpCode.SharpDevelop.Services;
using Debugger.Expressions;

namespace Debugger.AddIn.Visualizers.GridVisualizer.ValueProviders
{
	/// <summary>
	///  Provides <see cref="ObjectValue"/>s for debugee objects implementing IList.
	/// </summary>
	public class ListValuesProvider
	{
		private WindowsDebugger debugger;
		private Expression targetObject;
		
		public ListValuesProvider(WindowsDebugger debuggerService, Expression targetObject)
		{
			this.debugger = debuggerService;
			this.targetObject = targetObject;
		}
		
		public int GetCount()
		{
			return -1;
		}
		
		public ObjectValue GetItemAt(int index)
		{
			return null;
		}
	}
}
