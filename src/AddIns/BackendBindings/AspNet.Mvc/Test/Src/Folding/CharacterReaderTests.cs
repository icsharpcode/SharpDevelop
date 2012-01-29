// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc.Folding;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.Folding
{
	[TestFixture]
	public class CharacterReaderTests
	{
		CharacterReader reader;
		
		void CreateReader(string html)
		{
			reader = new CharacterReader(html);
		}
		
		[Test]
		public void ReadCharacters_ReadTwoCharacters_SecondCharacterIsCurrentCharacter()
		{
			CreateReader("12345");
			
			reader.ReadCharacters(2);
			
			Assert.AreEqual('2', reader.CurrentCharacter);
		}
	}
}
