/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.06.2015
 * Time: 11:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using ICSharpCode.Reporting.Exporter.Visitors;

namespace ICSharpCode.Reporting.PageBuilder.ExportColumns
{
	/// <summary>
	/// Description of ExportImage.
	/// </summary>
	public class ExportImage:ExportColumn,IAcceptor
	{
		public ExportImage()
		{
		}

		#region IAcceptor implementation
		public void Accept(IVisitor visitor){
			visitor.Visit(this);
		}
		#endregion
		
		public Image Image {get;set;}
		
		public bool ScaleImageToSize {get;set;}
	}
}
