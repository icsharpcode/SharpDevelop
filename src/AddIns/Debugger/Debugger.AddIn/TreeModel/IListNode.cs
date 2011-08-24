// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using Debugger.AddIn.Visualizers.Utils;
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
		Expression targetList;
		int listCount;
		
		public IListNode(TreeNode parent, Expression targetListObject)
			: base(parent)
		{
			this.targetList = targetListObject;
			
			this.Name = "IList";
			this.listCount = this.targetList.GetIListCount();
			this.childNodes = Utils.LazyGetItemsOfIList(this, this.targetList);
		}
		
		public override bool HasChildNodes {
			get { return this.listCount > 0; }
		}
	}
}
