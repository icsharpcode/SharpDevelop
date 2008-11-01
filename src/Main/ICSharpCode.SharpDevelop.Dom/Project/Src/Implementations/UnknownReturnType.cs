// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	sealed class UnknownReturnType : ProxyReturnType
	{
		public static readonly UnknownReturnType Instance = new UnknownReturnType();
		
		public override IReturnType BaseType {
			get {
				return null;
			}
		}
	}
}
