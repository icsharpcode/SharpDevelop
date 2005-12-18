// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class UsingStatement : StatementWithEmbeddedStatement
	{
		Statement  resourceAcquisition;
		
		public Statement ResourceAcquisition {
			get {
				return resourceAcquisition;
			}
			set {
				resourceAcquisition = Statement.CheckNull(value);
			}
		}
		
		public UsingStatement(Statement resourceAcquisition, Statement embeddedStatement)
		{
			this.ResourceAcquisition = resourceAcquisition;
			this.EmbeddedStatement   = embeddedStatement;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[UsingStatement: ResourceAcquisition={0}, EmbeddedStatement={1}]", 
			                     resourceAcquisition,
			                     EmbeddedStatement);
		}
	}
}
