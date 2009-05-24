// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Gui
{
	/// <summary>
	/// Base class for margins.
	/// Margins don't have to derive from this class, it just helps maintaining a reference to the TextView
	/// and the TextDocument.
	/// AbstractMargin derives from FrameworkElement, so if you don't want to handle visual children and rendering
	/// on your own, choose another base class for your margin!
	/// </summary>
	public abstract class AbstractMargin : FrameworkElement
	{
		/// <summary>
		/// TextView property.
		/// </summary>
		public static readonly DependencyProperty TextViewProperty =
			DependencyProperty.Register("TextView", typeof(TextView), typeof(AbstractMargin),
			                            new FrameworkPropertyMetadata(OnTextViewChanged));
		
		/// <summary>
		/// Gets/sets the text view for which line numbers are displayed.
		/// </summary>
		public TextView TextView {
			get { return (TextView)GetValue(TextViewProperty); }
			set { SetValue(TextViewProperty, value); }
		}
		
		static void OnTextViewChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
		{
			((AbstractMargin)dp).OnTextViewChanged((TextView)e.OldValue, (TextView)e.NewValue);
		}
		
		TextDocument document;
		
		/// <summary>
		/// Gets the document associated with the margin.
		/// </summary>
		public TextDocument Document {
			get { return document; }
		}
		
		/// <summary>
		/// Called when the <see cref="TextView"/> is changing.
		/// </summary>
		protected virtual void OnTextViewChanged(TextView oldTextView, TextView newTextView)
		{
			if (oldTextView != null) {
				oldTextView.DocumentChanged -= TextViewDocumentChanged;
			}
			if (newTextView != null) {
				newTextView.DocumentChanged += TextViewDocumentChanged;
			}
			TextViewDocumentChanged(null, null);
		}
		
		void TextViewDocumentChanged(object sender, EventArgs e)
		{
			OnDocumentChanged(document, TextView != null ? TextView.Document : null);
		}
		
		/// <summary>
		/// Called when the <see cref="Document"/> is changing.
		/// </summary>
		protected virtual void OnDocumentChanged(TextDocument oldDocument, TextDocument newDocument)
		{
			document = newDocument;
		}
	}
}
