// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IMember : IDecoration, ICloneable
	{
		string FullyQualifiedName {
			get;
		}
		
		/// <summary>
		/// Declaration region of the member (without body!)
		/// </summary>
		DomRegion Region {
			get;
		}
		
		string Name {
			get;
		}
		
		string Namespace {
			get;
		}
		
		string DotNetName {
			get;
		}
		
		IReturnType ReturnType {
			get;
			set;
		}
	}
}
