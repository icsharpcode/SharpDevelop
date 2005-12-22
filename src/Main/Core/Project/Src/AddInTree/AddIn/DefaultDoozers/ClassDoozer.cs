// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Creates object instances by invocating a type's parameterless constructor
	/// via System.Reflection.
	/// </summary>
	/// <attribute name="class" use="required">
	/// The fully qualified type name of the class to create an instace of.
	/// </attribute>
	/// <usage>Everywhere where objects are expected.</usage>
	/// <returns>
	/// Any kind of object.
	/// </returns>
	public class ClassDoozer : IDoozer
	{
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the condition is not met.
		/// </summary>
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			return codon.AddIn.CreateObject(codon.Properties["class"]);
		}
	}
}
