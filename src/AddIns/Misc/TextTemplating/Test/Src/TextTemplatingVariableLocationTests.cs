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
