/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 06.08.2010
 * Time: 19:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using ICSharpCode.Reports.Addin.Designer;
using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of BaseGroupedRow.
	/// </summary>
	[Designer(typeof(ICSharpCode.Reports.Addin.Designer.GroupedRowDesigner))]
	public class BaseGroupedRow:BaseRowItem
	{
		public BaseGroupedRow()
		{
		}
	}
}
