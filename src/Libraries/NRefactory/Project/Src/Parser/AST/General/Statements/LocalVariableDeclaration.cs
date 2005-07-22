// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class LocalVariableDeclaration : Statement
	{
		TypeReference             typeReference;
		Modifier                  modifier = Modifier.None;
		List<VariableDeclaration> variables = new List<VariableDeclaration>(1);
		BlockStatement            block     = BlockStatement.Null; // the block in witch the variable is declared; needed for the LookupTable
		
		public TypeReference TypeReference {
			get {
				return typeReference;
			}
			set {
				typeReference = TypeReference.CheckNull(value);
			}
		}
		
		public Modifier Modifier {
			get {
				return modifier;
			}
			set {
				modifier = value;
			}
		}
		
		public List<VariableDeclaration> Variables {
			get {
				return variables;
			}
		}
		
		public BlockStatement Block {
			get {
				return block;
			}
			set {
				Debug.Assert(value != null);
				block = value;
			}
		}
		
		public TypeReference GetTypeForVariable(int variableIndex)
		{
			if (!typeReference.IsNull) {
				return typeReference;
			}
			
			for (int i = variableIndex; i < Variables.Count;++i) {
				if (!((VariableDeclaration)Variables[i]).TypeReference.IsNull) {
					return ((VariableDeclaration)Variables[i]).TypeReference;
				}
			}
			return null;
		}
		
		public LocalVariableDeclaration(VariableDeclaration declaration) : this(TypeReference.Null)
		{
			Variables.Add(declaration);
		}
		
		public LocalVariableDeclaration(TypeReference typeReference)
		{
			this.TypeReference = typeReference;
		}
		
		public LocalVariableDeclaration(TypeReference typeReference, Modifier modifier)
		{
			this.TypeReference = typeReference;
			this.modifier      = modifier;
		}
		
		public LocalVariableDeclaration(Modifier modifier)
		{
			this.typeReference = TypeReference.Null;
			this.modifier      = modifier;
		}
		
		public VariableDeclaration GetVariableDeclaration(string variableName)
		{
			foreach (VariableDeclaration variableDeclaration in variables) {
				if (variableDeclaration.Name == variableName) {
					return variableDeclaration;
				}
			}
			return null;
		}
				
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[LocalVariableDeclaration: Type={0}, Modifier ={1} Variables={2}]", 
			                     typeReference, 
			                     modifier, 
			                     GetCollectionString(variables));
		}
	}
}
