// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Ast;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// NRefactory Expression plus a method that can evaluate it, 
	/// as the Debugger does not support evaluating NRefactory expressions directly.
	/// </summary>
	public class GraphExpression
	{
		public String Expr { get; set; }
		public Func<Value> GetValue { get; set; }
		
		public GraphExpression(String expr, Func<Value> getValue)
		{
			if (String.IsNullOrEmpty(expr)) throw new ArgumentNullException("expr");
			if (getValue == null) throw new ArgumentNullException("getValue");
			this.Expr = expr;
			this.GetValue = getValue;
		}
	}
}
