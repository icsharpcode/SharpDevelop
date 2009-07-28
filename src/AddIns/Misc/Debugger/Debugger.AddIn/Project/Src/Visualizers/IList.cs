// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.NRefactory.Ast;
using System.Collections;
using System.Collections.Generic;
using Debugger.MetaData;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.TreeModel
{
	public class IListNode: TreeNode
	{
		Expression targetObject;
		DebugType iListType;
		
		public IListNode(Expression targetObject, DebugType iListType)
		{
			this.targetObject = targetObject;
			this.iListType = iListType;
			
			this.Name = "IList";
			this.ChildNodes = LazyGetChildNodes();
		}
		
		public static WindowsDebugger WindowsDebugger {
			get {
				return (WindowsDebugger)DebuggerService.CurrentDebugger;
			}
		}
		
		IEnumerable<TreeNode> LazyGetChildNodes()
		{
			PropertyInfo countProperty = iListType.GetInterface(typeof(ICollection).FullName).GetProperty("Count");
			int count = 0;
			try {
				// Do not get string representation since it can be printed in hex
				Value list = targetObject.Evaluate(WindowsDebugger.DebuggedProcess);
				count = (int)list.GetPropertyValue(countProperty).PrimitiveValue;
			} catch (GetValueException) {
				yield break;
			}
			
			for(int i = 0; i < count; i++) {
				yield return new ExpressionNode(ExpressionNode.GetImageForArrayIndexer(), "[" + i + "]", targetObject.AppendIndexer(i));
			}
		}
	}
}
