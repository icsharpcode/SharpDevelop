/*
 * Created by SharpDevelop.
 * User: Daniel
 * Date: 04.06.2009
 * Time: 20:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace ICSharpCode.AvalonEdit
{
	/// <summary>
	/// Exposes <see cref="TextEditor"/> to automation.
	/// </summary>
	public class TextEditorAutomationPeer : FrameworkElementAutomationPeer, IValueProvider
	{
		/// <summary>
		/// Creates a new TextEditorAutomationPeer instance.
		/// </summary>
		public TextEditorAutomationPeer(TextEditor owner) : base(owner)
		{
		}
		
		private TextEditor TextEditor {
			get { return (TextEditor)base.Owner; }
		}
		
		void IValueProvider.SetValue(string value)
		{
			this.TextEditor.Text = value;
		}
		
		string IValueProvider.Value {
			get { return this.TextEditor.Text; }
		}
		
		bool IValueProvider.IsReadOnly {
			get { return false; }
		}
		
		/// <inheritdoc/>
		public override object GetPattern(PatternInterface patternInterface)
		{
			if (patternInterface == PatternInterface.Value)
				return this;
			
			if (patternInterface == PatternInterface.Scroll) {
				ScrollViewer scrollViewer = this.TextEditor.ScrollViewer;
				if (scrollViewer != null)
					return UIElementAutomationPeer.CreatePeerForElement(scrollViewer);
			}
			
			return base.GetPattern(patternInterface);
		}
	}
}
