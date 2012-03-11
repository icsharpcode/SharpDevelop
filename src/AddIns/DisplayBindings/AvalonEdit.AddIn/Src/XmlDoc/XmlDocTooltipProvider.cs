// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.AddIn.Options;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn.XmlDoc
{
	public class XmlDocTooltipProvider : ITextAreaToolTipProvider
	{
		public void HandleToolTipRequest(ToolTipRequestEventArgs e)
		{
			if (e.ResolveResult == null)
				return;
			TypeResolveResult trr = e.ResolveResult as TypeResolveResult;
			MemberResolveResult mrr = e.ResolveResult as MemberResolveResult;
			LocalResolveResult lrr = e.ResolveResult as LocalResolveResult;
			if (trr != null && trr.Type.GetDefinition() != null) {
				e.SetToolTip(CreateTooltip(trr.Type));
			} else if (mrr != null) {
				e.SetToolTip(CreateTooltip(mrr.Member));
			} else if (lrr != null) {
				var ambience = AmbienceService.GetCurrentAmbience();
				e.SetToolTip(ambience.ConvertVariable(lrr.Variable));
			}
		}
		
		sealed class FlowDocumentTooltip : Border, ITooltip
		{
			FlowDocumentScrollViewer viewer;
			
			public FlowDocumentTooltip(FlowDocument document)
			{
				viewer = new FlowDocumentScrollViewer();
				viewer.Document = document;
				this.Child = viewer;
				
				this.Background = SystemColors.InfoBrush;
				viewer.Foreground = SystemColors.InfoTextBrush;
				this.BorderBrush = SystemColors.InfoTextBrush;
				this.BorderThickness = new Thickness(1);
				this.MaxHeight = 400;
				document.FontSize = CodeEditorOptions.Instance.FontSize;
			}
			
			public event RoutedEventHandler Closed { add {} remove {} }
			
			public bool ShowAsPopup {
				get { return true; }
			}
			
			public bool Close(bool mouseClick)
			{
				return true;
			}
		}
		
		object CreateTooltip(IType type)
		{
			var ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags;
			string header = ambience.ConvertType(type);
			ITypeDefinition entity = type.GetDefinition();
			if (entity != null) {
				var documentation = XmlDocumentationElement.Get(entity);
				if (documentation != null) {
					ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
					DocumentationUIBuilder b = new DocumentationUIBuilder(ambience);
					b.AddCodeBlock(header, keepLargeMargin: true);
					foreach (var child in documentation.Children) {
						b.AddDocumentationElement(child);
					}
					return new FlowDocumentTooltip(b.FlowDocument);
				}
			}
			return header;
		}
		
		object CreateTooltip(IEntity entity)
		{
			var ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags;
			string header = ambience.ConvertEntity(entity);
			var documentation = XmlDocumentationElement.Get(entity);
			
			if (documentation != null) {
				ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
				DocumentationUIBuilder b = new DocumentationUIBuilder(ambience);
				b.AddCodeBlock(header, keepLargeMargin: true);
				foreach (var child in documentation.Children) {
					b.AddDocumentationElement(child);
				}
				return new FlowDocumentTooltip(b.FlowDocument);
			} else {
				return header;
			}
		}
	}
}
