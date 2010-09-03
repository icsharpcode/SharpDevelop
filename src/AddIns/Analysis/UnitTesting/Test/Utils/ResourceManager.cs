// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.Core;
using System;
using System.Reflection;

namespace UnitTesting.Tests.Utils
{
	/// <summary>
	/// This class is used to register any bitmaps used by 
	/// SharpDevelop so any references to the ResourceManager that
	/// ask for bitmaps succeed.
	/// </summary>
	public class ResourceManager
	{
		static bool initialized;
		
		ResourceManager()
		{
		}
		
		public static void Initialize()
		{
			if (!initialized) {
				initialized = true;
				Assembly exe = Assembly.Load("SharpDevelop");
				ResourceService.RegisterNeutralImages(new System.Resources.ResourceManager("Resources.BitmapResources", exe));
				ResourceService.RegisterNeutralStrings(new System.Resources.ResourceManager("Resources.StringResources", exe));
			}
		}
	}
}
