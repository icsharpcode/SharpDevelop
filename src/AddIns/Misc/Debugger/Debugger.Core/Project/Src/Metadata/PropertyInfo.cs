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
	/// Provides information about a property in a class
	/// </summary>
	public class PropertyInfo: MemberInfo
	{
		MethodInfo getMethod;
		MethodInfo setMethod;
		
		/// <summary> Gets a value indicating whether this property is private </summary>
		public override bool IsPrivate {
			get {
				return !(getMethod ?? setMethod).IsPublic;
			}
		}
		
		/// <summary> Gets a value indicating whether this property is public </summary>
		public override bool IsPublic {
			get {
				return (getMethod ?? setMethod).IsPublic;
			}
		}
		
		/// <summary> Gets a value indicating whether this property is static </summary>
		public override bool IsStatic {
			get {
				return (getMethod ?? setMethod).IsStatic;
			}
		}
		
		/// <summary> Gets the metadata token associated with getter (or setter)
		/// of this property </summary>
		[Debugger.Tests.Ignore]
		public override uint MetadataToken {
			get {
				return (getMethod ?? setMethod).MetadataToken;
			}
		}
		
		/// <summary> Gets the name of this property </summary>
		public override string Name {
			get {
				return (getMethod ?? setMethod).Name.Remove(0,4);
			}
		}
		
		internal PropertyInfo(DebugType declaringType, MethodInfo getMethod, MethodInfo setMethod): base(declaringType)
		{
			if (getMethod == null && setMethod == null) throw new ArgumentNullException("Both getter and setter can not be null.");
			
			this.getMethod = getMethod;
			this.setMethod = setMethod;
		}
		
		/// <summary> Get the get accessor of the property </summary>
		public MethodInfo GetMethod {
			get {
				return getMethod;
			}
		}
		
		/// <summary> Get the set accessor of the property </summary>
		public MethodInfo SetMethod {
			get {
				return setMethod;
			}
		}
	}
}
