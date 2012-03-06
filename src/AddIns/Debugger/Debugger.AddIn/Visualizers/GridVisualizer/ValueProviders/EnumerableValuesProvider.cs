// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;

using Debugger.AddIn.Visualizers.Utils;
using Debugger.MetaData;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Services;
using System.Reflection;

namespace Debugger.AddIn.Visualizers.GridVisualizer
{
	/// <summary>
	/// Provides <see cref="ObjectValue"/>s for debugee objects implementing IEnumerable.
	/// </summary>
	/*public class EnumerableValuesProvider : GridValuesProvider
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
	}*/
}
