// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using Debugger.AddIn.Visualizers.Utils;
using Debugger.MetaData;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Debugging;

namespace Debugger.AddIn.TreeModel
{
	/// <summary>
	/// IEnumerable node in the variable tree.
	/// </summary>
	public class IEnumerableNode : TreeNode
	{
		Expression targetObject;
		Expression debugListExpression;
		
		public IEnumerableNode(TreeNode parent, Expression targetObject, DebugType itemType)
			: base(parent)
		{
			this.targetObject = targetObject;
			
			this.Name = "IEnumerable";
			this.Text = "Expanding will enumerate the IEnumerable";
			DebugType debugListType;
			this.debugListExpression = DebuggerHelpers.CreateDebugListExpression(targetObject, itemType, out debugListType);
			this.childNodes = Utils.LazyGetItemsOfIList(this, this.debugListExpression);
		}
	}
}
