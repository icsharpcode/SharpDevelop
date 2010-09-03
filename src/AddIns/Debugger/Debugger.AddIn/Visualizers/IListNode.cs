// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

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
