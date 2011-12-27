// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests.ServiceReferences
{
	[TestFixture]
	public class ServiceReferenceMapFileTests
	{
		ServiceReferenceMapFile lhs;
		ServiceReferenceMapFile rhs;
		
		void CreateMapFilesToCompare()
		{
			lhs = new ServiceReferenceMapFile();
			rhs = new ServiceReferenceMapFile();
		}
		
		void AssertFilesAreEqual()
		{
			bool result = AreFilesEqual();	
			Assert.IsTrue(result);
		}
		
		bool AreFilesEqual()
		{
			return lhs.Equals(rhs);
		}
		
		void AssertFilesAreNotEqual()
		{
			bool result = AreFilesEqual();
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Equals_FilesAreSame_ReturnsTrue()
		{
			CreateMapFilesToCompare();
			
			AssertFilesAreEqual();
		}
		
		[Test]
		public void Equals_FilesHaveDifferentFileNames_ReturnsFalse()
		{
			CreateMapFilesToCompare();
			lhs.FileName = "a";
			rhs.FileName = "b";
			
			AssertFilesAreNotEqual();
		}
	}
}
