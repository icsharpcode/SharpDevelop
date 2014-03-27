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
using System.ComponentModel;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// Code editor options.
	/// </summary>
	public interface ICodeEditorOptions : INotifyPropertyChanged
	{
		bool AllowScrollBelowDocument {
			get;
			set;
		}
		
		bool ShowLineNumbers {
			get;
			set;
		}

		bool EnableChangeMarkerMargin {
			get;
			set;
		}
		
		bool WordWrap {
			get;
			set;
		}
		
		bool CtrlClickGoToDefinition {
			get;
			set;
		}
		
		bool MouseWheelZoom {
			get;
			set;
		}
		
		bool HighlightBrackets {
			get;
			set;
		}
		
		bool HighlightSymbol {
			get;
			set;
		}
		
		bool EnableAnimations {
			get;
			set;
		}
		
		bool UseSmartIndentation {
			get;
			set;
		}
		
		bool EnableFolding {
			get;
			set;
		}
		
		bool EnableQuickClassBrowser {
			get;
			set;
		}
		
		bool ShowHiddenDefinitions {
			get;
			set;
		}
	}
}
