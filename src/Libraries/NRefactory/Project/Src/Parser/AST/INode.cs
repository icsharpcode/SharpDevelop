// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public interface INode
	{
		INode Parent { 
			get;
			set;
		}
		
		ArrayList Children {
			get;
		}
		
		Point StartLocation {
			get;
			set;
		}
		
		Point EndLocation {
			get;
			set;
		}
		
		/// <summary>
		/// Visits all children
		/// </summary>
		/// <param name="visitor">The visitor to accept</param>
		/// <param name="data">Additional data for the visitor</param>
		/// <returns>The paremeter <paramref name="data"/></returns>
		object AcceptChildren(IASTVisitor visitor, object data);
		
		/// <summary>
		/// Accept the visitor
		/// </summary>
		/// <param name="visitor">The visitor to accept</param>
		/// <param name="data">Additional data for the visitor</param>
		/// <returns>The value the visitor returns after the visit</returns>
		object AcceptVisitor(IASTVisitor visitor, object data);
	}
}
