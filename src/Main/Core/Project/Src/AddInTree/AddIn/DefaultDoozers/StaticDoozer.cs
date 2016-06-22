// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Retrieves an object instance by accessing a static field or property
	/// via System.Reflection.
	/// </summary>
	/// <attribute name="class" use="required">
	/// The fully qualified type name of the class that contains the static field/property.
	/// </attribute>
	/// <attribute name="member" use="required">
	/// The name of the static field or property.
	/// </attribute>
	/// <usage>Everywhere where objects are expected.</usage>
	/// <returns>
	/// The value of the field/property.
	/// </returns>
	public class StaticDoozer : IDoozer
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
		
		public object BuildItem(BuildItemArgs args)
		{
			Codon codon = args.Codon;
			Type type = codon.AddIn.FindType(codon.Properties["class"]);
			if (type == null)
				return null;
			var memberName = codon.Properties["member"];
			var field = type.GetField(memberName);
			if (field != null)
				return field.GetValue(null);
			var property = type.GetProperty(memberName);
			if (property != null)
				return property.GetValue(null);
			throw new MissingFieldException("Field or property '" + memberName + "' not found in type " + type.FullName);
		}
	}
}
