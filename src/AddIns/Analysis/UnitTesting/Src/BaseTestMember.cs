// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents a test member that exists in a base class.
	/// </summary>
	/// <remarks>
	/// In order to have the Unit Test tree run the correct
	/// test when we have a class that has a base class with 
	/// test members is to return the derived class from the
	/// DeclaringType's property. Otherwise the base class
	/// member is tested and the derived class is not used.
	/// </remarks>
	public class BaseTestMember : AbstractMember
	{
		IMember member;
		
		/// <summary>
		/// Creates a new instance of the BaseTestMember.
		/// </summary>
		/// <param name="derivedClass">The derived class and not
		/// the class where the method is actually defined.</param>
		/// <param name="method">The base class's test member.</param>
		public BaseTestMember(IClass derivedClass, IMember member)
			: this(derivedClass, member, member.DeclaringType.Name + "." + member.Name)
		{
		}

		protected BaseTestMember(IClass derivedClass, IMember member, string name)
			: base(derivedClass, name)
		{
			this.member = member;
			this.ReturnType = member.ReturnType;
			this.Modifiers = member.Modifiers;
			this.Region = member.Region;
			this.BodyRegion = member.BodyRegion;
			this.Documentation = member.Documentation;
		}
		
		/// <summary>
		/// Gets the actual method used to create this object.
		/// </summary>
		public IMember Member {
			get { return member; }
		}
		
		public override EntityType EntityType {
			get {
				return member.EntityType;
			}
		}
		
		public override string DocumentationTag {
			get { 
				return null; 
			}
		}
		
		public override string Documentation {get;set;}

		public override IMember Clone()
		{
			return new BaseTestMember(DeclaringType, Member);
		}
	}
}
