// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	/// <summary>
	/// Provides information about a member of a given object.
	/// In particular, it allows to access the value.
	/// </summary>
	public class ObjectMember: Variable
	{
		[Flags] 
		public enum Flags { Default = Public, None = 0, Public = 1, Static = 2, PublicStatic = Public | Static};
		
		Flags memberFlags;
		
		public Flags MemberFlags {
			get {
				return memberFlags; 	
			}
		}
		
		public bool IsStatic {
			get {
				return (memberFlags & Flags.Static) != 0;
			}
		}
		
		public bool IsPublic {
			get {
				return (memberFlags & Flags.Public) != 0;
			}
		}
		
		public ObjectMember(string name, Flags flags, Value @value)
			:base (name, @value)
		{
			this.memberFlags = flags;
		}
	}
}
