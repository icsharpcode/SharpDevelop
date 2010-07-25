using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace ICSharpCode.CodeQualityAnalysis.Controls
{
    public class DependencyMatrixControl : MatrixControl<INode>
    {
    	static DependencyMatrixControl()
    	{
    		DefaultStyleKeyProperty.OverrideMetadata(
    			typeof(DependencyMatrixControl),
    			new FrameworkPropertyMetadata(typeof(DependencyMatrixControl)));
    	}
    }
}
