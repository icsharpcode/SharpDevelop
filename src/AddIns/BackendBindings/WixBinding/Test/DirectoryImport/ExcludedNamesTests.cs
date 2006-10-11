// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;

namespace WixBinding.Tests.DirectoryImport
{
	/// <summary>
	/// Tests the ExcludedNames class's IsExcluded method.
	/// </summary>
	[TestFixture]
	public class ExcludedNamesTests
	{
		ExcludedNames excludedNames;
		
		[SetUp]
		public void Init()
		{
			excludedNames = new ExcludedNames();
		}
		
		[Test]
		public void AllTextFilesExcluded()
		{
			excludedNames.Add("*.txt");
			
			Assert.IsTrue(excludedNames.IsExcluded("readme.txt"));
		}
			
		[Test]
		public void CaseInsensitive()
		{
			excludedNames.Add("readme.txt");
			Assert.IsTrue(excludedNames.IsExcluded("README.TXT"));
		}
		
		[Test]
		public void SingleCharacterWildcard()
		{
			excludedNames.Add("test?.txt");
			
			Assert.IsTrue(excludedNames.IsExcluded("test1.txt"));
			Assert.IsFalse(excludedNames.IsExcluded("test.txt"));
		}
		
		[Test]
		public void RegexInExcludedName()
		{
			excludedNames.Add("?tes(ab)+.txt");
			
			Assert.IsTrue(excludedNames.IsExcluded("Ates(ab)+.txt"));
			Assert.IsFalse(excludedNames.IsExcluded("Atesababab.txt"));
		}
	}
}
