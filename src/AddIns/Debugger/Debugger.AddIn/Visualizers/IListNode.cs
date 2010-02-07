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
	public class IListNode : TreeNode
	{
		Expression targetObject;
		int count;
		
		public IListNode(Expression targetObject)
		{
			this.targetObject = targetObject;
			
			this.Name = "IList";
			this.count = Utils.GetIListCount(this.targetObject);
			this.ChildNodes = Utils.LazyGetItemsOfIList(this.targetObject);
		}
		
		public override bool HasChildNodes {
			get { return this.count > 0; }
		}
	}
}
