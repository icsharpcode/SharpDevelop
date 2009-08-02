// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using Debugger.AddIn.Visualizers.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using Debugger.AddIn.Visualizers.Utils;
using Debugger.MetaData;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.NRefactory.Ast;

namespace Debugger.AddIn.Visualizers.GridVisualizer
{
	/// <summary>
	///  Provides <see cref="ObjectValue"/>s for debugee objects implementing IList.
	/// </summary>
	public class ListValuesProvider : GridValuesProvider, IListValuesProvider<ObjectValue>
	{
		private bool countEvaluated = false;
		private int count = -1;
		
		public ListValuesProvider(Expression targetObject, DebugType iListType, DebugType listItemType)
			:base(targetObject, iListType, listItemType)
		{
		}
		
		public int GetCount()
		{
			if (!countEvaluated)
			{
				this.count = evaluateCount();
				countEvaluated = true;
			}
			return this.count;
		}
		
		public ObjectValue GetItemAt(int index)
		{
			return ObjectValue.Create(
				targetObject.AppendIndexer(index).Evaluate(WindowsDebugger.CurrentProcess),
				//targetObject.AppendIndexer(index), // use Expression instead of value - possible only for IList though
				this.memberFromNameMap);
		}
		
		// TODO move to Utils iListType.EvaluateCount(Expression targetObject)
		// or targetObject.EvaluateCount(iListType)
		// or targetObject.EvaluateIListCount()	<- calls ResolveIListImplementation
		private int evaluateCount()
		{
			PropertyInfo countProperty = this.collectionType.GetGenericInterface("System.Collections.Generic.ICollection").GetProperty("Count");
			try {
				// Do not get string representation since it can be printed in hex later
				Value countValue = targetObject.Evaluate(WindowsDebugger.CurrentProcess).GetPropertyValue(countProperty);
				return (int)countValue.PrimitiveValue;
			} catch (GetValueException) {
				return -1;
			}
		}
	}
}
