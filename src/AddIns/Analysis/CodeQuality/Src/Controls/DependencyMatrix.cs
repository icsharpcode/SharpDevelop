// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.CodeQualityAnalysis.Utility;

namespace ICSharpCode.CodeQualityAnalysis.Controls
{
    public class DependencyMatrix : Matrix<INode, Relationship>
    {
		protected override Relationship GetCellValue(int rowIndex, int columnIndex)
		{
			return HeaderRows[rowIndex].Value.GetRelationship(HeaderColumns[columnIndex].Value);
		}
    }
}
