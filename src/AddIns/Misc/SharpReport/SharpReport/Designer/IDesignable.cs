/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 22.08.2005
 * Time: 23:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;

namespace SharpReport.Designer{
	/// <summary>
	/// Section Interface 
	/// </summary>
	public interface IDesignable {
		object Parent{
			get;
			set;
		}
		
		Point Location {
			get;
			set;
		}
		
		string Name {
			get;
			set;
		}
		
		Size Size {
			get;
			set;
		}
		
		
		ReportObjectControlBase VisualControl {
			get;
		}
		
		event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		event EventHandler <EventArgs> Selected;
	}
}
