// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;

namespace ICSharpCode.Reports.Core.Interfaces
{
	/// <summary>
	/// Description of ILayouter.
	/// </summary>
	public interface ILayouter
	{
		Rectangle Layout (Graphics graphics,BaseSection section);
		Rectangle Layout (Graphics graphics,ISimpleContainer container);
	}
}
