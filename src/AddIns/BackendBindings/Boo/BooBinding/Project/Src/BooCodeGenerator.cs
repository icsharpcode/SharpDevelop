// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;

using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Ast.Visitors;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using NRefactoryToBooConverter;

namespace Grunwald.BooBinding
{
	/// <summary>
	/// Description of BooCodeGenerator.
	/// </summary>
	public class BooCodeGenerator : CodeGenerator
	{
		public override void InsertCodeAtEnd(DomRegion region, IRefactoringDocument document, params AbstractNode[] nodes)
		{
			InsertCodeAfter(region.EndLine, document,
			                GetIndentation(document, region.BeginLine) + '\t', nodes);
		}
		
		public override string GenerateCode(AbstractNode node, string indentation)
		{
			StringBuilder errorBuilder = new StringBuilder();
			ConverterSettings settings = new ConverterSettings("codegeneration.cs");
			string output = null;
			
			Node booNode = (Node)node.AcceptVisitor(new ConvertVisitor(settings), null);
			
			if (settings.Errors.Count > 0) {
				foreach (CompilerError error in settings.Errors) {
					errorBuilder.AppendLine(error.ToString());
				}
			} else {
				if (settings.Warnings.Count > 0) {
					foreach (CompilerWarning warning in settings.Warnings) {
						errorBuilder.AppendLine(warning.ToString());
					}
				}
				booNode.Accept(new RemoveRedundantTypeReferencesVisitor());
				using (StringWriter w = new StringWriter()) {
					BooPrinterVisitor printer = new BooPrinterVisitor(w);
					int indentCount = 0;
					foreach (char c in indentation) {
						if (c == '\t')
							indentCount += 4;
						else
							indentCount += 1;
					}
					indentCount /= 4;
					while (indentCount-- > 0)
						printer.Indent();
					booNode.Accept(printer);
					output = w.ToString();
				}
			}
			if (errorBuilder.Length > 0) {
				MessageService.ShowMessage(errorBuilder.ToString());
			}
			return output;
		}
		
		public static readonly BooCodeGenerator Instance = new BooCodeGenerator();
	}
}
