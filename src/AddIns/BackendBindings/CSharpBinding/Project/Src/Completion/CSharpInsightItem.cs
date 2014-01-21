// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			ambience.ConvertEntity(Method, formatter, FormattingOptionsFactory.CreateSharpDevelop());
			
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
