/*
 * Created by SharpDevelop.
 * User: DG
 * Date: 17.12.2005
 * Time: 21:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.TextEditor
{
	public delegate void ToolTipRequestEventHandler(object sender, ToolTipRequestEventArgs e);
	
	public class ToolTipRequestEventArgs
	{
		Point mousePosition;
		Point logicalPosition;
		bool inDocument;
		
		public Point MousePosition {
			get {
				return mousePosition;
			}
		}
		
		public Point LogicalPosition {
			get {
				return logicalPosition;
			}
		}
		
		public bool InDocument {
			get {
				return inDocument;
			}
		}
		
		/// <summary>
		/// Gets if some client handling the event has already shown a tool tip.
		/// </summary>
		public bool ToolTipShown {
			get {
				return toolTipText != null;
			}
		}
		
		internal string toolTipText;
		
		public void ShowToolTip(string text)
		{
			toolTipText = text;
		}
		
		public ToolTipRequestEventArgs(Point mousePosition, Point logicalPosition, bool inDocument)
		{
			this.mousePosition = mousePosition;
			this.logicalPosition = logicalPosition;
			this.inDocument = inDocument;
		}
	}
}
