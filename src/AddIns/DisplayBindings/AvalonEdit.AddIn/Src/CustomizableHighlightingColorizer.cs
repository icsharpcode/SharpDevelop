// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Applies a set of customizations to syntax highlighting.
	/// </summary>
	public class CustomizableHighlightingColorizer : HighlightingColorizer
	{
		#region ApplyCustomizationsToDefaultElements
		public const string DefaultTextAndBackground = "Default text/background";
		public const string SelectedText = "Selected text";
		public const string NonPrintableCharacters = "Non-printable characters";
		public const string LineNumbers = "Line numbers";
		public const string LinkText = "Link text";
		public const string BreakpointMarker = "Breakpoint";
		public const string InstructionPointerMarker = "Current statement";
		public const string ColumnRuler = "Column ruler";
		
		public static void ApplyCustomizationsToDefaultElements(TextEditor textEditor, IEnumerable<CustomizedHighlightingColor> customizations)
		{
			textEditor.ClearValue(TextEditor.BackgroundProperty);
			textEditor.ClearValue(TextEditor.ForegroundProperty);
			textEditor.ClearValue(TextEditor.LineNumbersForegroundProperty);
			textEditor.TextArea.ClearValue(TextArea.SelectionBorderProperty);
			textEditor.TextArea.ClearValue(TextArea.SelectionBrushProperty);
			textEditor.TextArea.ClearValue(TextArea.SelectionForegroundProperty);
			textEditor.TextArea.TextView.ClearValue(TextView.NonPrintableCharacterBrushProperty);
			textEditor.TextArea.TextView.ClearValue(TextView.LinkTextForegroundBrushProperty);
			textEditor.TextArea.TextView.ClearValue(TextView.LinkTextBackgroundBrushProperty);
			textEditor.TextArea.TextView.ClearValue(TextView.ColumnRulerPenProperty);
			
			// 'assigned' flags are used so that the first matching customization wins.
			// This is necessary because more specific customizations come first in the list
			// (language-specific customizations are first, followed by 'all languages' customizations)
			bool assignedDefaultText = false;
			bool assignedSelectedText = false;
			bool assignedNonPrintableCharacter = false;
			bool assignedLineNumbers = false;
			bool assignedLinkText = false;
			bool assignedColumnRulerColor = false;
			
			foreach (CustomizedHighlightingColor color in customizations) {
				switch (color.Name) {
					case DefaultTextAndBackground:
						if (assignedDefaultText)
							continue;
						assignedDefaultText = true;
						
						if (color.Background != null)
							textEditor.Background = CreateFrozenBrush(color.Background.Value);
						if (color.Foreground != null)
							textEditor.Foreground = CreateFrozenBrush(color.Foreground.Value);
						break;
					case SelectedText:
						if (assignedSelectedText)
							continue;
						assignedSelectedText = true;
						
						if (color.Background != null) {
							Pen pen = new Pen(CreateFrozenBrush(color.Background.Value), 1);
							pen.Freeze();
							textEditor.TextArea.SelectionBorder = pen;
							SolidColorBrush back = new SolidColorBrush(color.Background.Value);
							back.Opacity = 0.7; // TODO : remove this constant, let the use choose the opacity.
							back.Freeze();
							textEditor.TextArea.SelectionBrush = back;
						}
						if (color.Foreground != null) {
							textEditor.TextArea.SelectionForeground = CreateFrozenBrush(color.Foreground.Value);
						}
						break;
					case NonPrintableCharacters:
						if (assignedNonPrintableCharacter)
							continue;
						assignedNonPrintableCharacter = true;
						
						if (color.Foreground != null)
							textEditor.TextArea.TextView.NonPrintableCharacterBrush = CreateFrozenBrush(color.Foreground.Value);
						break;
					case LineNumbers:
						if (assignedLineNumbers)
							continue;
						assignedLineNumbers = true;
						
						if (color.Foreground != null)
							textEditor.LineNumbersForeground = CreateFrozenBrush(color.Foreground.Value);
						break;
					case LinkText:
						if (assignedLinkText)
							continue;
						assignedLinkText = true;
						if (color.Foreground != null)
							textEditor.TextArea.TextView.LinkTextForegroundBrush = CreateFrozenBrush(color.Foreground.Value);
						if (color.Background != null)
							textEditor.TextArea.TextView.LinkTextBackgroundBrush = CreateFrozenBrush(color.Background.Value);
						break;
					case ColumnRuler:
						if (assignedColumnRulerColor)
							continue;
						assignedColumnRulerColor = true;
						if (color.Foreground != null)
							textEditor.TextArea.TextView.ColumnRulerPen = CreateFrozenPen(color.Foreground.Value);
						break;
				}
			}
		}
		#endregion
		
		readonly IHighlightingDefinition highlightingDefinition;
		readonly IEnumerable<CustomizedHighlightingColor> customizations;
		
		public CustomizableHighlightingColorizer(IHighlightingDefinition highlightingDefinition, IEnumerable<CustomizedHighlightingColor> customizations)
			: base(highlightingDefinition)
		{
			if (customizations == null)
				throw new ArgumentNullException("customizations");
			this.highlightingDefinition = highlightingDefinition;
			this.customizations = customizations;
		}
		
		protected override void DeregisterServices(TextView textView)
		{
			textView.Services.RemoveService(typeof(IHighlighter));
			base.DeregisterServices(textView);
		}
		
		protected override void RegisterServices(TextView textView)
		{
			base.RegisterServices(textView);
//			textView.Services.AddService(typeof(IHighlighter), (CustomizingHighlighter)textView.GetService(typeof(IHighlighter)));
		}
		
		protected override IHighlighter CreateHighlighter(TextView textView, TextDocument document)
		{
			return new CustomizingHighlighter(textView, customizations, highlightingDefinition, base.CreateHighlighter(textView, document));
		}
		
		internal sealed class CustomizingHighlighter : IHighlighter
		{
			readonly TextView textView;
			readonly IEnumerable<CustomizedHighlightingColor> customizations;
			readonly IHighlightingDefinition highlightingDefinition;
			readonly IHighlighter baseHighlighter;
			readonly IDocument document;
			
			public CustomizingHighlighter(TextView textView, IEnumerable<CustomizedHighlightingColor> customizations, IHighlightingDefinition highlightingDefinition, IHighlighter baseHighlighter)
			{
				Debug.Assert(textView != null);
				Debug.Assert(customizations != null);
				Debug.Assert(highlightingDefinition != null);
				Debug.Assert(baseHighlighter != null);
				
				this.textView = textView;
				this.document = textView.Document;
				this.customizations = customizations;
				this.highlightingDefinition = highlightingDefinition;
				this.baseHighlighter = baseHighlighter;
				baseHighlighter.HighlightingStateChanged += highlighter_HighlightingStateChanged;
			}
			
			public CustomizingHighlighter(IDocument document, IEnumerable<CustomizedHighlightingColor> customizations, IHighlightingDefinition highlightingDefinition, IHighlighter baseHighlighter)
			{
				Debug.Assert(customizations != null);
				Debug.Assert(highlightingDefinition != null);
				Debug.Assert(baseHighlighter != null);
				
				this.document = document;
				this.customizations = customizations;
				this.highlightingDefinition = highlightingDefinition;
				this.baseHighlighter = baseHighlighter;
				baseHighlighter.HighlightingStateChanged += highlighter_HighlightingStateChanged;
			}

			public IDocument Document {
				get { return baseHighlighter.Document; }
			}
			
			public IHighlightingDefinition HighlightingDefinition {
				get { return highlightingDefinition; }
			}
			
			public event HighlightingStateChangedEventHandler HighlightingStateChanged;
			
			public void UpdateHighlightingState(int lineNumber)
			{
				baseHighlighter.UpdateHighlightingState(lineNumber);
			}
			
			void highlighter_HighlightingStateChanged(IHighlighter sender, int fromLineNumber, int toLineNumber)
			{
				if (HighlightingStateChanged != null)
					HighlightingStateChanged(this, fromLineNumber, toLineNumber);
			}
			
			public IEnumerable<string> GetSpanColorNamesFromLineStart(int lineNumber)
			{
				// delayed evaluation doesn't cause a problem here: GetColorStack is called immediately,
				// only the where/select portion is evaluated later. But that won't be a problem because the
				// HighlightingColor instance shouldn't change once it's in use.
				return from color in GetColorStack(lineNumber - 1)
					where color.Name != null
					select color.Name;
			}
			
			public IEnumerable<HighlightingColor> GetColorStack(int lineNumber)
			{
				return baseHighlighter.GetColorStack(lineNumber);
			}
			
			public HighlightedLine HighlightLine(int lineNumber)
			{
				HighlightedLine line = baseHighlighter.HighlightLine(lineNumber);
				foreach (HighlightedSection section in line.Sections) {
					section.Color = CustomizeColor(section.Color, customizations);
				}
				return line;
			}

			public void InvalidateLine(IDocumentLine line)
			{
				if (textView == null)
					throw new InvalidOperationException("IHighlighter has no TextView assigned!");
				textView.Redraw(line, DispatcherPriority.Background);
			}
			
			public void InvalidateAll()
			{
				if (textView == null)
					throw new InvalidOperationException("IHighlighter has no TextView assigned!");
				textView.Redraw(DispatcherPriority.Background);
			}
			
			public event EventHandler VisibleDocumentLinesChanged {
				add { textView.VisualLinesChanged += value; }
				remove { textView.VisualLinesChanged -= value; }
			}
			
			public IEnumerable<IDocumentLine> GetVisibleDocumentLines()
			{
				if (textView == null)
					throw new InvalidOperationException("IHighlighter has no TextView assigned!");
				List<IDocumentLine> result = new List<IDocumentLine>();
				foreach (VisualLine line in textView.VisualLines) {
					if (line.FirstDocumentLine == line.LastDocumentLine) {
						result.Add(line.FirstDocumentLine);
					} else {
						int firstLineStart = line.FirstDocumentLine.Offset;
						int lineEndOffset = firstLineStart + line.FirstDocumentLine.TotalLength;
						foreach (VisualLineElement e in line.Elements) {
							int elementOffset = firstLineStart + e.RelativeTextOffset;
							if (elementOffset >= lineEndOffset) {
								var currentLine = this.Document.GetLineByOffset(elementOffset);
								lineEndOffset = currentLine.Offset + currentLine.TotalLength;
								result.Add(currentLine);
							}
						}
					}
				}
				return result;
			}
			
			public HighlightingColor GetNamedColor(string name)
			{
				return CustomizeColor(name, CustomizedHighlightingColor.FetchCustomizations(highlightingDefinition.Name));
			}
			
			public HighlightingColor DefaultTextColor {
				get {
					return GetNamedColor(CustomizableHighlightingColorizer.DefaultTextAndBackground);
				}
			}
			
			public void BeginHighlighting()
			{
				baseHighlighter.BeginHighlighting();
			}
			
			public void EndHighlighting()
			{
				baseHighlighter.EndHighlighting();
			}
		}
		
		internal static HighlightingColor CustomizeColor(HighlightingColor color, IEnumerable<CustomizedHighlightingColor> customizations)
		{
			if (color == null || color.Name == null)
				return color;
			return CustomizeColor(color.Name, customizations) ?? color;
		}
		
		internal static HighlightingColor CustomizeColor(string name, IEnumerable<CustomizedHighlightingColor> customizations)
		{
			foreach (CustomizedHighlightingColor customization in customizations) {
				if (customization.Name == name) {
					return new HighlightingColor {
						Name = name,
						Background = CreateBrush(customization.Background),
						Foreground = CreateBrush(customization.Foreground),
						FontWeight = customization.Bold ? FontWeights.Bold : FontWeights.Normal,
						FontStyle = customization.Italic ? FontStyles.Italic : FontStyles.Normal
					};
				}
			}
			return null;
		}
		
		static HighlightingBrush CreateBrush(Color? color)
		{
			if (color == null)
				return null;
			else
				return new CustomizedBrush(color.Value);
		}
		
		sealed class CustomizedBrush : HighlightingBrush
		{
			readonly SolidColorBrush brush;
			
			public CustomizedBrush(Color color)
			{
				brush = CreateFrozenBrush(color);
			}
			
			public override Brush GetBrush(ITextRunConstructionContext context)
			{
				return brush;
			}
			
			public override string ToString()
			{
				return brush.ToString();
			}
		}
		
		static SolidColorBrush CreateFrozenBrush(Color color)
		{
			SolidColorBrush brush = new SolidColorBrush(color);
			brush.Freeze();
			return brush;
		}
		
		static Pen CreateFrozenPen(Color color)
		{
			Pen pen = new Pen(CreateFrozenBrush(color), 1);
			pen.Freeze();
			return pen;
		}
	}
}
