// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version>$Revision: 321 $</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
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
		
		public EventHandlerCompletitionDataProvider(string expression, ResolveResult resolveResult)
		{
			this.expression = expression;
			this.resolveResult = resolveResult;
		}
		
		/// <summary>
		/// Generates the completion data. This method is called by the text editor control.
		/// </summary>
		public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			ArrayList completionData = new ArrayList();
			completionData.Add(new DefaultCompletionData("new " + resolveResult.ResolvedType.FullyQualifiedName + "()", "Event handler", ClassBrowserIconService.DelegateIndex));
			return (ICompletionData[])completionData.ToArray(typeof(ICompletionData));
		}
	}
}
