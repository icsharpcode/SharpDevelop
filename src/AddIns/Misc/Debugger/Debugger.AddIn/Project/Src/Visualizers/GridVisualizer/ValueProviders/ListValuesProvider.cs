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
using Debugger.Expressions;
using Debugger.MetaData;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Visualizers.GridVisualizer
{
	/// <summary>
	///  Provides <see cref="ObjectValue"/>s for debugee objects implementing IList.
	/// </summary>
	public class ListValuesProvider : IListValuesProvider<ObjectValue>
	{
		private readonly Debugger.MetaData.BindingFlags bindingFlags =
			BindingFlags.Public | BindingFlags.Instance | BindingFlags.Field | BindingFlags.GetProperty;
		
		private Expression targetObject;
		private DebugType iListType;
		private DebugType listItemType;
		
		private bool countEvaluated = false;
		private int count = -1;
		
		public ListValuesProvider(Expression targetObject, DebugType iListType, DebugType listItemType)
		{
			this.targetObject = targetObject;
			this.iListType = iListType;
			this.listItemType = listItemType;
		}
		
		public int GetCount()
		{
			if (!countEvaluated)
			{
				this.count = evaluateCount();
			}
			return this.count;
		}
		
		public ObjectValue GetItemAt(int index)
		{
			PropertyInfo itemProperty = iListType.GetProperty("Item");
			Expression itemExpr = targetObject.AppendMemberReference(itemProperty, new PrimitiveExpression(index));
			return ObjectValue.Create(
				itemExpr.Evaluate(WindowsDebugger.CurrentProcess).GetPermanentReference(), 
				this.listItemType, this.bindingFlags);
		}
		
		public IList<MemberInfo> GetItemTypeMembers()
		{
			return listItemType.GetMembers(this.bindingFlags);
		}
		
		// TODO move to Utils iListType.EvaluateCount(Expression targetObject)
		// or targetObject.EvaluateCount(iListType)
		// or targetObject.EvaluateIListCount()	<- calls ResolveIListImplementation
		private int evaluateCount()
		{
			PropertyInfo countProperty = iListType.GetGenericInterface("System.Collections.Generic.ICollection").GetProperty("Count");
			Expression countExpr = targetObject.AppendPropertyReference(countProperty);
			int count = 0;
			try {
				// Do not get string representation since it can be printed in hex later
				Value countValue = countExpr.Evaluate(WindowsDebugger.CurrentProcess);
				count = (int)countValue.PrimitiveValue;
			} catch (GetValueException) {
				count = -1;
			}
			return count;
		}
	}
}
