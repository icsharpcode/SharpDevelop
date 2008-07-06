// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.MetaData;

namespace Debugger.MetaData
{
	/// <summary>
	/// Provides information about a member of some class
	/// (eg. a field or a method).
	/// </summary>
	public abstract class MemberInfo: DebuggerObject
	{
		DebugType declaringType;
		
		/// <summary> Gets the process in which the type was loaded </summary>
		[Debugger.Tests.Ignore]
		public Process Process {
			get {
				return declaringType.Process;
			}
		}
		
		/// <summary> Gets the name of this member </summary>
		public abstract string Name { get; }
		
		/// <summary> Gets name of the method including the full name of the declaring type </summary>
		public string FullName {
			get {
				return this.DeclaringType.FullName + "." + this.Name;
			}
		}
		
		/// <summary> Gets the module in which this member is defined </summary>
		public Module Module {
			get {
				return declaringType.Module;
			}
		}
		
		/// <summary> Gets the type that declares this member element </summary>
		public DebugType DeclaringType {
			get {
				return declaringType;
			}
		}
		
		/// <summary> Gets a value indicating whether this member has the private access modifier</summary>
		public abstract bool IsPrivate { get; }
		
		/// <summary> Gets a value indicating whether this member has the internal access modifier</summary>
		public abstract bool IsInternal { get; }
		
		/// <summary> Gets a value indicating whether this member has the protected access modifier</summary>
		public abstract bool IsProtected { get; }
		
		/// <summary> Gets a value indicating whether this member has the public access modifier</summary>
		public abstract bool IsPublic { get; }
		
		/// <summary> Gets a value indicating whether this member is static </summary>
		public abstract bool IsStatic { get; }
		
		/// <summary> Gets the metadata token associated with this member </summary>
		[Debugger.Tests.Ignore]
		public abstract uint MetadataToken { get; }
		
		internal MemberInfo(DebugType declaringType)
		{
			this.declaringType = declaringType;
		}
		
		public override string ToString()
		{
			return this.Name;
		}
	}
}
