/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 25.05.2006
 * Time: 09:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Forms;


namespace SharpReport.Designer
{
	/// <summary>
	/// Description of ITracker.
	/// </summary>
	public interface ITracker{
		
		/// <summary>
		/// Clear all selections
		/// </summary>
		
		void ClearSelections();
		/// <summary>
		/// Invalidate the DesignSurface and draw the Tracking rectangle
		/// </summary>
		void InvalidateEx();
		
		/// <summary>
		/// The selected Control
		/// </summary>
		ReportControlBase SelectedControl
		{set;}
		
		/// <summary>
		/// The <see cref="RectTracker"></see>
		/// </summary>
		RectTracker RectTracker
		{get;}
		
		///<summary>
		/// The Body Conrol to draw the Treckung Rectangle on
		/// </summary>
	
		Control DesignSurface
		{get;}
	}
}
