// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System.Collections;
using System.Collections.Generic;
using Debugger.Expressions;
using Debugger.MetaData;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;

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
		
		public static WindowsDebugger WindowsDebugger {
			get {
				return (WindowsDebugger)DebuggerService.CurrentDebugger;
			}
		}
		
		IEnumerable<AbstractNode> GetChildNodes()
		{
			PropertyInfo countProperty = iListType.GetInterface(typeof(ICollection).FullName).GetProperty("Count");
			Expression countExpr = targetObject.AppendPropertyReference(countProperty);
			int count = 0;
			try {
				// Do not get string representation since it can be printed in hex
				Value countValue = countExpr.Evaluate(WindowsDebugger.DebuggedProcess.SelectedStackFrame);
				count = (int)countValue.PrimitiveValue;
			} catch (GetValueException) {
				count = -1;
			}
			if (count == -1) {
				yield return ValueNode.Create(countExpr);
				yield break;
			}
			
			for(int i = 0; i < count; i++) {
				PropertyInfo itemProperty = iListType.GetProperty("Item");
				Expression itemExpr = targetObject.AppendMemberReference(itemProperty, new PrimitiveExpression(i));
				yield return ValueNode.Create(itemExpr);
			}
		}
	}
}
