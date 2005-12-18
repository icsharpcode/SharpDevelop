// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST
{
	
	///<summary>
	/// Charset types, used in external methods
	/// declarations (VB only).
	///</summary>
	public enum CharsetModifier
	{
		None,
		Auto,
		Unicode,
		ANSI
	}
	
	public class DeclareDeclaration : ParametrizedNode
	{
		string          alias = "";
		string          library = "";
		CharsetModifier charset = CharsetModifier.None;
		TypeReference   returnType = TypeReference.Null;
		
		public TypeReference TypeReference {
			get {
				return returnType;
			}
			set {
				returnType = TypeReference.CheckNull(value);
			}
		}
		
		public CharsetModifier Charset {
			get {
				return charset;
			}
			set {
				charset = value;
			}
		}
		
		public string Alias {
			get {
				return alias;
			}
			set {
				alias = value == null ? String.Empty : value;
			}
		}
		
		public string Library {
			get {
				return library;
			}
			set {
				library = value == null ? String.Empty : value;
			}
		}
		
		public DeclareDeclaration(string name, Modifier modifier, TypeReference returnType, List<ParameterDeclarationExpression> parameters, List<AttributeSection> attributes, string library, string alias, CharsetModifier charset) : base(modifier, attributes, name, parameters)
		{
			this.TypeReference = returnType;
			this.Library = library;
			this.Alias = alias;
			this.Charset = charset;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[DeclareDeclaration: Name = {0}, Modifier = {1}, ReturnType = {2}, Parameters = {3}, Charset = {4}, Alias = {5}, Library = {6}, Attributes = {7}]",
			                     Name,
			                     Modifier,
			                     TypeReference,
			                     GetCollectionString(Parameters),
			                     Charset,
			                     Alias,
			                     Library,
			                     GetCollectionString(Attributes));
		}
	}
}
