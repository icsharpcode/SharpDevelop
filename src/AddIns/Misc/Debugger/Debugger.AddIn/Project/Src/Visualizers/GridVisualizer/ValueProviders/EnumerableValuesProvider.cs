// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using Debugger.MetaData;
using System;
using System.Collections;
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
		private readonly Debugger.MetaData.BindingFlags bindingFlags =
			BindingFlags.Public | BindingFlags.Instance | BindingFlags.Field | BindingFlags.GetProperty;
		
		//private WindowsDebugger debugger;
		
		private Expression targetObject;
		private DebugType iEnumerableType;
		private DebugType itemType;
		
		public EnumerableValuesProvider(Expression targetObject, DebugType iEnumerableType, DebugType itemType)
		{
			this.targetObject = targetObject;
			this.iEnumerableType = iEnumerableType;
			this.itemType = itemType;
			
			this.itemsSource = enumerateItems();
		}
		
		private IEnumerable<ObjectValue> itemsSource;
		public IEnumerable<ObjectValue> ItemsSource
		{
			get { return this.itemsSource; }
		} 
		
		public IList<MemberInfo> GetItemTypeMembers()
		{
			return itemType.GetMembers(this.bindingFlags);
		}
		
		private IEnumerable<ObjectValue> enumerateItems()
		{
			MethodInfo enumeratorMethod = iEnumerableType.GetMethod("GetEnumerator");
			Value enumerator = targetObject.Evaluate(WindowsDebugger.CurrentProcess).InvokeMethod(enumeratorMethod, null).GetPermanentReference();
			
			MethodInfo moveNextMethod = enumerator.Type.GetMethod("MoveNext");
			PropertyInfo currentproperty = enumerator.Type.GetInterface(typeof(IEnumerator).FullName).GetProperty("Current");

			while ((bool)enumerator.InvokeMethod(moveNextMethod, null).PrimitiveValue)
			{
				Value currentValue = enumerator.GetPropertyValue(currentproperty).GetPermanentReference();
				yield return ObjectValue.Create(currentValue, this.itemType, this.bindingFlags);
			}
		}
	}
}
