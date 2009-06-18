// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using Debugger.MetaData;
using System;
using System.Collections.Generic;
using Debugger.Expressions;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Visualizers.GridVisualizer
{
	/// <summary>
	/// Provides <see cref="ObjectValue"/>s for debugee objects implementing IEnumerable.
	/// </summary>
	public class EnumerableValuesProvider
	{
		//private WindowsDebugger debugger;
		
		private Expression targetObject;
		private DebugType iListType;
		private DebugType listItemType;
		
		public EnumerableValuesProvider(Expression targetObject, DebugType iListType, DebugType listItemType)
		{
			this.targetObject = targetObject;
			this.iListType = iListType;
			this.listItemType = listItemType;
			
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
