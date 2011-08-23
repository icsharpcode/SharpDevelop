// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Exporter;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of Interface1.
	/// </summary>
	public interface IExportColumnBuilder{
		IBaseExportColumn CreateExportColumn ();
	}

}
