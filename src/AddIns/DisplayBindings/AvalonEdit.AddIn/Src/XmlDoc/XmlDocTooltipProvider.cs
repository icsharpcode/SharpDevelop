// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
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
		
		sealed class FlowDocumentTooltip : Popup, ITooltip
		{
			FlowDocumentScrollViewer viewer;
			
			public FlowDocumentTooltip(FlowDocument document)
			{
				TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
				viewer = new FlowDocumentScrollViewer();
				viewer.Document = document;
				Border border = new Border {
					Background = SystemColors.InfoBrush,
					BorderBrush = SystemColors.InfoTextBrush,
					BorderThickness = new Thickness(1),
					MaxHeight = 400,
					Child = viewer
				};
				this.Child = border;
				viewer.Foreground = SystemColors.InfoTextBrush;
				document.FontSize = CodeEditorOptions.Instance.FontSize;
			}
			
			public bool CloseWhenMouseMovesAway {
				get { return !this.IsKeyboardFocusWithin; }
			}
			
			protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
			{
				base.OnLostKeyboardFocus(e);
				this.IsOpen = false;
			}
		}
		
		object CreateTooltip(IType type)
		{
			var ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags | ConversionFlags.ShowDeclaringType | ConversionFlags.UseFullyQualifiedTypeNames;
			string header;
			if (type is ITypeDefinition)
				header = ambience.ConvertEntity((ITypeDefinition)type);
			else
				header = ambience.ConvertType(type);
			
			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
			DocumentationUIBuilder b = new DocumentationUIBuilder(ambience);
			b.AddCodeBlock(header, keepLargeMargin: true);
			
			ITypeDefinition entity = type.GetDefinition();
			if (entity != null) {
				var documentation = XmlDocumentationElement.Get(entity);
				if (documentation != null) {
					foreach (var child in documentation.Children) {
						b.AddDocumentationElement(child);
					}
				}
			}
			return new FlowDocumentTooltip(b.CreateFlowDocument());
		}
		
		object CreateTooltip(IEntity entity)
		{
			var ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags | ConversionFlags.ShowDeclaringType | ConversionFlags.UseFullyQualifiedTypeNames;
			string header = ambience.ConvertEntity(entity);
			var documentation = XmlDocumentationElement.Get(entity);
			
			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
			DocumentationUIBuilder b = new DocumentationUIBuilder(ambience);
			b.AddCodeBlock(header, keepLargeMargin: true);
			if (documentation != null) {
				foreach (var child in documentation.Children) {
					b.AddDocumentationElement(child);
				}
			}
			return new FlowDocumentTooltip(b.CreateFlowDocument());
		}
	}
}
