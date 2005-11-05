/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 05.11.2005
 * Time: 20:57
 */

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
