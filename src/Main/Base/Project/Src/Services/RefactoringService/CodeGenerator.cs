/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 22.10.2005
 * Time: 14:41
 */

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Provides code generation facilities.
	/// </summary>
	public abstract class CodeGenerator
	{
		protected TypeReference ConvertType(IReturnType returnType)
		{
			if (returnType == null)           return TypeReference.Null;
			if (returnType is NullReturnType) return TypeReference.Null;
			
			TypeReference typeRef = new TypeReference(returnType.FullyQualifiedName);
			while (returnType is ArrayReturnType) {
				ArrayReturnType art = (ArrayReturnType)returnType;
				int[] rank = typeRef.RankSpecifier ?? new int[0];
				Array.Resize(ref rank, rank.Length + 1);
				rank[rank.Length - 1] = art.ArrayDimensions;
				typeRef.RankSpecifier = rank;
				returnType = art.ElementType;
			}
			if (returnType is ConstructedReturnType) {
				ConstructedReturnType rt = (ConstructedReturnType)returnType;
				foreach (IReturnType typeArgument in rt.TypeArguments) {
					typeRef.GenericTypes.Add(ConvertType(typeArgument));
				}
			}
			return typeRef;
		}
		
		protected Modifier ConvertModifier(ModifierEnum m)
		{
			return (Modifier)m;
		}
		
		public virtual string GetPropertyName(string fieldName)
		{
			if (fieldName.StartsWith("_") && fieldName.Length > 1)
				return Char.ToUpper(fieldName[1]) + fieldName.Substring(2);
			else if (fieldName.StartsWith("m_") && fieldName.Length > 2)
				return Char.ToUpper(fieldName[2]) + fieldName.Substring(3);
			else
				return Char.ToUpper(fieldName[0]) + fieldName.Substring(1);
		}
		
		public virtual void CreateProperty(IField field, IDocument document, bool createGetter, bool createSetter)
		{
			string name = GetPropertyName(field.Name);
			PropertyDeclaration property = new PropertyDeclaration(name,
			                                                       ConvertType(field.ReturnType),
			                                                       ConvertModifier(field.Modifiers), null);
			if (createGetter) {
				BlockStatement block = new BlockStatement();
				block.AddChild(new ReturnStatement(new IdentifierExpression(field.Name)));
				property.GetRegion = new PropertyGetRegion(block, null);
			}
			if (createSetter) {
				BlockStatement block = new BlockStatement();
				Expression left = new IdentifierExpression(field.Name);
				Expression right = new IdentifierExpression("value");
				block.AddChild(new StatementExpression(new AssignmentExpression(left, AssignmentOperatorType.Assign, right)));
				property.SetRegion = new PropertySetRegion(block, null);
			}
			
			InsertCodeAfter(field, document, property);
		}
		
		/// <summary>
		/// Generates code for <paramref name="node"/> and inserts it into <paramref name="document"/>
		/// after <paramref name="position"/>.
		/// </summary>
		public virtual void InsertCodeAfter(IMember position, IDocument document, AbstractNode node)
		{
			int insertLine = position.Region.EndLine;
			LineSegment lineSegment = document.GetLineSegment(insertLine - 1);
			string lineText = document.GetText(lineSegment.Offset, lineSegment.Length);
			string indentation = lineText.Substring(0, lineText.Length - lineText.TrimStart().Length);
			// insert one line below field (text editor uses different coordinates)
			lineSegment = document.GetLineSegment(insertLine);
			document.Insert(lineSegment.Offset, GenerateCode(node, indentation));
		}
		
		/// <summary>
		/// Generates code for the NRefactory node.
		/// </summary>
		public abstract string GenerateCode(AbstractNode node, string indentation);
	}
}
