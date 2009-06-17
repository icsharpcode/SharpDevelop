// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using Debugger.Expressions;
using ICSharpCode.SharpDevelop.Services;
using System.Collections.Generic;

namespace Debugger.AddIn.Visualizers.GridVisualizer.ValueProviders
{
	/// <summary>
	/// Provides <see cref="ObjectValue"/>s for debugee objects implementing IEnumerable.
	/// </summary>
	public class EnumerableValuesProvider
	{
		private WindowsDebugger debugger;
		
		public EnumerableValuesProvider(WindowsDebugger debuggerService, Expression targetObject)
		{
			this.debugger = debuggerService;
			this.itemsSource = enumerateItems(targetObject);
		}
		
		private IEnumerable<ObjectValue> itemsSource;
		public IEnumerable<ObjectValue> ItemsSource
		{
			get { return this.itemsSource; }
		} 
		
		private IEnumerable<ObjectValue> enumerateItems(Expression targetObject)
		{
			return null;
		}
	}
}
