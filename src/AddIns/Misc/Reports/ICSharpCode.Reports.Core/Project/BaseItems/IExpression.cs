/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 25.07.2010
 * Time: 14:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
