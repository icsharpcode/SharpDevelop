// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of IExpression.
	/// </summary>
	public interface IReportExpression
	{
		string Expression {get;set;}
		string Text {get;set;}
	}
}
