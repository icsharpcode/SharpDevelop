// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>The type of the 'null'/'nothing' literal.</summary>
	public sealed class NullReturnType : AbstractReturnType
	{
		private NullReturnType() {}
		
		public static readonly NullReturnType Instance = new NullReturnType();
		
		public override bool Equals(IReturnType o)
		{
			return o is NullReturnType;
		}
		
		public override int GetHashCode()
		{
			return 0;
		}
		
		public override bool IsDefaultReturnType {
			get {
				return false;
			}
		}
		
		public override IClass GetUnderlyingClass()     { return null; }
		public override List<IMethod>   GetMethods()    { return new List<IMethod>(); }
		public override List<IProperty> GetProperties() { return new List<IProperty>(); }
		public override List<IField>    GetFields()     { return new List<IField>(); }
		public override List<IEvent>    GetEvents()     { return new List<IEvent>(); }
	}
}
