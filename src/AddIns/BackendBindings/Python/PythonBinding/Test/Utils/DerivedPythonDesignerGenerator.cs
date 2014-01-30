// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

using ICSharpCode.FormsDesigner;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// Gives access to various protected methods
	/// of the PythonDesignerGenerator class for testing.
	/// </summary>
	public class DerivedPythonDesignerGenerator : PythonDesignerGenerator
	{
		ParseInformation parseInfoToReturnFromParseFile;

		public DerivedPythonDesignerGenerator() : this(new MockTextEditorOptions())
		{
		}
		
		public DerivedPythonDesignerGenerator(ITextEditorOptions textEditorOptions) 
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
