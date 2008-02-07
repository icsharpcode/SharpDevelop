// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// Provides code completion for attribute names.
	/// </summary>
	public class AttributesDataProvider : CtrlSpaceCompletionDataProvider
	{
		public AttributesDataProvider(IProjectContent pc)
			: this(ExpressionContext.Attribute)
		{
		}
		
		public AttributesDataProvider(ExpressionContext context) : base(context)
		{
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
			if (removeAttributeSuffix && data != null) {
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
