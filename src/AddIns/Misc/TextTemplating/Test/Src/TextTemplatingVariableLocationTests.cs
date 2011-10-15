// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.TextTemplating;
using NUnit.Framework;

namespace TextTemplating.Tests
{
	[TestFixture]
	public class TextTemplatingVariableLocationTests
	{
		TextTemplatingVariableLocation lhs;
		TextTemplatingVariableLocation rhs;
		
		void CreateVariableLocationsToCompare()
		{
			lhs = new TextTemplatingVariableLocation();
			rhs = new TextTemplatingVariableLocation();
		}
		
		[Test]
		public void Equals_AllPropertiesSame_ReturnsTrue()
		{
			CreateVariableLocationsToCompare();
			
			bool result = lhs.Equals(rhs);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Equals_VariableNamesAreDifferent_ReturnsFalse()
		{
			CreateVariableLocationsToCompare();
			lhs.VariableName = "A";
			rhs.VariableName = "B";
			
			bool result = lhs.Equals(rhs);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Equals_NullPassed_ReturnsFalse()
		{
			CreateVariableLocationsToCompare();
			
			bool result = lhs.Equals(null);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Equals_IndexesAreDifferent_ReturnsFalse()
		{
			CreateVariableLocationsToCompare();
			lhs.Index = 1;
			rhs.Index = 3;
			
			bool result = lhs.Equals(rhs);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Equals_LengthsAreDifferent_ReturnsFalse()
		{
			CreateVariableLocationsToCompare();
			lhs.Length = 1;
			rhs.Length = 3;
			
			bool result = lhs.Equals(rhs);
			
			Assert.IsFalse(result);
		}
	}
}
