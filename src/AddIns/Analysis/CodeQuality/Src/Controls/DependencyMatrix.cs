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
            return rowHeader.Value == columnHeader.Value; // TODO: Add a new method to INode Uses(INode -> Boolean) or maybe Relatioship(INode -> Relatioship)
        }
    }
}
