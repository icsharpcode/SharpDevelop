// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Alpert" email="david@spinthemoose.com"/>
//     <version>$Revision:  $</version>
// </file>

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
