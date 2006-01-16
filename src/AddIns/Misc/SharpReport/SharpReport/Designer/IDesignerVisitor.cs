/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 10.08.2005
 * Time: 22:14
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

/// <summary>
/// Each DesignControl Visitor has to implement this Interface
/// </summary>
using System;

namespace SharpReport.Designer{
	public interface IDesignerVisitor  {
		void Visit (SharpReport.Designer.BaseDesignerControl  designer);
		}
}
		
