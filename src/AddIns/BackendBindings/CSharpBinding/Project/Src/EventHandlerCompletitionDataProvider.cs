// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version>$Revision: 321 $</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using ICSharpCode.Core;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;

namespace CSharpBinding
{
	public class EventHandlerCompletitionDataProvider : AbstractCompletionDataProvider
	{
		string expression;
		ResolveResult resolveResult;
		IClass resolvedClass;
		
		public EventHandlerCompletitionDataProvider(string expression, ResolveResult resolveResult)
		{
			this.expression = expression;
			this.resolveResult = resolveResult;
			this.resolvedClass = resolveResult.ResolvedType.GetUnderlyingClass();
		}
		
		/// <summary>
		/// Generates the completion data. This method is called by the text editor control.
		/// </summary>
		public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			ArrayList completionData = new ArrayList();
			completionData.Add(new DelegateCompletionData("new " + resolveResult.ResolvedType.Name + "();", 2,
			                                              "delegate " + resolvedClass.FullyQualifiedName + "\n" + CodeCompletionData.GetDocumentation(resolvedClass.Documentation)));
			completionData.Add(new DelegateCompletionData("delegate {  };", 3,
			                                              "${res:CSharpBinding.InsertAnonymousMethod}"));
			CSharpAmbience ambience = new CSharpAmbience();
			ambience.ConversionFlags = ConversionFlags.ShowParameterNames;
			IMethod invoke = resolvedClass.SearchMember("Invoke", LanguageProperties.CSharp) as IMethod;
			if (invoke != null) {
				StringBuilder builder = new StringBuilder("delegate(");
				for (int i = 0; i < invoke.Parameters.Count; ++i) {
					if (i > 0) {
						builder.Append(", ");
					}
					builder.Append(ambience.Convert(invoke.Parameters[i]));
				}
				builder.Append(") {  };");
				completionData.Add(new DelegateCompletionData(builder.ToString(), 3,
				                                              "${res:CSharpBinding.InsertAnonymousMethodWithParameters}"));
				IClass callingClass = resolveResult.CallingClass;
				IClass eventReturnType = invoke.ReturnType.GetUnderlyingClass();
				IClass[] eventParameters = new IClass[invoke.Parameters.Count];
				for (int i = 0; i < eventParameters.Length; i++) {
					eventParameters[i] = invoke.Parameters[i].ReturnType.GetUnderlyingClass();
					if (eventParameters[i] == null) {
						eventReturnType = null;
						break;
					}
				}
				if (callingClass != null && eventReturnType != null) {
					bool inStatic = false;
					if (resolveResult.CallingMember != null)
						inStatic = resolveResult.CallingMember.IsStatic;
					foreach (IMethod method in callingClass.DefaultReturnType.GetMethods()) {
						if (inStatic && !method.IsStatic)
							continue;
						if (!method.IsAccessible(callingClass, true))
							continue;
						if (method.Parameters.Count != invoke.Parameters.Count)
							continue;
						// check return type compatibility:
						IClass c2 = method.ReturnType.GetUnderlyingClass();
						if (c2 == null || !c2.IsTypeInInheritanceTree(eventReturnType))
							continue;
						bool ok = true;
						for (int i = 0; i < eventParameters.Length; i++) {
							c2 = method.Parameters[i].ReturnType.GetUnderlyingClass();
							if (c2 == null || !eventParameters[i].IsTypeInInheritanceTree(c2)) {
								ok = false;
								break;
							}
						}
						if (ok) {
							completionData.Add(new CodeCompletionData(method));
						}
					}
				}
			}
			return (ICompletionData[])completionData.ToArray(typeof(ICompletionData));
		}
		
		private class DelegateCompletionData : DefaultCompletionData
		{
			int cursorOffset;
			
			public DelegateCompletionData(string text, int cursorOffset, string documentation)
				: base(text, StringParser.Parse(documentation), ClassBrowserIconService.DelegateIndex)
			{
				this.cursorOffset = cursorOffset;
			}
			
			public override bool InsertAction(TextArea textArea, char ch)
			{
				bool r = base.InsertAction(textArea, ch);
				textArea.Caret.Column -= cursorOffset;
				return r;
			}
		}
	}
}
