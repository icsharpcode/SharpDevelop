// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using ICSharpCode.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using Debugger.AddIn.Visualizers.Utils;
using Debugger.MetaData;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Visualizers.GridVisualizer
{
	/// <summary>
	/// Provides <see cref="ObjectValue"/>s for debugee objects implementing IEnumerable.
	/// </summary>
	public class EnumerableValuesProvider : GridValuesProvider
	{
		public EnumerableValuesProvider(Expression targetObject, DebugType iEnumerableType, DebugType itemType)
			:base(targetObject, iEnumerableType, itemType)
		{
			this.itemsSource = enumerateItems();
		}
		
		private IEnumerable<ObjectValue> itemsSource;
		public IEnumerable<ObjectValue> ItemsSource
		{
			get { return this.itemsSource; }
		} 
		
		private IEnumerable<ObjectValue> enumerateItems()
		{
			MethodInfo enumeratorMethod = collectionType.GetMethod("GetEnumerator");
			Value enumerator = targetObject.Evaluate(WindowsDebugger.CurrentProcess).InvokeMethod(enumeratorMethod, null).GetPermanentReference();
			
			MethodInfo moveNextMethod = enumerator.Type.GetMethod("MoveNext");
			PropertyInfo currentproperty = enumerator.Type.GetInterface(typeof(IEnumerator).FullName).GetProperty("Current");

			int index = 0;
			while ((bool)enumerator.InvokeMethod(moveNextMethod, null).PrimitiveValue)
			{
				Value currentValue = enumerator.GetPropertyValue(currentproperty);
				yield return ObjectValue.Create(currentValue, index++, this.memberFromNameMap);
			}
		}
	}
}
