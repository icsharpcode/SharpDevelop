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
		
		public ListValuesProvider(Expression targetObject, DebugType listItemType)
			:base(targetObject, listItemType)
		{
		}
		
		public int GetCount()
		{
			if (!countEvaluated)
			{
				this.count = Debugger.AddIn.TreeModel.Utils.GetIListCount(this.targetObject);
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
	}
}
