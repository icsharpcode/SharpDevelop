// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Gui
{
	/// <summary>
	/// A virtualizing panel producing+showing <see cref="VisualLine"/>s for a <see cref="TextDocument"/>.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
	                                                 Justification = "The user usually doesn't work with TextView but with TextEditor; nulling the Document property is sufficient to dispose everything.")]
	public class TextView : FrameworkElement, IScrollInfo, IWeakEventListener
	{
		#region Constructor
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static TextView()
		{
			ClipToBoundsProperty.OverrideMetadata(typeof(TextView), new FrameworkPropertyMetadata(true));
		}
		
		/// <summary>
		/// Creates a new TextView instance.
		/// </summary>
		public TextView()
		{
			elementGenerators.CollectionChanged += delegate { Redraw(); };
			lineTransformers.CollectionChanged += delegate { Redraw(); };
			backgroundRenderer.CollectionChanged += delegate { InvalidateVisual(); };
			adorners = new UIElementCollection(this, this);
		}
		#endregion
		
		#region Properties
		/// <summary>
		/// Document property.
		/// </summary>
		public static readonly DependencyProperty DocumentProperty
			= TextEditor.DocumentProperty.AddOwner(
				typeof(TextView), new FrameworkPropertyMetadata(OnDocumentChanged));
		
		TextDocument document;
		HeightTree heightTree;
		
		/// <summary>
		/// Gets/Sets the document displayed by the text editor.
		/// </summary>
		public TextDocument Document {
			get { return (TextDocument)GetValue(DocumentProperty); }
			set { SetValue(DocumentProperty, value); }
		}
		
		static void OnDocumentChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
		{
			((TextView)dp).OnDocumentChanged((TextDocument)e.OldValue, (TextDocument)e.NewValue);
		}
		
		double LineHeight {
			get {
				return (double)GetValue(TextBlock.FontSizeProperty);
			}
		}
		
		/// <summary>
		/// Occurs when the document property has changed.
		/// </summary>
		public event EventHandler DocumentChanged;
		
		void OnDocumentChanged(TextDocument oldValue, TextDocument newValue)
		{
			if (oldValue != null) {
				heightTree.Dispose();
				heightTree = null;
				formatter.Dispose();
				formatter = null;
				TextDocumentWeakEventManager.Changing.RemoveListener(oldValue, this);
			}
			this.document = newValue;
			ClearScrollData();
			ClearVisualLines();
			if (newValue != null) {
				TextDocumentWeakEventManager.Changing.AddListener(newValue, this);
				heightTree = new HeightTree(newValue, LineHeight + 3);
				formatter = TextFormatter.Create();
			}
			InvalidateMeasure(DispatcherPriority.Normal);
			if (DocumentChanged != null)
				DocumentChanged(this, EventArgs.Empty);
		}
		
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			if (managerType == typeof(TextDocumentWeakEventManager.Changing)) {
				// put redraw into background so that other input events can be handled before the redraw
				DocumentChangeEventArgs change = (DocumentChangeEventArgs)e;
				Redraw(change.Offset, change.RemovalLength, DispatcherPriority.Background);
				return true;
			}
			return false;
		}
		#endregion
		
		readonly ObservableCollection<VisualLineElementGenerator> elementGenerators = new ObservableCollection<VisualLineElementGenerator>();
		
		/// <summary>
		/// Gets a collection where element generators can be registered.
		/// </summary>
		public ObservableCollection<VisualLineElementGenerator> ElementGenerators {
			get { return elementGenerators; }
		}
		
		readonly ObservableCollection<IVisualLineTransformer> lineTransformers = new ObservableCollection<IVisualLineTransformer>();
		
		/// <summary>
		/// Gets a collection where line transformers can be registered.
		/// </summary>
		public ObservableCollection<IVisualLineTransformer> LineTransformers {
			get { return lineTransformers; }
		}
		
		readonly UIElementCollection adorners;
		
		/// <summary>
		/// Gets a collection where text view adorners can be added.
		/// </summary>
		public UIElementCollection Adorners {
			get { return adorners; }
		}
		
		/// <summary>
		/// Causes the text editor to regenerate all visual lines.
		/// </summary>
		public void Redraw()
		{
			Redraw(DispatcherPriority.Render);
		}
		
		/// <summary>
		/// Causes the text editor to regenerate all visual lines.
		/// </summary>
		public void Redraw(DispatcherPriority redrawPriority)
		{
			VerifyAccess();
			ClearVisualLines();
			InvalidateMeasure(redrawPriority);
		}
		
		/// <summary>
		/// Causes the text editor to regenerate the specified visual line.
		/// </summary>
		public void Redraw(VisualLine visualLine, DispatcherPriority redrawPriority)
		{
			VerifyAccess();
			if (allVisualLines.Remove(visualLine)) {
				visibleVisualLines = null;
				DisposeVisualLine(visualLine);
				InvalidateMeasure(redrawPriority);
			}
		}
		
		/// <summary>
		/// Causes the text editor to redraw all lines overlapping with the specified segment.
		/// </summary>
		public void Redraw(int offset, int length, DispatcherPriority redrawPriority)
		{
			VerifyAccess();
			if (allVisualLines.Count != 0 || visibleVisualLines != null) {
				bool removedLine = false;
				for (int i = 0; i < allVisualLines.Count; i++) {
					VisualLine visualLine = allVisualLines[i];
					int lineStart = visualLine.FirstDocumentLine.Offset;
					int lineEnd = visualLine.LastDocumentLine.Offset + visualLine.LastDocumentLine.TotalLength;
					if (!(lineEnd < offset || lineStart > offset + length)) {
						removedLine = true;
						allVisualLines.RemoveAt(i--);
						DisposeVisualLine(visualLine);
					}
				}
				if (removedLine) {
					visibleVisualLines = null;
					InvalidateMeasure(redrawPriority);
				}
			}
		}
		
		/// <summary>
		/// Causes the text editor to redraw all lines overlapping with the specified segment.
		/// Does nothing if segment is null.
		/// </summary>
		public void Redraw(ISegment segment, DispatcherPriority redrawPriority)
		{
			if (segment != null) {
				Redraw(segment.Offset, segment.Length, redrawPriority);
			}
		}
		
		DispatcherOperation invalidateMeasureOperation;
		
		void InvalidateMeasure(DispatcherPriority priority)
		{
			if (priority >= DispatcherPriority.Render) {
				if (invalidateMeasureOperation != null) {
					invalidateMeasureOperation.Abort();
					invalidateMeasureOperation = null;
				}
				base.InvalidateMeasure();
			} else {
				if (invalidateMeasureOperation != null) {
					invalidateMeasureOperation.Priority = priority;
				} else {
					invalidateMeasureOperation = Dispatcher.BeginInvoke(
						priority,
						new Action(
							delegate {
								invalidateMeasureOperation = null;
								base.InvalidateMeasure();
							}
						)
					);
				}
			}
		}
		
		/// <summary>
		/// Waits for the visual lines to be built.
		/// Warning: this methods runs all operations with priority &gt;= Render
		/// </summary>
		public void EnsureVisualLines()
		{
			Dispatcher.VerifyAccess();
			if (visibleVisualLines == null) {
				if (invalidateMeasureOperation != null) {
					// increase priority
					InvalidateMeasure(DispatcherPriority.Normal);
				}
				Dispatcher.DoEvents(DispatcherPriority.Render);
			}
		}
		
		void ClearVisualLines()
		{
			visibleVisualLines = null;
			if (allVisualLines.Count != 0) {
				foreach (VisualLine visualLine in allVisualLines) {
					DisposeVisualLine(visualLine);
				}
				allVisualLines.Clear();
			}
		}
		
		void DisposeVisualLine(VisualLine visualLine)
		{
			visualLine.IsDisposed = true;
			foreach (TextLine textLine in visualLine.TextLines) {
				textLine.Dispose();
			}
			RemoveInlineObjects(visualLine);
		}
		
		/// <summary>
		/// Gets the visual line that contains the document line with the specified number.
		/// Returns null if the document line is outside the visible range.
		/// </summary>
		public VisualLine GetVisualLine(int documentLineNumber)
		{
			VerifyAccess();
			foreach (VisualLine visualLine in allVisualLines) {
				int start = visualLine.FirstDocumentLine.LineNumber;
				int end = visualLine.LastDocumentLine.LineNumber;
				if (documentLineNumber >= start && documentLineNumber <= end)
					return visualLine;
			}
			return null;
		}
		
		/// <summary>
		/// Gets the visual line that contains the document line with the specified number.
		/// If that line is outside the visible range, a new VisualLine for that document line is constructed.
		/// </summary>
		public VisualLine GetOrConstructVisualLine(DocumentLine documentLine)
		{
			if (documentLine == null)
				throw new ArgumentNullException("documentLine");
			if (documentLine.Document != this.Document)
				throw new InvalidOperationException("Line belongs to wrong document");
			VerifyAccess();
			
			VisualLine l = GetVisualLine(documentLine.LineNumber);
			if (l == null) {
				TextRunProperties globalTextRunProperties = CreateGlobalTextRunProperties();
				TextParagraphProperties paragraphProperties = CreateParagraphProperties(globalTextRunProperties);
				
				while (heightTree.GetIsCollapsed(documentLine)) {
					documentLine = heightTree.GetLineByNumber(documentLine.LineNumber - 1);
				}
				
				l = BuildVisualLine(documentLine,
				                    globalTextRunProperties, paragraphProperties,
				                    elementGenerators.ToArray(), lineTransformers.ToArray(),
				                    lastAvailableSize);
				l.VisualTop = heightTree.GetVisualPosition(documentLine);
				allVisualLines.Add(l);
			}
			return l;
		}
		
		/// <summary>
		/// Collapses lines for the purpose of scrolling. This method is meant for
		/// <see cref="VisualLineElementGenerator"/>s that cause <see cref="VisualLine"/>s to span
		/// multiple <see cref="DocumentLine"/>s. Do not call it without providing a corresponding
		/// <see cref="VisualLineElementGenerator"/>.
		/// If you want to create collapsible text sections, see <see cref="FoldingManager"/>.
		/// </summary>
		public CollapsedLineSection CollapseLines(DocumentLine start, DocumentLine end)
		{
			VerifyAccess();
			return heightTree.CollapseText(start, end);
		}
		
		#region Measure
		TextFormatter formatter;
		List<VisualLine> allVisualLines = new List<VisualLine>();
		ReadOnlyCollection<VisualLine> visibleVisualLines;
		double clippedPixelsOnTop;
		
		/// <summary>
		/// Gets the currently visible visual lines.
		/// Is empty when the visual lines are not available
		/// (when one or more of the visual lines need to be regenerated).
		/// </summary>
		public ReadOnlyCollection<VisualLine> VisualLines {
			get {
				return visibleVisualLines ?? Empty<VisualLine>.ReadOnlyCollection;
			}
		}
		
		/// <summary>
		/// Occurs when the TextView was measured and changed its visual lines.
		/// </summary>
		public event EventHandler VisualLinesChanged;
		
		TextRunProperties CreateGlobalTextRunProperties()
		{
			return new GlobalTextRunProperties {
				typeface = this.CreateTypeface(),
				fontRenderingEmSize = LineHeight,
				foregroundBrush = (Brush)GetValue(Control.ForegroundProperty),
				cultureInfo = CultureInfo.CurrentCulture
			};
		}
		
		TextParagraphProperties CreateParagraphProperties(TextRunProperties defaultTextRunProperties)
		{
			return new VisualLineTextParagraphProperties {
				defaultTextRunProperties = defaultTextRunProperties,
				textWrapping = canHorizontallyScroll ? TextWrapping.NoWrap : TextWrapping.Wrap,
				tabSize = 4 * WideSpaceWidth
			};
		}
		
		Size lastAvailableSize;
		
		/// <summary>
		/// Measure implementation.
		/// </summary>
		protected override Size MeasureOverride(Size availableSize)
		{
			if (!canHorizontallyScroll && !availableSize.Width.IsClose(lastAvailableSize.Width))
				ClearVisualLines();
			
			RemoveInlineObjectsNow();
			
			lastAvailableSize = availableSize;
			if (document == null)
				return Size.Empty;
			
			TextRunProperties globalTextRunProperties = CreateGlobalTextRunProperties();
			TextParagraphProperties paragraphProperties = CreateParagraphProperties(globalTextRunProperties);
			InvalidateVisual(); // = InvalidateArrange+InvalidateRender
			
			Debug.WriteLine("Measure availableSize=" + availableSize + ", scrollOffset=" + scrollOffset);
			var firstLineInView = heightTree.GetLineByVisualPosition(scrollOffset.Y);
			
			// number of pixels clipped from the first visual line(s)
			clippedPixelsOnTop = scrollOffset.Y - heightTree.GetVisualPosition(firstLineInView);
			Debug.Assert(clippedPixelsOnTop >= 0);
			
			List<VisualLine> newVisualLines = new List<VisualLine>();
			
			var elementGeneratorsArray = elementGenerators.ToArray();
			var lineTransformersArray = lineTransformers.ToArray();
			var nextLine = firstLineInView;
			double maxWidth = 0;
			double yPos = -clippedPixelsOnTop;
			while (yPos < availableSize.Height && nextLine != null) {
				VisualLine visualLine = GetVisualLine(nextLine.LineNumber);
				if (visualLine == null) {
					visualLine = BuildVisualLine(nextLine,
					                             globalTextRunProperties, paragraphProperties,
					                             elementGeneratorsArray, lineTransformersArray,
					                             availableSize);
				}
				
				visualLine.VisualTop = scrollOffset.Y + yPos;
				
				int visualLineEndLineNumber = visualLine.LastDocumentLine.LineNumber;
				if (visualLineEndLineNumber == document.LineCount)
					nextLine = null;
				else
					nextLine = document.GetLineByNumber(visualLineEndLineNumber + 1);
				
				yPos += visualLine.Height;
				
				foreach (TextLine textLine in visualLine.TextLines) {
					if (textLine.WidthIncludingTrailingWhitespace > maxWidth)
						maxWidth = textLine.WidthIncludingTrailingWhitespace;
				}
				
				newVisualLines.Add(visualLine);
			}
			
			foreach (VisualLine line in allVisualLines) {
				if (!newVisualLines.Contains(line))
					DisposeVisualLine(line);
			}
			RemoveInlineObjectsNow();
			allVisualLines = newVisualLines;
			// visibleVisualLines = readonly copy of visual lines
			visibleVisualLines = new ReadOnlyCollection<VisualLine>(newVisualLines.ToArray());
			
			if (allVisualLines.Any(line => line.IsDisposed)) {
				throw new InvalidOperationException("A visual line was disposed even though it is still in use.\n" +
				                                    "This can happen when Redraw() is called during measure for lines " +
				                                    "that are already constructed.");
			}
			
			SetScrollData(availableSize,
			              new Size(maxWidth, heightTree.TotalHeight),
			              scrollOffset);
			if (VisualLinesChanged != null)
				VisualLinesChanged(this, EventArgs.Empty);
			if (canHorizontallyScroll) {
				return availableSize;
			} else {
				return new Size(maxWidth, availableSize.Height);
			}
		}
		
		VisualLine BuildVisualLine(DocumentLine documentLine,
		                           TextRunProperties globalTextRunProperties,
		                           TextParagraphProperties paragraphProperties,
		                           VisualLineElementGenerator[] elementGeneratorsArray,
		                           IVisualLineTransformer[] lineTransformersArray,
		                           Size availableSize)
		{
			if (heightTree.GetIsCollapsed(documentLine))
				throw new InvalidOperationException("Trying to build visual line from collapsed line");
			
			Debug.WriteLine("Building line " + documentLine.LineNumber);
			
			VisualLine visualLine = new VisualLine(documentLine);
			VisualLineTextSource textSource = new VisualLineTextSource(visualLine) {
				Document = document,
				GlobalTextRunProperties = globalTextRunProperties,
				TextView = this
			};
			
			visualLine.ConstructVisualElements(textSource, elementGeneratorsArray);
			
			#if DEBUG
			for (int i = visualLine.FirstDocumentLine.LineNumber + 1; i <= visualLine.LastDocumentLine.LineNumber; i++) {
				if (!heightTree.GetIsCollapsed(document.GetLineByNumber(i)))
					throw new InvalidOperationException("Line " + i + " was skipped by a VisualLineElementGenerator, but it is not collapsed.");
			}
			#endif
			
			visualLine.RunTransformers(textSource, lineTransformersArray);
			
			// now construct textLines:
			int textOffset = 0;
			TextLineBreak lastLineBreak = null;
			var textLines = new List<TextLine>();
			while (textOffset <= visualLine.VisualLength) {
				TextLine textLine = formatter.FormatLine(
					textSource,
					textOffset,
					availableSize.Width,
					paragraphProperties,
					lastLineBreak
				);
				textLines.Add(textLine);
				textOffset += textLine.Length;
				
				lastLineBreak = textLine.GetTextLineBreak();
			}
			visualLine.SetTextLines(textLines);
			heightTree.SetHeight(visualLine.FirstDocumentLine, visualLine.Height);
			return visualLine;
		}
		#endregion
		
		#region Inline object handling
		List<InlineObjectRun> inlineObjects = new List<InlineObjectRun>();
		
		/// <summary>
		/// Adds a new inline object.
		/// </summary>
		internal void AddInlineObject(InlineObjectRun inlineObject)
		{
			inlineObjects.Add(inlineObject);
			AddVisualChild(inlineObject.Element);
			inlineObject.Element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
		}
		
		List<VisualLine> visualLinesWithOutstandingInlineObjects = new List<VisualLine>();
		
		void RemoveInlineObjects(VisualLine visualLine)
		{
			// Delay removing inline objects:
			// A document change immediately invalidates affected visual lines, but it does not
			// cause an immediate redraw.
			// To prevent inline objects from flickering when they are recreated, we delay removing
			// inline objects until the next redraw.
			visualLinesWithOutstandingInlineObjects.Add(visualLine);
		}
		
		void RemoveInlineObjectsNow()
		{
			inlineObjects.RemoveAll(
				ior => {
					if (visualLinesWithOutstandingInlineObjects.Contains(ior.VisualLine)) {
						ior.VisualLine = null;
						RemoveVisualChild(ior.Element);
						return true;
					}
					return false;
				});
			visualLinesWithOutstandingInlineObjects.Clear();
		}
		
		/// <summary>
		/// Removes the inline object that displays the specified UIElement.
		/// </summary>
		public void RemoveInlineObject(UIElement element)
		{
			inlineObjects.RemoveAll(
				ior => {
					if (ior.Element == element) {
						ior.VisualLine = null;
						RemoveVisualChild(ior.Element);
						return true;
					}
					return false;
				});
		}
		
		/// <inheritdoc/>
		protected override int VisualChildrenCount {
			get { return inlineObjects.Count + adorners.Count; }
		}
		
		/// <inheritdoc/>
		protected override Visual GetVisualChild(int index)
		{
			if (index < inlineObjects.Count)
				return inlineObjects[index].Element;
			else
				return adorners[index - inlineObjects.Count];
		}
		#endregion
		
		#region Arrange
		/// <summary>
		/// Arrange implementation.
		/// </summary>
		protected override Size ArrangeOverride(Size finalSize)
		{
			if (document == null || allVisualLines.Count == 0)
				return finalSize;
			
			// validate scroll position
			Vector newScrollOffset = scrollOffset;
			if (scrollOffset.X + finalSize.Width > scrollExtent.Width) {
				newScrollOffset.X = Math.Max(0, scrollExtent.Width - finalSize.Width);
			}
			if (scrollOffset.Y + finalSize.Height > scrollExtent.Height) {
				newScrollOffset.Y = Math.Max(0, scrollExtent.Height - finalSize.Height);
			}
			if (SetScrollData(scrollViewport, scrollExtent, newScrollOffset))
				InvalidateMeasure(DispatcherPriority.Normal);
			
			//Debug.WriteLine("Arrange finalSize=" + finalSize + ", scrollOffset=" + scrollOffset);
			
//			double maxWidth = 0;
			
			foreach (UIElement adorner in adorners) {
				adorner.Arrange(new Rect(new Point(0, 0), finalSize));
			}
			
			if (visibleVisualLines != null) {
				Point pos = new Point(-scrollOffset.X, -clippedPixelsOnTop);
				foreach (VisualLine visualLine in visibleVisualLines) {
					int offset = 0;
					foreach (TextLine textLine in visualLine.TextLines) {
						foreach (var span in textLine.GetTextRunSpans()) {
							InlineObjectRun inline = span.Value as InlineObjectRun;
							if (inline != null && inline.VisualLine != null) {
								Debug.Assert(inlineObjects.Contains(inline));
								double distance = textLine.GetDistanceFromCharacterHit(new CharacterHit(offset, 0));
								inline.Element.Arrange(new Rect(new Point(pos.X + distance, pos.Y), inline.Element.DesiredSize));
							}
							offset += span.Length;
						}
						pos.Y += textLine.Height;
					}
				}
			}
			InvalidateCursor();
			
			return finalSize;
		}
		#endregion
		
		#region Render
		readonly ObservableCollection<IBackgroundRenderer> backgroundRenderer = new ObservableCollection<IBackgroundRenderer>();
		
		/// <summary>
		/// Gets a collection where line transformers can be registered.
		/// </summary>
		public ObservableCollection<IBackgroundRenderer> BackgroundRenderer {
			get { return backgroundRenderer; }
		}
		
		/// <inheritdoc/>
		protected override void OnRender(DrawingContext drawingContext)
		{
			foreach (IBackgroundRenderer r in backgroundRenderer)
				r.Draw(drawingContext);
			Point pos = new Point(-scrollOffset.X, -clippedPixelsOnTop);
			foreach (VisualLine visualLine in allVisualLines) {
				foreach (TextLine textLine in visualLine.TextLines) {
					textLine.Draw(drawingContext, pos, InvertAxes.None);
					pos.Y += textLine.Height;
				}
			}
		}
		#endregion
		
		#region IScrollInfo implementation
		/// <summary>
		/// Size of the document, in pixels.
		/// </summary>
		Size scrollExtent;
		
		/// <summary>
		/// Offset of the scroll position.
		/// </summary>
		Vector scrollOffset;
		
		/// <summary>
		/// Size of the viewport.
		/// </summary>
		Size scrollViewport;
		
		void ClearScrollData()
		{
			SetScrollData(new Size(), new Size(), new Vector());
		}
		
		bool SetScrollData(Size viewport, Size extent, Vector offset)
		{
			if (!(viewport.IsClose(this.scrollViewport)
			      && extent.IsClose(this.scrollExtent)
			      && offset.IsClose(this.scrollOffset)))
			{
				this.scrollViewport = viewport;
				this.scrollExtent = extent;
				SetScrollOffset(offset);
				this.OnScrollChange();
				return true;
			}
			return false;
		}
		
		void OnScrollChange()
		{
			ScrollViewer scrollOwner = ((IScrollInfo)this).ScrollOwner;
			if (scrollOwner != null) {
				scrollOwner.InvalidateScrollInfo();
			}
		}
		
		bool canVerticallyScroll;
		bool IScrollInfo.CanVerticallyScroll {
			get { return canVerticallyScroll; }
			set {
				if (canVerticallyScroll != value) {
					canVerticallyScroll = value;
					InvalidateMeasure(DispatcherPriority.Normal);
				}
			}
		}
		bool canHorizontallyScroll;
		bool IScrollInfo.CanHorizontallyScroll {
			get { return canHorizontallyScroll; }
			set {
				if (canHorizontallyScroll != value) {
					canHorizontallyScroll = value;
					ClearVisualLines();
					InvalidateMeasure(DispatcherPriority.Normal);
				}
			}
		}
		
		double IScrollInfo.ExtentWidth {
			get { return scrollExtent.Width; }
		}
		
		double IScrollInfo.ExtentHeight {
			get { return scrollExtent.Height; }
		}
		
		double IScrollInfo.ViewportWidth {
			get { return scrollViewport.Width; }
		}
		
		double IScrollInfo.ViewportHeight {
			get { return scrollViewport.Height; }
		}
		
		/// <summary>
		/// Gets the horizontal scroll offset.
		/// </summary>
		public double HorizontalOffset {
			get { return scrollOffset.X; }
		}
		
		/// <summary>
		/// Gets the vertical scroll offset.
		/// </summary>
		public double VerticalOffset {
			get { return scrollOffset.Y; }
		}
		
		/// <summary>
		/// Gets the scroll offset;
		/// </summary>
		public Vector ScrollOffset {
			get { return scrollOffset; }
		}
		
		/// <summary>
		/// Occurs when the scroll offset has changed.
		/// </summary>
		public event EventHandler ScrollOffsetChanged;
		
		void SetScrollOffset(Vector vector)
		{
			if (!scrollOffset.IsClose(vector)) {
				scrollOffset = vector;
				if (ScrollOffsetChanged != null)
					ScrollOffsetChanged(this, EventArgs.Empty);
			}
		}
		
		ScrollViewer IScrollInfo.ScrollOwner { get; set; }
		
		void IScrollInfo.LineUp()
		{
			((IScrollInfo)this).SetVerticalOffset(scrollOffset.Y - LineHeight);
		}
		
		void IScrollInfo.LineDown()
		{
			((IScrollInfo)this).SetVerticalOffset(scrollOffset.Y + LineHeight);
		}
		
		void IScrollInfo.LineLeft()
		{
			((IScrollInfo)this).SetHorizontalOffset(scrollOffset.X - WideSpaceWidth);
		}
		
		void IScrollInfo.LineRight()
		{
			((IScrollInfo)this).SetHorizontalOffset(scrollOffset.X + WideSpaceWidth);
		}
		
		void IScrollInfo.PageUp()
		{
			((IScrollInfo)this).SetVerticalOffset(scrollOffset.Y - scrollViewport.Height);
		}
		
		void IScrollInfo.PageDown()
		{
			((IScrollInfo)this).SetVerticalOffset(scrollOffset.Y + scrollViewport.Height);
		}
		
		void IScrollInfo.PageLeft()
		{
			((IScrollInfo)this).SetHorizontalOffset(scrollOffset.X - scrollViewport.Width);
		}
		
		void IScrollInfo.PageRight()
		{
			((IScrollInfo)this).SetHorizontalOffset(scrollOffset.X + scrollViewport.Width);
		}
		
		void IScrollInfo.MouseWheelUp()
		{
			((IScrollInfo)this).SetVerticalOffset(
				scrollOffset.Y - (SystemParameters.WheelScrollLines * LineHeight));
			OnScrollChange();
		}
		
		void IScrollInfo.MouseWheelDown()
		{
			((IScrollInfo)this).SetVerticalOffset(
				scrollOffset.Y + (SystemParameters.WheelScrollLines * LineHeight));
			OnScrollChange();
		}
		
		void IScrollInfo.MouseWheelLeft()
		{
			((IScrollInfo)this).SetHorizontalOffset(
				scrollOffset.X - (SystemParameters.WheelScrollLines * WideSpaceWidth));
			OnScrollChange();
		}
		
		void IScrollInfo.MouseWheelRight()
		{
			((IScrollInfo)this).SetHorizontalOffset(
				scrollOffset.X + (SystemParameters.WheelScrollLines * WideSpaceWidth));
			OnScrollChange();
		}
		
		double WideSpaceWidth {
			get {
				return LineHeight / 2;
			}
		}
		
		static double ValidateVisualOffset(double offset)
		{
			if (double.IsNaN(offset))
				throw new ArgumentException("offset must not be NaN");
			if (offset < 0)
				return 0;
			else
				return offset;
		}
		
		void IScrollInfo.SetHorizontalOffset(double offset)
		{
			offset = ValidateVisualOffset(offset);
			if (!scrollOffset.X.IsClose(offset)) {
				SetScrollOffset(new Vector(offset, scrollOffset.Y));
				InvalidateVisual();
			}
		}
		
		void IScrollInfo.SetVerticalOffset(double offset)
		{
			offset = ValidateVisualOffset(offset);
			if (!scrollOffset.Y.IsClose(offset)) {
				SetScrollOffset(new Vector(scrollOffset.X, offset));
				InvalidateMeasure(DispatcherPriority.Normal);
			}
		}
		
		Rect IScrollInfo.MakeVisible(Visual visual, Rect rectangle)
		{
			if (rectangle.IsEmpty || visual == null || visual == this || !this.IsAncestorOf(visual)) {
				return Rect.Empty;
			}
			// Convert rectangle into our coordinate space.
			GeneralTransform childTransform = visual.TransformToAncestor(this);
			rectangle = childTransform.TransformBounds(rectangle);
			
			MakeVisible(rectangle);
			
			return rectangle;
		}
		
		/// <summary>
		/// Scrolls the text view so that the specified rectangle gets visible.
		/// </summary>
		public void MakeVisible(Rect rectangle)
		{
			Rect visibleRectangle = new Rect(scrollOffset.X, scrollOffset.Y,
			                                 scrollViewport.Width, scrollViewport.Height);
			Vector newScrollOffset = scrollOffset;
			if (rectangle.Left < visibleRectangle.Left) {
				if (rectangle.Right > visibleRectangle.Right) {
					newScrollOffset.X = rectangle.Left + rectangle.Width / 2;
				} else {
					newScrollOffset.X = rectangle.Left;
				}
			} else if (rectangle.Right > visibleRectangle.Right) {
				newScrollOffset.X = rectangle.Right - scrollViewport.Width;
			}
			if (rectangle.Top < visibleRectangle.Top) {
				if (rectangle.Bottom > visibleRectangle.Bottom) {
					newScrollOffset.Y = rectangle.Top + rectangle.Height / 2;
				} else {
					newScrollOffset.Y = rectangle.Top;
				}
			} else if (rectangle.Bottom > visibleRectangle.Bottom) {
				newScrollOffset.Y = rectangle.Bottom - scrollViewport.Height;
			}
			newScrollOffset.X = ValidateVisualOffset(newScrollOffset.X);
			newScrollOffset.Y = ValidateVisualOffset(newScrollOffset.Y);
			if (!scrollOffset.IsClose(newScrollOffset)) {
				SetScrollOffset(newScrollOffset);
				this.OnScrollChange();
				InvalidateMeasure(DispatcherPriority.Normal);
			}
		}
		#endregion
		
		/// <summary>
		/// Gets the document line at the specified visual position.
		/// </summary>
		public DocumentLine GetDocumentLineByVisualTop(double visualTop)
		{
			VerifyAccess();
			if (heightTree == null)
				throw new InvalidOperationException();
			return heightTree.GetLineByVisualPosition(visualTop);
		}
		
		#region Visual element mouse handling
		/// <inheritdoc/>
		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			// accept clicks even where the text area draws no background
			return new PointHitTestResult(this, hitTestParameters.HitPoint);
		}
		
		[ThreadStatic] static bool invalidCursor;
		
		/// <summary>
		/// Updates the mouse cursor by calling <see cref="Mouse.UpdateCursor"/>, but with input priority.
		/// </summary>
		public static void InvalidateCursor()
		{
			if (!invalidCursor) {
				invalidCursor = true;
				Dispatcher.CurrentDispatcher.BeginInvoke(
					DispatcherPriority.Input,
					new Action(
						delegate {
							invalidCursor = false;
							Mouse.UpdateCursor();
						}));
			}
		}
		
		/// <inheritdoc/>
		protected override void OnQueryCursor(QueryCursorEventArgs e)
		{
			VisualLineElement element = GetVisualLineElementFromPosition(e.GetPosition(this) + scrollOffset);
			if (element != null) {
				element.OnQueryCursor(e);
			}
		}
		
		/// <inheritdoc/>
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			if (!e.Handled) {
				EnsureVisualLines();
				VisualLineElement element = GetVisualLineElementFromPosition(e.GetPosition(this) + scrollOffset);
				if (element != null) {
					element.OnMouseDown(e);
				}
			}
		}
		
		/// <inheritdoc/>
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);
			if (!e.Handled) {
				EnsureVisualLines();
				VisualLineElement element = GetVisualLineElementFromPosition(e.GetPosition(this) + scrollOffset);
				if (element != null) {
					element.OnMouseUp(e);
				}
			}
		}
		
		/// <summary>
		/// Gets the visual line at the specified document position (relative to start of document).
		/// Returns null if there is no visual line for the position (e.g. the position is outside the visible
		/// text area).
		/// You may want to call <see cref="EnsureVisualLines"/>() before calling this method.
		/// </summary>
		public VisualLine GetVisualLineFromVisualTop(double visualTop)
		{
			VerifyAccess();
			foreach (VisualLine vl in this.VisualLines) {
				if (visualTop < vl.VisualTop)
					continue;
				if (visualTop < vl.VisualTop + vl.Height)
					return vl;
			}
			return null;
		}
		
		VisualLineElement GetVisualLineElementFromPosition(Point visualPosition)
		{
			VisualLine vl = GetVisualLineFromVisualTop(visualPosition.Y);
			if (vl != null) {
				int column = vl.GetVisualColumn(visualPosition);
//				Debug.WriteLine(vl.FirstDocumentLine.LineNumber + " vc " + column);
				foreach (VisualLineElement element in vl.Elements) {
					if (element.VisualColumn + element.VisualLength < column)
						continue;
					return element;
				}
			}
			return null;
		}
		#endregion
	}
}
