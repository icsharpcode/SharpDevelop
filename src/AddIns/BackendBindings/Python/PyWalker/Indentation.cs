// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace PyWalker
{
	class Indentation : IDisposable
	{
		static int currentLevel;
		
		public static int CurrentLevel {
			get { return currentLevel; }
		}
		
		Indentation()
		{
			currentLevel++;
		}
		
		public void Dispose()
		{
			currentLevel--;
		}
		
		public static IDisposable IncrementLevel()
		{
			return new Indentation();
		}
	}
}
