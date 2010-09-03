// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using Debugger.AddIn.Visualizers.Common;
using Debugger.AddIn.Visualizers.Utils;
using Debugger.MetaData;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Services;
using System.Reflection;

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
				index,
				this.memberFromNameMap);
		}
		
		// TODO move to Utils iListType.EvaluateCount(Expression targetObject)
		// or targetObject.EvaluateCount(iListType)
		// or targetObject.EvaluateIListCount()	<- calls ResolveIListImplementation
		int evaluateCount()
		{
			PropertyInfo countProperty = this.collectionType.GetGenericInterface("System.Collections.Generic.ICollection").GetProperty("Count");
			// Do not get string representation since it can be printed in hex later
			Value countValue = targetObject.Evaluate(WindowsDebugger.CurrentProcess).GetPropertyValue(countProperty);
			//Value countValue = targetObject.AppendMemberReference(countProperty).Evaluate(WindowsDebugger.CurrentProcess);
			return (int)countValue.PrimitiveValue;
		}
	}
}
