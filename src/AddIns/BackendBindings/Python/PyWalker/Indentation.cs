// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
