// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using Debugger.AddIn.Visualizers.Utils;
using Debugger.MetaData;
using ICSharpCode.NRefactory.Ast;
using System.Reflection;

namespace Debugger.AddIn.Visualizers.GridVisualizer
{
	/// <summary>
	/// Provides <see cref="ObjectValue"/>s to be displayed in Grid visualizer.
	/// Descandants implement getting values for IList and IEnumerable.
	/// </summary>
	public class GridValuesProvider
	{
		/// <summary> Used to quickly find MemberInfo by member name - DebugType.GetMember(name) uses a loop to search members </summary>
		protected Dictionary<string, MemberInfo> memberFromNameMap;
		
		protected Expression targetObject;
		protected DebugType itemType;
		
		public GridValuesProvider(Expression targetObject, DebugType itemType)
		{
			this.targetObject = targetObject;
			this.itemType = itemType;
			
			this.memberFromNameMap = this.GetItemTypeMembers().MakeDictionary(memberInfo => memberInfo.Name);
		}
		
		/// <summary>
		/// Gets members that will be displayed as columns.
		/// </summary>
		public IList<MemberInfo> GetItemTypeMembers()
		{
			var publicPropetiesAndFields = itemType.GetFieldsAndNonIndexedProperties(BindingFlags.Public | BindingFlags.Instance);
			return publicPropetiesAndFields;
		}
	}
}
