// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

using ICSharpCode.FormsDesigner;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace RubyBinding.Tests.Utils
{
	/// <summary>
	/// Gives access to various protected methods
	/// of the RubyDesignerGenerator class for testing.
	/// </summary>
	public class DerivedRubyDesignerGenerator : RubyDesignerGenerator
	{
		ParseInformation parseInfoToReturnFromParseFile;

		public DerivedRubyDesignerGenerator() : this(new MockTextEditorOptions())
		{
		}
		
		public DerivedRubyDesignerGenerator(ITextEditorOptions textEditorOptions) 
			: base(textEditorOptions)
		{
		}
		
		/// <summary>
		/// Gets or sets the parse information that will be returned from the
		/// ParseFile method.
		/// </summary>
		public ParseInformation ParseInfoToReturnFromParseFileMethod {
			get { return parseInfoToReturnFromParseFile; }
			set { parseInfoToReturnFromParseFile = value; }
		}
				
		protected override ParseInformation ParseFile(string fileName, string textContent)
		{
			return parseInfoToReturnFromParseFile;
		}
	}
}
