// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace ICSharpCode.CodeQualityAnalysis
{
	public interface INode
	{
		string Name { get; }
		IDependency Dependency { get; }
		string GetInfo();
		Relationship GetRelationship(INode node);
		BitmapSource Icon { get; }
	}
}
