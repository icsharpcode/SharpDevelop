// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.SharpDevelop.Refactoring;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class OverrideCompletionDataProvider : AbstractCompletionDataProvider
	{
		public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			ParseInformation parseInfo = ParserService.GetParseInformation(fileName);
			if (parseInfo == null) return null;
			IClass c = parseInfo.MostRecentCompilationUnit.GetInnermostClass(textArea.Caret.Line, textArea.Caret.Column);
			if (c == null) return null;
			List<ICompletionData> result = new List<ICompletionData>();
			foreach (IMethod m in c.DefaultReturnType.GetMethods()) {
				if (m.IsPublic || m.IsProtected) {
					if (m.IsAbstract || m.IsVirtual || m.IsOverride) {
						if (!m.IsSealed && !m.IsConst) {
							if (m.DeclaringType.FullyQualifiedName != c.FullyQualifiedName) {
								result.Add(new OverrideCompletionData(m));
							}
						}
					}
				}
			}
			foreach (IProperty m in c.DefaultReturnType.GetProperties()) {
				if (m.IsPublic || m.IsProtected) {
					if (m.IsAbstract || m.IsVirtual || m.IsOverride) {
						if (!m.IsSealed && !m.IsConst) {
							if (m.DeclaringType.FullyQualifiedName != c.FullyQualifiedName) {
								result.Add(new OverrideCompletionData(m));
							}
						}
					}
				}
			}
			return result.ToArray();
		}
	}
	
	public class OverrideCompletionData : DefaultCompletionData
	{
		IMember member;
		
		static string GetName(IMethod method, ConversionFlags flags)
		{
			AmbienceService.CurrentAmbience.ConversionFlags = flags | ConversionFlags.ShowParameterNames;
			return AmbienceService.CurrentAmbience.Convert(method);
		}
		
		public OverrideCompletionData(IMethod method)
			: base(GetName(method, ConversionFlags.None),
			       "override " + GetName(method, ConversionFlags.ShowReturnType
			                             | ConversionFlags.ShowAccessibility)
			       + "\n\n" + method.Documentation,
			       ClassBrowserIconService.GetIcon(method))
		{
			this.member = method;
		}
		
		public OverrideCompletionData(IProperty property)
			: base(property.Name, "override " + property.Name + "\n\n" + property.Documentation,
			       ClassBrowserIconService.GetIcon(property))
		{
			this.member = property;
		}
		
		public override bool InsertAction(TextArea textArea, char ch)
		{
			ClassFinder context = new ClassFinder(textArea.MotherTextEditorControl.FileName,
			                                      textArea.Caret.Line + 1, textArea.Caret.Column + 1);
			int caretPosition = textArea.Caret.Offset;
			LineSegment line = textArea.Document.GetLineSegment(textArea.Caret.Line);
			string lineText = textArea.Document.GetText(line.Offset, caretPosition - line.Offset);
			foreach (char c in lineText) {
				if (!char.IsWhiteSpace(c) && !char.IsLetterOrDigit(c)) {
					return base.InsertAction(textArea, ch);
				}
			}
			string indentation = lineText.Substring(lineText.Length - lineText.TrimStart().Length);
			
			CodeGenerator codeGen = ParserService.CurrentProjectContent.Language.CodeGenerator;
			
			string text = codeGen.GenerateCode(GetOverrideAST(member, context), indentation);
			text = text.TrimEnd(); // remove newline from end
			textArea.Document.Replace(line.Offset, caretPosition - line.Offset, text);
			
			int endPos = line.Offset + text.Length;
			int endLine = textArea.Document.GetLineNumberForOffset(endPos);
			line = textArea.Document.GetLineSegment(endLine);
			textArea.MotherTextAreaControl.JumpTo(endLine, endPos - line.Offset);
			textArea.Refresh();
			return true;
		}
		
		AttributedNode GetOverrideAST(IMember member, ClassFinder targetContext)
		{
			AttributedNode node = CodeGenerator.ConvertMember(member, targetContext);
			node.Modifier &= ~(Modifier.Virtual | Modifier.Abstract);
			node.Modifier |= Modifier.Override;
			
			MethodDeclaration method = node as MethodDeclaration;
			if (method != null) {
				method.Body.Children.Clear();
				method.Body.AddChild(new ReturnStatement(CreateMethodCall(method)));
			}
			PropertyDeclaration property = node as PropertyDeclaration;
			if (property != null) {
				Expression field = new FieldReferenceExpression(new BaseReferenceExpression(),
				                                                property.Name);
				if (!property.GetRegion.Block.IsNull) {
					property.GetRegion.Block.Children.Clear();
					property.GetRegion.Block.AddChild(new ReturnStatement(field));
				}
				if (!property.SetRegion.Block.IsNull) {
					property.SetRegion.Block.Children.Clear();
					Expression expr = new AssignmentExpression(field,
					                                           AssignmentOperatorType.Assign,
					                                           new IdentifierExpression("value"));
					property.SetRegion.Block.AddChild(new StatementExpression(expr));
				}
			}
			return node;
		}
		
		static InvocationExpression CreateMethodCall(MethodDeclaration method)
		{
			Expression methodName = new FieldReferenceExpression(new BaseReferenceExpression(),
			                                                     method.Name);
			InvocationExpression ie = new InvocationExpression(methodName, null);
			foreach (ParameterDeclarationExpression param in method.Parameters) {
				Expression expr = new IdentifierExpression(param.ParameterName);
				if (param.ParamModifier == ParamModifier.Ref) {
					expr = new DirectionExpression(FieldDirection.Ref, expr);
				} else if (param.ParamModifier == ParamModifier.Out) {
					expr = new DirectionExpression(FieldDirection.Out, expr);
				}
				ie.Arguments.Add(expr);
			}
			return ie;
		}
	}
}
