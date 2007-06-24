// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents a test method that exists in a base class.
	/// </summary>
	/// <remarks>
	/// In order to have the Unit Test tree run the correct
	/// test when we have a class that has a base class with 
	/// test methods is to return the derived class from the
	/// DeclaringType's property. Otherwise the base class
	/// method is tested and the derived class is not used.
	/// </remarks>
	public class BaseTestMethod : DefaultMethod
	{
		IMethod method;
		
		/// <summary>
		/// Creates a new instance of the BaseTestMethod.
		/// </summary>
		/// <param name="derivedClass">The derived class and not
		/// the class where the method is actually defined.</param>
		/// <param name="method">The base class's test method.</param>
		public BaseTestMethod(IClass derivedClass, IMethod method)
			: base(method.Name, method.ReturnType, method.Modifiers, method.Region, method.BodyRegion, derivedClass)
		{
			this.method = method;
		}
		
		/// <summary>
		/// Gets the actual method used to create this object.
		/// </summary>
		public IMethod Method {
			get { return method; }
		}
	}
}
