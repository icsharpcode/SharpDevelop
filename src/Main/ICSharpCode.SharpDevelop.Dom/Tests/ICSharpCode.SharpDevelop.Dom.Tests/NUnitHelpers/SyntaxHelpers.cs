// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.SyntaxHelpers
{
	/// <summary>
	/// Syntax helper class that generates custom constraints to test ICSharpCode.SharpDevelop.Dom objects
	/// </summary>
	public class HasEmpty {
		public static PropertyGetIsEmptyConstraint GetRegion {
			get { return new PropertyGetIsEmptyConstraint(); }
		}
		public static PropertySetIsEmptyConstraint SetRegion {
			get { return new PropertySetIsEmptyConstraint(); }
		}
		public static MethodBodyIsEmptyConstraint MethodBody {
			get { return new MethodBodyIsEmptyConstraint(); }
		}
	}
}
