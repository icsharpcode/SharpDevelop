/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 03.10.2006
 * Time: 11:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using SharpReportCore.Exporters;

namespace SharpReportCore
{
	/// <summary>
	/// Description of Interface1.
	/// </summary>
	public interface IExportColumnBuilder{
		BaseExportColumn CreateExportColumn (Graphics graphics);
	}
	/*
	public interface IPerformLine{
		BaseStyleDecorator StyleDecorator {
			get;set;
		}
	}
	*/
}
