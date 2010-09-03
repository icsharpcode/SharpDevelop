// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using Debugger.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using Debugger.AddIn.Visualizers.Utils;
using ICSharpCode.NRefactory.Ast;

namespace Debugger.AddIn.TreeModel
{
	/// <summary>
	/// IEnumerable node in the variable tree.
	/// </summary>
	public class IEnumerableNode : TreeNode
	{
		Expression targetObject;
		Expression debugListExpression;
		
		public IEnumerableNode(Expression targetObject, DebugType itemType)
		{
			this.targetObject = targetObject;
			
			this.Name = "IEnumerable";
			this.Text = "Expanding will enumerate the IEnumerable";
			DebugType debugListType;
			this.debugListExpression = DebuggerHelpers.CreateDebugListExpression(targetObject, itemType, out debugListType);
			this.ChildNodes = Utils.LazyGetItemsOfIList(this.debugListExpression);
		}
	}
}
