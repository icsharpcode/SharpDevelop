// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.NRefactory.PrettyPrinter;
using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	/// <summary>
	/// Allows converting code snippets between C# and VB.
	/// This class isn't used by SharpDevelop itself (because it doesn't support projects).
	/// It works by creating a dummy project for the file to convert with a set of default references.
	/// </summary>
	public class CodeSnippetConverter
	{
		/// <summary>
		/// Project-wide imports to add to all files when converting VB to C#.
		/// </summary>
		public IList<string> DefaultImportsToAdd = new List<string> { "Microsoft.VisualBasic", "System", "System.Collections", "System.Collections.Generic", "System.Data", "System.Diagnostics" };
		
		/// <summary>
		/// Imports to remove (because they will become project-wide imports) when converting C# to VB.
		/// </summary>
		public IList<string> DefaultImportsToRemove = new List<string> { "Microsoft.VisualBasic", "System" };
		
		/// <summary>
		/// References project contents, for resolving type references during the conversion.
		/// </summary>
		public IList<IProjectContent> ReferencedContents = new List<IProjectContent>();
		
		DefaultProjectContent project;
		List<ISpecial> specials;
		CompilationUnit compilationUnit;
		ParseInformation parseInfo;
		
		#region Parsing
		INode Parse(SupportedLanguage sourceLanguage, string sourceCode, out string error)
		{
			project = new DefaultProjectContent();
			project.ReferencedContents.AddRange(ReferencedContents);
			if (sourceLanguage == SupportedLanguage.VBNet) {
				project.DefaultImports = new DefaultUsing(project);
				project.DefaultImports.Usings.AddRange(DefaultImportsToAdd);
			}
			SnippetParser parser = new SnippetParser(sourceLanguage);
			INode result = parser.Parse(sourceCode);
			error = parser.Errors.ErrorOutput;
			specials = parser.Specials;
			if (parser.Errors.Count != 0)
				return null;
			
			// now create a dummy compilation unit around the snippet result
			switch (parser.SnippetType) {
				case SnippetType.CompilationUnit:
					compilationUnit = (CompilationUnit)result;
					break;
				case SnippetType.Expression:
					compilationUnit = MakeCompilationUnitFromTypeMembers(
						MakeMethodFromBlock(
							MakeBlockFromExpression(
								(Expression)result
							)));
					break;
				case SnippetType.Statements:
					compilationUnit = MakeCompilationUnitFromTypeMembers(
						MakeMethodFromBlock(
							(BlockStatement)result
						));
					break;
				case SnippetType.TypeMembers:
					compilationUnit = MakeCompilationUnitFromTypeMembers(result.Children);
					break;
				default:
					throw new NotSupportedException("Unknown snippet type: " + parser.SnippetType);
			}
			
			// convert NRefactory CU in DOM CU
			NRefactoryASTConvertVisitor visitor = new NRefactoryASTConvertVisitor(project);
			visitor.VisitCompilationUnit(compilationUnit, null);
			visitor.Cu.FileName = sourceLanguage == SupportedLanguage.CSharp ? "a.cs" : "a.vb";
			
			// and register the compilation unit in the DOM
			foreach (IClass c in visitor.Cu.Classes) {
				project.AddClassToNamespaceList(c);
			}
			parseInfo = new ParseInformation();
			parseInfo.SetCompilationUnit(visitor.Cu);
			
			return result;
		}
		
		BlockStatement MakeBlockFromExpression(Expression expr)
		{
			return new BlockStatement {
				Children = {
					new ExpressionStatement(expr)
				},
				StartLocation = expr.StartLocation,
				EndLocation = expr.EndLocation
			};
		}
		
		INode[] MakeMethodFromBlock(BlockStatement block)
		{
			return new INode[] {
				new MethodDeclaration {
					Name = "DummyMethodForConversion",
					Body = block,
					StartLocation = block.StartLocation,
					EndLocation = block.EndLocation
				}
			};
		}
		
		CompilationUnit MakeCompilationUnitFromTypeMembers(IList<INode> members)
		{
			TypeDeclaration type = new TypeDeclaration(Modifiers.None, null) {
				Name = "DummyTypeForConversion",
				StartLocation = members[0].StartLocation,
				EndLocation = members[members.Count - 1].EndLocation
			};
			type.Children.AddRange(members);
			return new CompilationUnit {
				Children = {
					type
				}
			};
		}
		#endregion
		
		public string CSharpToVB(string input, out string errors)
		{
			INode node = Parse(SupportedLanguage.CSharp, input, out errors);
			if (node == null)
				return null;
			// apply conversion logic:
			compilationUnit.AcceptVisitor(
				new CSharpToVBNetConvertVisitor(project, parseInfo) {
					DefaultImportsToRemove = DefaultImportsToRemove,
				},
				null);
			PreprocessingDirective.CSharpToVB(specials);
			return CreateCode(node, new VBNetOutputVisitor());
		}
		
		public string VBToCSharp(string input, out string errors)
		{
			INode node = Parse(SupportedLanguage.VBNet, input, out errors);
			if (node == null)
				return null;
			// apply conversion logic:
			compilationUnit.AcceptVisitor(
				new VBNetToCSharpConvertVisitor(project, parseInfo),
				null);
			PreprocessingDirective.VBToCSharp(specials);
			return CreateCode(node, new CSharpOutputVisitor());
		}
		
		string CreateCode(INode node, IOutputAstVisitor outputVisitor)
		{
			using (SpecialNodesInserter.Install(specials, outputVisitor)) {
				node.AcceptVisitor(outputVisitor, null);
			}
			return outputVisitor.Text;
		}
	}
}
