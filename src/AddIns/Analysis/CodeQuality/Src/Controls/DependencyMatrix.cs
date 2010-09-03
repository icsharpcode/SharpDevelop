// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.CodeQualityAnalysis.Controls
{
    public class DependencyMatrix : Matrix<INode>
    {
        public override object EvaluateCell(MatrixCell<INode> rowHeader, MatrixCell<INode> columnHeader)
        {
        	return rowHeader.Value.GetRelationship(columnHeader.Value);
        }
    }
}
