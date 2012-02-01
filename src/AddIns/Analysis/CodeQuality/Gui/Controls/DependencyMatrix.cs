// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICSharpCode.CodeQuality.Engine.Dom;

namespace ICSharpCode.CodeQuality.Gui
{
	public class DependencyMatrix : VisibleMatrix<NodeBase, Tuple<int, int>>
	{
		protected override Tuple<int, int> GetCellValue(int rowIndex, int columnIndex)
		{
			int a = HeaderRows[rowIndex].Value.GetUses(HeaderColumns[columnIndex].Value);
			int b = HeaderColumns[columnIndex].Value.GetUses(HeaderRows[rowIndex].Value);
			return Tuple.Create(a, b);
		}
	}
}
