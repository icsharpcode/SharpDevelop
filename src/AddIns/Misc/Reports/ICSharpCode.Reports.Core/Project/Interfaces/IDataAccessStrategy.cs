// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Data;

namespace ICSharpCode.Reports.Core.Project.Interfaces
{
	/// <summary>
	/// Description of IDataAccessStrategy.
	/// </summary>
	public interface IDataAccessStrategy
	{
		bool OpenConnection ();
		DataSet ReadData();
	}
}
