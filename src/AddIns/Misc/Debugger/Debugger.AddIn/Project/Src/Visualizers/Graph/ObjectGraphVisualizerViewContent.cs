// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníèek" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// ViewContent of the visualizer.
	/// </summary>
	public class ObjectGraphVisualizerViewContent : AbstractViewContent
	{
		VisualizerWinFormsControl control = new VisualizerWinFormsControl();
		
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the view.
		/// </summary>
		public override object Control 
		{
			get 
			{
				return control;
			}
		}
		
		public ObjectGraphVisualizerViewContent()
		{
			this.TitleName = "Object graph visualizer";
		}
	}
}
