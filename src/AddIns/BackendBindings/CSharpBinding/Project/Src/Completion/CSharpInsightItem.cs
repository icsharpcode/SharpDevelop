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
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Xml;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using CSharpBinding.FormattingStrategy;

namespace CSharpBinding.Completion
{
	sealed class CSharpInsightItem : IInsightItem
	{
		public readonly IParameterizedMember Method;
		
		public CSharpInsightItem(IParameterizedMember method)
		{
			this.Method = method;
		}
		
		FlowDocumentScrollViewer header;
		
		public object Header {
			get {
				if (header == null) {
					header = GenerateHeader();
					OnPropertyChanged("Header");
				}
				return header;
			}
		}
		
		int highlightedParameterIndex = -1;
		
		public void HighlightParameter(int parameterIndex)
		{
			if (highlightedParameterIndex == parameterIndex)
				return;
			this.highlightedParameterIndex = parameterIndex;
			if (header != null)
				header = GenerateHeader();
			OnPropertyChanged("Header");
			OnPropertyChanged("Content");
		}
		
		FlowDocumentScrollViewer GenerateHeader()
		{
			CSharpAmbience ambience = new CSharpAmbience();
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags;
			var stringBuilder = new StringBuilder();
			var formatter = new ParameterHighlightingOutputFormatter(stringBuilder, highlightedParameterIndex);
			ambience.ConvertSymbol(Method, formatter, CSharpFormattingPolicies.Instance.GlobalOptions.OptionsContainer.GetEffectiveOptions());
			
			var documentation = XmlDocumentationElement.Get(Method);
			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
			
			DocumentationUIBuilder b = new DocumentationUIBuilder(ambience);
			string parameterName = null;
			if (Method.Parameters.Count > highlightedParameterIndex)
				parameterName = Method.Parameters[highlightedParameterIndex].Name;
			b.AddSignatureBlock(stringBuilder.ToString(), formatter.parameterStartOffset, formatter.parameterLength, parameterName);
			
			DocumentationUIBuilder b2 = new DocumentationUIBuilder(ambience);
			b2.ParameterName = parameterName;
			b2.ShowAllParameters = false;
			
			if (documentation != null) {
				foreach (var child in documentation.Children) {
					b2.AddDocumentationElement(child);
				}
			}
			
			content = new FlowDocumentScrollViewer {
				Document = b2.CreateFlowDocument(),
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto
			};
			
			var flowDocument = b.CreateFlowDocument();
			flowDocument.PagePadding = new Thickness(0); // default is NOT Thickness(0), but Thickness(Auto), which adds unnecessary space
			
			return new FlowDocumentScrollViewer {
				Document = flowDocument,
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto
			};
		}
		
		FlowDocumentScrollViewer content;
		
		public object Content {
			get {
				if (content == null) {
					GenerateHeader();
					OnPropertyChanged("Content");
				}
				return content;
			}
		}
		
		sealed class ParameterHighlightingOutputFormatter : TextWriterTokenWriter
		{
			StringBuilder b;
			int highlightedParameterIndex;
			int parameterIndex;
			internal int parameterStartOffset;
			internal int parameterLength;
			
			public ParameterHighlightingOutputFormatter(StringBuilder b, int highlightedParameterIndex)
				: base(new StringWriter(b))
			{
				this.b = b;
				this.highlightedParameterIndex = highlightedParameterIndex;
			}
			
			public override void StartNode(AstNode node)
			{
				if (parameterIndex == highlightedParameterIndex && node is ParameterDeclaration) {
					parameterStartOffset = b.Length;
				}
				base.StartNode(node);
			}
			
			public override void EndNode(AstNode node)
			{
				base.EndNode(node);
				if (node is ParameterDeclaration) {
					if (parameterIndex == highlightedParameterIndex)
						parameterLength = b.Length - parameterStartOffset;
					parameterIndex++;
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
