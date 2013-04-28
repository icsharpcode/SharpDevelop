/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 20.04.2013
 * Time: 18:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ICSharpCode.Reporting.Exporter.Visitors
{
	/// <summary>
	/// Description of IAcceptor.
	/// </summary>
	public interface IAcceptor
	{
		 void Accept(IVisitor visitor);
	}
}
