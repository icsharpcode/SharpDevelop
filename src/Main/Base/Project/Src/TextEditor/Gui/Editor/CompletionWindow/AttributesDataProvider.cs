// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// Provides code completion for attribute names.
	/// </summary>
	public class AttributesDataProvider : CtrlSpaceCompletionDataProvider
	{
		public AttributesDataProvider()
			: this(ExpressionContext.TypeDerivingFrom(ProjectContentRegistry.Mscorlib.GetClass("System.Attribute"), true))
		{
		}
		
		public AttributesDataProvider(ExpressionContext context) : base(context)
		{
			this.ForceNewExpression = true;
		}
		
		bool removeAttributeSuffix = true;
		
		public bool RemoveAttributeSuffix {
			get {
				return removeAttributeSuffix;
			}
			set {
				removeAttributeSuffix = value;
			}
		}
		
		public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			ICompletionData[] data = base.GenerateCompletionData(fileName, textArea, charTyped);
			if (removeAttributeSuffix) {
				foreach (ICompletionData d in data) {
					if (d.Text.EndsWith("Attribute")) {
						d.Text = d.Text.Substring(0, d.Text.Length - 9);
					}
				}
			}
			return data;
		}
	}
}
