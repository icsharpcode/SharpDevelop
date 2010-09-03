// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.CodeQualityAnalysis.Controls
{
    public abstract class Matrix<TValue>
    {
        public List<MatrixCell<TValue>> HeaderRows { get; set; }
        public List<MatrixCell<TValue>> HeaderColumns { get; set; }

        protected Matrix()
        {
            HeaderRows = new List<MatrixCell<TValue>>();
            HeaderColumns = new List<MatrixCell<TValue>>();
        }

        public abstract object EvaluateCell(MatrixCell<TValue> rowHeader, MatrixCell<TValue> columnHeader);
    }

    public class MatrixCell<TValue>
    {
        public TValue Value { get; set; }

        public MatrixCell(TValue value)
        {
            Value = value;
        }
    }
}
