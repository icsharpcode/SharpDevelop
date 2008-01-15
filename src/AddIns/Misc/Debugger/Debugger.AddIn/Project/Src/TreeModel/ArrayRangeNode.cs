// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.Core;

using Debugger;
using Debugger.Expressions;

namespace Debugger.AddIn.TreeModel
{
	public partial class Util
	{
		public static IEnumerable<AbstractNode> GetChildNodesOfArray(Expression expression, ArrayDimensions dimensions)
		{
			foreach(Expression childExpr in expression.AppendIndexers(dimensions)) {
				yield return Util.CreateNode(childExpr);
			}
		}
	}
	
	public class ArrayRangeNode
	{
	}
}
