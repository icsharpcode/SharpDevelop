// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class OverrideCompletionDataProvider : AbstractCompletionDataProvider
	{
		static IEnumerable<IMember> GetOverridableMembers(IClass c)
		{
			if (c == null) {
				throw new ArgumentException("c");
			}
			
			return MemberLookupHelper.GetAccessibleMembers(c.BaseType, c, c.ProjectContent.Language, true)
				.Where(m => m.IsOverridable && !m.IsConst);
		}
		
		/// <summary>
		/// Gets a list of overridable methods from the specified class.
		/// A better location for this method is in the DefaultClass
		/// class and the IClass interface.
		/// </summary>
		public static IMethod[] GetOverridableMethods(IClass c)
		{
			return GetOverridableMembers(c).OfType<IMethod>().ToArray();
		}
		
		/// <summary>
		/// Gets a list of overridable properties from the specified class.
		/// </summary>
		public static IProperty[] GetOverridableProperties(IClass c)
		{
			return GetOverridableMembers(c).OfType<IProperty>().ToArray();
		}
		
		public override CompletionDataProviderKeyResult ProcessKey(char key)
		{
			if (key == '(')
				return CompletionDataProviderKeyResult.NormalKey;
			else
				return base.ProcessKey(key);
		}
		
		public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			ParseInformation parseInfo = ParserService.GetParseInformation(fileName);
			if (parseInfo == null) return null;
			IClass c = parseInfo.MostRecentCompilationUnit.GetInnermostClass(textArea.Caret.Line, textArea.Caret.Column);
			if (c == null) return null;
			List<ICompletionData> result = new List<ICompletionData>();
			foreach (IMethod m in GetOverridableMethods(c)) {
				result.Add(new OverrideCompletionData(m));
			}
			foreach (IProperty p in GetOverridableProperties(c)) {
				result.Add(new OverrideCompletionData(p));
			}
			return result.ToArray();
		}
	}
	
	public class OverrideCompletionData : DefaultCompletionData
	{
		IMember member;
		
		static string GetName(IMember member, ConversionFlags flags)
		{
			IAmbience ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = flags | ConversionFlags.ShowParameterNames | ConversionFlags.ShowTypeParameterList;
			return ambience.Convert(member);
		}
		
		public OverrideCompletionData(IMethod method)
			: base(GetName(method, ConversionFlags.ShowParameterList),
			       ClassBrowserIconService.GetIcon(method))
		{
			this.member = method;
		}
		
		public OverrideCompletionData(IProperty property)
			: base(property.Name, "override " + property.Name + "\n\n" + CodeCompletionData.ConvertDocumentation(property.Documentation),
			       ClassBrowserIconService.GetIcon(property))
		{
			this.member = property;
		}
		
		public override string Description {
			get {
				return "override " + GetName(member, ConversionFlags.ShowReturnType
				                             | ConversionFlags.ShowParameterList
				                             | ConversionFlags.ShowAccessibility)
					+ "\n\n" + CodeCompletionData.ConvertDocumentation(member.Documentation);
			}
		}
		
		public override bool InsertAction(TextArea textArea, char ch)
		{
			ClassFinder context = new ClassFinder(ParserService.GetParseInformation(textArea.MotherTextEditorControl.FileName),
			                                      textArea.Caret.Line + 1, textArea.Caret.Column + 1);
			int caretPosition = textArea.Caret.Offset;
			LineSegment line = textArea.Document.GetLineSegment(textArea.Caret.Line);
			string lineText = textArea.Document.GetText(line.Offset, caretPosition - line.Offset);
			foreach (char c in lineText) {
				if (!char.IsWhiteSpace(c) && !char.IsLetterOrDigit(c)) {
					return base.InsertAction(textArea, ch);
				}
			}
			string indentation = lineText.Substring(0, lineText.Length - lineText.TrimStart().Length);
			
			CodeGenerator codeGen = ParserService.CurrentProjectContent.Language.CodeGenerator;
			
			string text = codeGen.GenerateCode(codeGen.GetOverridingMethod(member, context), indentation);
			text = text.TrimEnd(); // remove newline from end
			textArea.Document.Replace(line.Offset, caretPosition - line.Offset, text);
			
			int endPos = line.Offset + text.Length;
			int endLine = textArea.Document.GetLineNumberForOffset(endPos);
			line = textArea.Document.GetLineSegment(endLine);
			textArea.MotherTextAreaControl.JumpTo(endLine, endPos - line.Offset);
			textArea.Refresh();
			return true;
		}
	}
}
