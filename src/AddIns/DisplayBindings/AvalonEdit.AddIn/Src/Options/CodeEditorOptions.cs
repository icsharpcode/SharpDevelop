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
using System.Runtime.Serialization;
using System.Windows.Data;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.AvalonEdit.AddIn.Options
{
	[Serializable]
	public class CodeEditorOptions : TextEditorOptions, ITextEditorOptions, ICodeEditorOptions
	{
		static readonly Lazy<CodeEditorOptions> instance = new Lazy<CodeEditorOptions>(
			() => PropertyService.Get("CodeEditorOptions", new CodeEditorOptions()));
		
		public static CodeEditorOptions Instance {
			get { return instance.Value; }
		}
		
		protected override void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			PropertyService.Set("CodeEditorOptions", this);
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
		
		string fontFamily = Core.WinForms.WinFormsResourceService.DefaultMonospacedFont.Name;
		
		public string FontFamily {
			get { return fontFamily; }
			set {
				if (fontFamily != value) {
					fontFamily = value;
					OnPropertyChanged("FontFamily");
				}
			}
		}
		
		double fontSize = 13.0;
		
		[DefaultValue(13.0)]
		public double FontSize {
			get { return fontSize; }
			set {
				if (fontSize != value) {
					fontSize = value;
					OnPropertyChanged("FontSize");
				}
			}
		}
		
		bool showLineNumbers = true;
		
		[DefaultValue(true)]
		public bool ShowLineNumbers {
			get { return showLineNumbers; }
			set {
				if (showLineNumbers != value) {
					showLineNumbers = value;
					OnPropertyChanged("ShowLineNumbers");
				}
			}
		}
		
		bool enableChangeMarkerMargin = true;
		
		[DefaultValue(true)]
		public bool EnableChangeMarkerMargin {
			get { return enableChangeMarkerMargin; }
			set {
				if (enableChangeMarkerMargin != value) {
					enableChangeMarkerMargin = value;
					OnPropertyChanged("EnableChangeMarkerMargin");
				}
			}
		}
		
		bool wordWrap;
		
		[DefaultValue(false)]
		public bool WordWrap {
			get { return wordWrap; }
			set {
				if (wordWrap != value) {
					wordWrap = value;
					OnPropertyChanged("WordWrap");
				}
			}
		}
		
		bool ctrlClickGoToDefinition = true;
		
		[DefaultValue(true)]
		public bool CtrlClickGoToDefinition {
			get { return ctrlClickGoToDefinition; }
			set {
				if (ctrlClickGoToDefinition != value) {
					ctrlClickGoToDefinition = value;
					OnPropertyChanged("CtrlClickGoToDefinition");
				}
			}
		}
		
		bool mouseWheelZoom = true;
		
		[DefaultValue(true)]
		public bool MouseWheelZoom {
			get { return mouseWheelZoom; }
			set {
				if (mouseWheelZoom != value) {
					mouseWheelZoom = value;
					OnPropertyChanged("MouseWheelZoom");
				}
			}
		}
		
		bool underlineErrors = true;
		
		[DefaultValue(true)]
		public bool UnderlineErrors {
			get { return underlineErrors; }
			set {
				if (underlineErrors != value) {
					underlineErrors = value;
					OnPropertyChanged("UnderlineErrors");
				}
			}
		}
		
		bool highlightBrackets = true;
		
		[DefaultValue(true)]
		public bool HighlightBrackets {
			get { return highlightBrackets; }
			set {
				if (highlightBrackets != value) {
					highlightBrackets = value;
					OnPropertyChanged("HighlightBrackets");
				}
			}
		}
		
		bool highlightSymbol = true;
		
		[DefaultValue(true)]
		public bool HighlightSymbol {
			get { return highlightSymbol; }
			set {
				if (highlightSymbol != value) {
					highlightSymbol = value;
					OnPropertyChanged("HighlightSymbol");
				}
			}
		}
		
		bool enableAnimations = true;
		
		[DefaultValue(true)]
		public bool EnableAnimations {
			get { return enableAnimations; }
			set {
				if (enableAnimations != value) {
					enableAnimations = value;
					OnPropertyChanged("EnableAnimations");
				}
			}
		}
		
		bool useSmartIndentation = true;
		
		[DefaultValue(true)]
		public bool UseSmartIndentation {
			get { return useSmartIndentation; }
			set {
				if (useSmartIndentation != value) {
					useSmartIndentation = value;
					OnPropertyChanged("UseSmartIndentation");
				}
			}
		}
		
		bool enableFolding = true;
		
		[DefaultValue(true)]
		public bool EnableFolding {
			get { return enableFolding; }
			set {
				if (enableFolding != value) {
					enableFolding = value;
					OnPropertyChanged("EnableFolding");
				}
			}
		}
		
		bool enableQuickClassBrowser = true;
		
		[DefaultValue(true)]
		public bool EnableQuickClassBrowser {
			get { return enableQuickClassBrowser; }
			set {
				if (enableQuickClassBrowser != value) {
					enableQuickClassBrowser = value;
					OnPropertyChanged("EnableQuickClassBrowser");
				}
			}
		}
		
		bool showHiddenDefinitions = false;
		
		[DefaultValue(false)]
		public bool ShowHiddenDefinitions {
			get { return showHiddenDefinitions; }
			set {
				if (showHiddenDefinitions != value) {
					showHiddenDefinitions = value;
					OnPropertyChanged("ShowHiddenDefinitions");
				}
			}
		}
		
		public void BindToTextEditor(TextEditor editor)
		{
			editor.Options = this;
			editor.SetBinding(TextEditor.FontFamilyProperty, new Binding("FontFamily") { Source = this });
			editor.SetBinding(TextEditor.FontSizeProperty, new Binding("FontSize") { Source = this });
			editor.SetBinding(TextEditor.ShowLineNumbersProperty, new Binding("ShowLineNumbers") { Source = this });
			editor.SetBinding(TextEditor.WordWrapProperty, new Binding("WordWrap") { Source = this });
		}
		
		bool autoInsertBlockEnd = true;
		
		[DefaultValue(true)]
		public bool AutoInsertBlockEnd {
			get { return autoInsertBlockEnd; }
			set {
				if (autoInsertBlockEnd != value) {
					autoInsertBlockEnd = value;
					OnPropertyChanged("AutoInsertBlockEnd");
				}
			}
		}
		
		int ITextEditorOptions.VerticalRulerColumn {
			get { return ColumnRulerPosition; }
		}
	}
}
