// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
			}
		}
	}
}
