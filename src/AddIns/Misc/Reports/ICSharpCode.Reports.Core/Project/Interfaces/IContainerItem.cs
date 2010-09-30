// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Expressions.ReportingLanguage;

namespace ICSharpCode.Reports.Core.Interfaces
{
	/// <summary>
	/// Description of IContainerControl.
	/// </summary>
	public interface ISimpleContainer{

		ReportItemCollection Items {get;}
		BaseReportItem Parent {set;get;}
		Point Location {set;get;}
		Size Size {get;set;}
		Color BackColor {get;set;}
	}
	
	
	public interface ITableContainer:ISimpleContainer
	{
		IDataNavigator DataNavigator {set;}
		void StartLayoutAt (BaseSection section);         
	}
}
