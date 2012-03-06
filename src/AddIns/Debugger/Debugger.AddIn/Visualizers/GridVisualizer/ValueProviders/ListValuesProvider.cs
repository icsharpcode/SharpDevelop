// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Debugger.AddIn.Visualizers.Utils;
using Debugger.MetaData;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Visualizers.GridVisualizer
{
	/// <summary>
	///  Provides <see cref="ObjectValue"/>s for debugee objects implementing IList.
	/// </summary>
	public class ListValuesProvider : GridValuesProvider, IListValuesProvider<ObjectValue>
	{
		int? listCount = null;
		/// <summary>
		/// After evaluating how many items to clear debugger Expression cache,
		/// so that the cache does not keep too many PermanentReferences.
		/// </summary>
		static readonly int ClearCacheThreshold = 50;
		
		public ListValuesProvider(Expression targetObject, DebugType listItemType)
			:base(targetObject, listItemType)
		{
		}
		
		public int GetCount()
		{
			if (this.listCount == null) {
				this.listCount = this.targetObject.GetIListCount();
			}
			return this.listCount.Value;
		}
		
		/// <summary>When this reaches ClearCacheThreshold, the debugger Expression cache is cleared.</summary>
		int itemClearCacheCounter = 0;
		
		public ObjectValue GetItemAt(int index)
		{
			if (itemClearCacheCounter++ > ClearCacheThreshold) {
				// clear debugger Expression cache to avoid holding too many PermanentReferences
				WindowsDebugger.CurrentProcess.ClearExpressionCache();
				LoggingService.Info("Cleared debugger Expression cache.");
				itemClearCacheCounter = 0;
			}
			return ObjectValue.Create(
				targetObject.AppendIndexer(index).Evaluate(WindowsDebugger.CurrentProcess),
				//targetObject.AppendIndexer(index), // use Expression instead of value - possible only for IList though
				index,
				this.memberFromNameMap);
		}
	}
}
