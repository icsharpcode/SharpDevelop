// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class InterfaceImplementation
	{
		TypeReference interfaceType;
		string memberName;
		
		public TypeReference InterfaceType {
			get {
				return interfaceType;
			}
		}
		
		public string MemberName {
			get {
				return memberName;
			}
		}
		
		public InterfaceImplementation(TypeReference interfaceType, string memberName)
		{
			this.interfaceType = interfaceType ?? TypeReference.Null;
			this.memberName = memberName ?? "?";
		}
	}
}
