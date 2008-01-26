// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

using Debugger.Expressions;
using Debugger.MetaData;

namespace Debugger.AddIn.TreeModel
{
	public class IListNode: AbstractNode
	{
		Expression targetObject;
		DebugType iListType;
		
		public IListNode(Expression targetObject, DebugType iListType)
		{
			this.targetObject = targetObject;
			this.iListType = iListType;
			
			this.Name = "IList";
			this.ChildNodes = GetChildNodes();
		}
		
		IEnumerable<AbstractNode> GetChildNodes()
		{
			PropertyInfo countProperty = iListType.GetInterface(typeof(ICollection).FullName).GetProperty("Count");
			Expression countExpr = targetObject.AppendPropertyReference(countProperty);
			AbstractNode countNode = ValueNode.Create(countExpr);
			int count = 0;
			if (countNode is ValueNode) {
				count = int.Parse(countNode.Text);
			} else {
				yield return countNode;
			}
			
			for(int i = 0; i < count; i++) {
				PropertyInfo itemProperty = iListType.GetProperty("Item");
				Expression itemExpr = targetObject.AppendMemberReference(itemProperty, new PrimitiveExpression(i));
				yield return ValueNode.Create(itemExpr);
			}
		}
	}
}
