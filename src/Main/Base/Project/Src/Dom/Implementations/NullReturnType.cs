// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	/// <summary>The type of the 'null'/'nothing' literal.</summary>
	public sealed class NullReturnType : AbstractReturnType
	{
		public static readonly NullReturnType Instance = new NullReturnType();
		
		public override bool Equals(object o)
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
