// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using Debugger.AddIn.TreeModel;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class TextNode : TreeNode, ISetText
	{
		public TextNode(string text, SupportedLanguage language)
		{
			this.Name = text;
			this.Language = language;
		}
		
		public override bool CanSetText {
			get {
				return true;
			}
		}
		
		public override bool SetText(string text)
		{
			this.Text = text;
			return true;
		}
		
		public bool SetName(string name)
		{
			this.Name = name;
			return true;
		}
		
		public SupportedLanguage Language { get; set; }
	}
	
	public class ErrorInfoNode : ICorDebug.InfoNode
	{
		public ErrorInfoNode(string name, string text) : base(name, text)
		{
			IconImage = DebuggerResourceService.GetImage("Icons.16x16.Error");
		}
	}
}
