// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
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
			DebugType debugListType;
			this.debugListExpression = DebuggerHelpers.CreateDebugListExpression(targetObject, itemType, out debugListType);
			this.ChildNodes = Utils.LazyGetItemsOfIList(this.debugListExpression);
		}
	}
}
