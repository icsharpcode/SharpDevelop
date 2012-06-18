//
// NamingHelper.cs
//
// Author:
//       Simon Lindgren <simon.n.lindgren@gmail.com>
//
// Copyright (c) 2012 Simon Lindgren
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Semantics;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	public class NamingHelper
	{
		ISet<string> UsedVariableNames;
		RefactoringContext context;

		public NamingHelper(RefactoringContext context)
		{
			this.context = context;
			var astNode = context.GetNode<Statement>();
			if (UsedVariableNames == null) {
				var visitor = new VariableFinderVisitor();
				astNode.AcceptVisitor(visitor);
				UsedVariableNames = visitor.VariableNames;
			}
		}

		public static IEnumerable<string> GenerateNameProposals(AstType type)
		{
			if (type is PrimitiveType) {
				var pt = (PrimitiveType)type;
				switch (pt.Keyword) {
					case "object":
						yield return "o";
						yield return "obj";
						break;
					case "bool":
						yield return "b";
						yield return "pred";
						break;
					case "double":
					case "float":
					case "decimal":
						yield return "d";
						yield return "f";
						yield return "m";
						break;
					case "char":
						yield return "c";
						break;
					default:
						yield return "i";
						yield return "j";
						yield return "k";
						break;
				}
				yield break;
			}
			string name;
			if (type is SimpleType) {
				name = ((SimpleType)type).Identifier;
			} else if (type is MemberType) {
				name = ((MemberType)type).MemberName;
			} else {
				yield break;
			}

			var names = WordParser.BreakWords(name);

			var possibleName = new StringBuilder();
			for (int i = 0; i < names.Count; i++) {
				possibleName.Length = 0;
				for (int j = i; j < names.Count; j++) {
					if (string.IsNullOrEmpty(names [j])) {
						continue;
					}
					if (j == i) { 
						names [j] = Char.ToLower(names [j] [0]) + names [j].Substring(1);
					}
					possibleName.Append(names [j]);
				}
				yield return possibleName.ToString();
			}
		}

		/// <summary>
		/// Generates a variable name for a variable of the specified type.
		/// </summary>
		/// <returns>
		/// The variable name.
		/// </returns>
		/// <param name='type'>
		/// The type of the variable.
		/// </param>
		public string GenerateVariableName(AstType type)
		{
			string firstSuggestion = null;
			foreach (var name in NamingHelper.GenerateNameProposals(type)) {
				firstSuggestion = firstSuggestion ?? name;
				if (NameIsUnused(name))
					return name;
			}
			// If we get here, all of the standard suggestions are already used.
			// This will at least be the second variable named based on firstSuggestion, so start at 2
			int counter = 2;
			string proposedName;
			do {
				proposedName = firstSuggestion + counter++;
			} while (!NameIsUnused(proposedName));
			return proposedName;
		}

		bool NameIsUnused(string name)
		{
			return !UsedVariableNames.Contains(name) && LookupVariable(name) == null;
		}

		/// <summary>
		/// Generates a variable name for a variable of the specified type.
		/// </summary>
		/// <returns>
		/// The variable name.
		/// </returns>
		/// <param name='type'>
		/// The type of the variable.
		/// </param>
		public string GenerateVariableName(IType type)
		{
			AstType astType = ToAstType(type);
			return GenerateVariableName(astType);
		}

		AstType ToAstType(IType type)
		{
			switch (type.FullName) {
				case "System.Object":
					return new PrimitiveType("object");
				case "System.String":
					return new PrimitiveType("string");
				case "System.Boolean":
					return new PrimitiveType("bool");
				case "System.Char":
					return new PrimitiveType("char");
				case "System.SByte":
					return new PrimitiveType("sbyte");
				case "System.Byte":
					return new PrimitiveType("byte");
				case "System.Int16":
					return new PrimitiveType("short");
				case "System.UInt16":
					return new PrimitiveType("ushort");
				case "System.Int32":
					return new PrimitiveType("int");
				case "System.UInt32":
					return new PrimitiveType("uint");
				case "System.Int64":
					return new PrimitiveType("long");
				case "System.UInt64":
					return new PrimitiveType("ulong");
				case "System.Single":
					return new PrimitiveType("float");
				case "System.Double":
					return new PrimitiveType("double");
				case "System.Decimal":
					return new PrimitiveType("decimal");
				default:
					return new SimpleType(type.Name);
			}
		}

		IVariable LookupVariable(string name)
		{
			var astNode = context.GetNode<BlockStatement>();
			var resolverState = context.Resolver.GetResolverStateAfter(astNode.LastChild.PrevSibling);
			var simpleNameRR = resolverState.ResolveSimpleName(name, new List<IType>()) as LocalResolveResult;
			if (simpleNameRR == null)
				return null;
			return simpleNameRR.Variable;
		}

		class VariableFinderVisitor : DepthFirstAstVisitor
		{

			public ISet<string> VariableNames = new HashSet<string>();

			public override void VisitVariableInitializer(VariableInitializer variableInitializer)
			{
				ProcessName(variableInitializer.Name);
				base.VisitVariableInitializer(variableInitializer);
			}

			public override void VisitQueryLetClause(QueryLetClause queryLetClause)
			{
				ProcessName(queryLetClause.Identifier);
				base.VisitQueryLetClause(queryLetClause);
			}

			public override void VisitQueryFromClause(QueryFromClause queryFromClause)
			{
				ProcessName(queryFromClause.Identifier);
				base.VisitQueryFromClause(queryFromClause);
			}

			public override void VisitQueryContinuationClause(QueryContinuationClause queryContinuationClause)
			{
				ProcessName(queryContinuationClause.Identifier);
				base.VisitQueryContinuationClause(queryContinuationClause);
			}

			void ProcessName(string name)
			{
				if (!VariableNames.Contains(name))
					VariableNames.Add(name);
			}
		}
	}
}

