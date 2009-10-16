// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.AvalonEdit.AddIn.Options
{
	[Serializable]
	public class CodeEditorOptions : TextEditorOptions
	{
		public static CodeEditorOptions Instance {
			get { return PropertyService.Get("CodeEditorOptions", new CodeEditorOptions()); }
		}
		
		// always support scrolling below the end of the document - it's better when folding is enabled
		[DefaultValue(true)]
		public override bool AllowScrollBelowDocument {
			get { return true; }
			set {
				if (value == false)
					throw new NotSupportedException();
			}
		}
	}
}
