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
using System.Linq;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using SD = ICSharpCode.SharpDevelop.Project;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class SolutionGlobalsTests
	{
		SolutionHelper solutionHelper;
		global::EnvDTE.Globals globals;
		
		void CreateSolution()
		{
			solutionHelper = new SolutionHelper();
			globals = solutionHelper.Solution.Globals;
		}
		
		void AddVariableToExtensibilityGlobals(string name)
		{
			AddVariableToExtensibilityGlobals(name, String.Empty);
		}
		
		void AddVariableToExtensibilityGlobals(string name, string value)
		{
			solutionHelper.AddVariableToExtensibilityGlobals(name, value);
		}
		
		void AddExtensibilityGlobalsSection()
		{
			solutionHelper.AddExtensibilityGlobalsSection();
		}
		
		string GetExtensibilityGlobalsSolutionItem(string name)
		{
			return solutionHelper.GetExtensibilityGlobalsSolutionItem(name);
		}
		
		SD.SolutionSection GetExtensibilityGlobalsSection()
		{
			return solutionHelper.GetExtensibilityGlobalsSection();
		}
		
		[Test]
		public void VariableExists_VariableExistsInExtensibilityGlobalsSection_ReturnsTrue()
		{
			CreateSolution();
			AddExtensibilityGlobalsSection();
			AddVariableToExtensibilityGlobals("test");
			
			bool exists = globals.get_VariableExists("test");
			
			Assert.IsTrue(exists);
		}
		
		[Test]
		public void VariableExists_NoExtensibilityGlobalsSection_ReturnsFalse()
		{
			CreateSolution();
			
			bool exists = globals.get_VariableExists("test");
			
			Assert.IsFalse(exists);
		}
		
		[Test]
		public void VariableExists_ExtensibilityGlobalsSectionExistsButHasNoVariables_ReturnsFalse()
		{
			CreateSolution();
			AddExtensibilityGlobalsSection();
			
			bool exists = globals.get_VariableExists("test");
			
			Assert.IsFalse(exists);
		}
		
		[Test]
		public void VariableExists_VariableExistsInExtensibilityGlobalsSectionWithDifferentCasing_ReturnsTrue()
		{
			CreateSolution();
			AddExtensibilityGlobalsSection();
			AddVariableToExtensibilityGlobals("TEST");
			
			bool exists = globals.get_VariableExists("test");
			
			Assert.IsTrue(exists);
		}
		
		[Test]
		public void VariableValue_VariableAddedToExtensibilityGlobalsSection_ReturnsVariableValue()
		{
			CreateSolution();
			AddExtensibilityGlobalsSection();
			AddVariableToExtensibilityGlobals("test", "test-value");
			
			object value = globals.get_VariableValue("test");
			
			Assert.AreEqual("test-value", value);
		}
		
		[Test]
		public void VariableValue_VariableAddedToExtensibilityGlobalsSectionWithDifferentCasing_ReturnsVariableValue()
		{
			CreateSolution();
			AddExtensibilityGlobalsSection();
			AddVariableToExtensibilityGlobals("TEST", "test-value");
			
			object value = globals.get_VariableValue("test");
			
			Assert.AreEqual("test-value", value);
		}
		
		[Test]
		public void VariableValue_NoExtensibilityGlobalsSection_ThrowInvalidArgumentException()
		{
			CreateSolution();
			
			Assert.Throws<ArgumentException>(delegate { object value = globals.get_VariableValue("test"); });
		}
		
		[Test]
		public void VariableValue_ExtensibilityGlobalsSectionButNoVariable_ThrowInvalidArgumentException()
		{
			CreateSolution();
			AddExtensibilityGlobalsSection();
			
			Assert.Throws<ArgumentException>(delegate { object value = globals.get_VariableValue("test"); });
		}
		
		[Test]
		public void VariableValue_SetNewValueForVariableAddedToExtensibilityGlobalsSection_SolutionItemUpdated()
		{
			CreateSolution();
			AddExtensibilityGlobalsSection();
			AddVariableToExtensibilityGlobals("test", "value");
			
			globals.set_VariableValue("test", "new-value");
			
			string item = GetExtensibilityGlobalsSolutionItem("test");
			Assert.AreEqual("new-value", item);
		}
		
		[Test]
		public void VariableValue_SetNewValueForVariableWhenExtensibilityGlobalsExistsButVariableDoesNotExist_SolutionItemUpdated()
		{
			CreateSolution();
			AddExtensibilityGlobalsSection();
			
			globals.set_VariableValue("test", "new-value");
			object value = globals.get_VariableValue("test");
			
			Assert.AreEqual("new-value", value);
		}
		
		[Test]
		public void VariableValue_SetNewValueForVariableWhenExtensibilityGlobalsExistsButVariableDoesNotExist_SolutionNotUpdatedWithNewSolutionItem()
		{
			CreateSolution();
			AddExtensibilityGlobalsSection();
			
			globals.set_VariableValue("test", "new-value");
			
			string item = GetExtensibilityGlobalsSolutionItem("test");
			Assert.IsNull(item);
		}
		
		[Test]
		public void VariableValue_SetNewValueForVariableWhenExtensibilityGlobalsDoesNotExist_ExtensibilityGlobalsSectionCreated()
		{
			CreateSolution();
			
			globals.set_VariableValue("test", "new-value");
			
			SD.SolutionSection section = GetExtensibilityGlobalsSection();
			Assert.IsNull(section);
		}
		
		[Test]
		public void VariableValue_SetNewValueForVariableWhenExtensibilityGlobalsDoesNotExistAndRetrieveItUsingDifferentCase_NewValueReturned()
		{
			CreateSolution();
			
			globals.set_VariableValue("test", "new-value");
			object value = globals.get_VariableValue("test");
			
			Assert.AreEqual("new-value", value);
		}
		
		[Test]
		public void VariableExists_AddNonPersistedVariable_ReturnsTrue()
		{
			CreateSolution();
			AddExtensibilityGlobalsSection();
			
			globals.set_VariableValue("test", "value");
			
			bool exists = globals.get_VariableExists("test");
			
			Assert.IsTrue(exists);
		}
		
		[Test]
		public void VariablePersists_NewVariableAdded_ReturnsFalse()
		{
			CreateSolution();
			globals.set_VariableValue("test", "new-value");
			
			bool persists = globals.get_VariablePersists("test");
			
			Assert.IsFalse(persists);
		}
		
		[Test]
		public void VariablePersists_VariableDoesNotExist_ExceptionThrown()
		{
			CreateSolution();
			
			Assert.Throws<ArgumentException>(delegate { bool persists = globals.get_VariablePersists("test"); });
		}
		
		[Test]
		public void VariablePersists_VariableExistsInSolution_ReturnsTrue()
		{
			CreateSolution();
			AddExtensibilityGlobalsSection();
			AddVariableToExtensibilityGlobals("test", "value");
			
			bool persists = globals.get_VariablePersists("test");
			
			Assert.IsTrue(persists);
		}
		
		[Test]
		public void VariablePersists_NoExistingExtensibilityGlobalsAndVariableSetToTrueAfterNewVariableAdded_VariableAddedToSolution()
		{
			CreateSolution();
			globals.set_VariableValue("test", "new-value");
			
			globals.set_VariablePersists("test", true);
			
			string item = GetExtensibilityGlobalsSolutionItem("test");
			Assert.AreEqual("new-value", item);
		}
		
		[Test]
		public void VariablePersists_SetToTrueAfterNewVariableAdded_ReturnsTrue()
		{
			CreateSolution();
			globals.set_VariableValue("test", "new-value");
			
			globals.set_VariablePersists("test", true);
			bool persists = globals.get_VariablePersists("test");
			
			Assert.IsTrue(persists);
		}
		
		[Test]
		public void VariablePersists_ExistingExtensibilityGlobalsAndSetToTrueAfterNewVariableAdded_VariableAddedToSolution()
		{
			CreateSolution();
			AddExtensibilityGlobalsSection();
			globals.set_VariableValue("test", "new-value");
			
			globals.set_VariablePersists("test", true);
			
			string item = GetExtensibilityGlobalsSolutionItem("test");
			Assert.AreEqual("new-value", item);
		}
		
		[Test]
		public void VariablePersists_SetToTrueAfterNewVariableAdded_ExtensibilityGlobalsSectionAddedWithPostSolutionType()
		{
			CreateSolution();
			globals.set_VariableValue("test", "new-value");
			
			globals.set_VariablePersists("test", true);
			
			SD.SolutionSection section = GetExtensibilityGlobalsSection();
			Assert.AreEqual("postSolution", section.SectionType);
		}
		
		[Test]
		public void VariablePersists_SetToFalseForVariableInSolution_ReturnsFalse()
		{
			CreateSolution();
			AddExtensibilityGlobalsSection();
			AddVariableToExtensibilityGlobals("test", "value");
			
			globals.set_VariablePersists("test", false);
			bool persists = globals.get_VariablePersists("test");
			
			Assert.IsFalse(persists);
		}
		
		[Test]
		public void VariablePersists_SetToFalseForVariableInSolution_VariableRemovedFromSolution()
		{
			CreateSolution();
			AddExtensibilityGlobalsSection();
			AddVariableToExtensibilityGlobals("test", "value");
			
			globals.set_VariablePersists("test", false);
			
			string item = GetExtensibilityGlobalsSolutionItem("test");
			Assert.IsNull(item);
		}
		
		[Test]
		public void VariablePersists_SetToFalseForVariableInSolutionButWithDifferentCase_VariableRemovedFromSolution()
		{
			CreateSolution();
			AddExtensibilityGlobalsSection();
			AddVariableToExtensibilityGlobals("test", "value");
			
			globals.set_VariablePersists("TEST", false);
			
			string item = GetExtensibilityGlobalsSolutionItem("test");
			Assert.IsNull(item);
		}
		
		[Test]
		public void VariableValue_AddNewVariableAndPersistThenUpdateVariableValue_SolutionValueUpdated()
		{
			CreateSolution();
			globals.set_VariableValue("test", "one");
			globals.set_VariablePersists("test", true);
			
			globals.set_VariableValue("test", "two");
			
			string item = GetExtensibilityGlobalsSolutionItem("test");
			Assert.AreEqual("two", item);
		}
		
		[Test]
		public void VariableValue_PersistVariableAfterSettingVariableValueTwice_NoExceptionThrown()
		{
			CreateSolution();
			globals.set_VariableValue("test", "two");
			globals.set_VariableValue("test", "three");
			globals.set_VariablePersists("test", true);
			
			object value = globals.get_VariableValue("test");
			
			Assert.AreEqual("three", value);
		}
		
		[Test]
		public void VariableValue_PersistVariableMultipleTimes_NoExceptionThrown()
		{
			CreateSolution();
			globals.set_VariableValue("test", "two");
			globals.set_VariablePersists("test", true);
			globals.set_VariablePersists("test", false);
			globals.set_VariablePersists("test", true);
			globals.set_VariableValue("test", "four");
			
			object value = globals.get_VariableValue("test");
			
			Assert.AreEqual("four", value);
		}
		
		[Test]
		public void VariablePersists_SetPersistToTrueTwice_VariableAddedToSolutionOnce()
		{
			CreateSolution();
			globals.set_VariableValue("test", "one");
			
			globals.set_VariablePersists("test", true);
			globals.set_VariablePersists("test", true);
			
			SD.SolutionSection section = GetExtensibilityGlobalsSection();
			int count = section.Keys.Count();
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void VariablePersists_SetPersistToFalseWhenNoExtensibilityGlobalsSection_ExceptionNotThrown()
		{
			CreateSolution();
			globals.set_VariableValue("test", "one");
			
			globals.set_VariablePersists("test", false);
			
			object value = globals.get_VariableValue("test");
			Assert.AreEqual("one", value);
		}
		
		[Test]
		public void VariablePersists_SetToTrueAfterNewVariableAdded_SolutionIsSaved()
		{
			CreateSolution();
			globals.set_VariableValue("test", "new-value");
			
			globals.set_VariablePersists("test", true);
			
			solutionHelper.AssertSolutionIsSaved();
		}
		
		[Test]
		public void VariablePersists_SetToFalseAfterNewVariableAdded_SolutionIsNotSaved()
		{
			CreateSolution();
			globals.set_VariableValue("test", "new-value");
			
			globals.set_VariablePersists("test", false);
			
			solutionHelper.AssertSolutionIsNotSaved();
		}
		
		[Test]
		public void VariablePersists_SetToFalseForVariableInSolution_SolutionSaved()
		{
			CreateSolution();
			AddExtensibilityGlobalsSection();
			AddVariableToExtensibilityGlobals("test", "value");
			
			globals.set_VariablePersists("test", false);
			
			solutionHelper.AssertSolutionIsSaved();
		}
		
		[Test]
		public void VariablePersists_SetToTrueForVariableInSolution_SolutionNotSaved()
		{
			CreateSolution();
			AddExtensibilityGlobalsSection();
			AddVariableToExtensibilityGlobals("test", "value");
			
			globals.set_VariablePersists("test", true);
			
			solutionHelper.AssertSolutionIsNotSaved();
		}
		
		[Test]
		public void VariableValue_VariableInSolutionChanged_SolutionIsSaved()
		{
			CreateSolution();
			AddExtensibilityGlobalsSection();
			AddVariableToExtensibilityGlobals("test", "value");
			
			globals.set_VariableValue("test", "new-value");
			
			solutionHelper.AssertSolutionIsSaved();
		}
		
		[Test]
		public void VariableValue_VariableNotSolutionChanged_SolutionIsNotSaved()
		{
			CreateSolution();
			globals.set_VariableValue("test", "value");
			
			globals.set_VariableValue("test", "new-value");
			
			solutionHelper.AssertSolutionIsNotSaved();
		}
	}
}
