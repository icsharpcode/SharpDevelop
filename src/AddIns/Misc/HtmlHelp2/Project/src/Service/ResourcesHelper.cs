// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mathias Simmack" email="mathias@simmack.de"/>
//     <version>$Revision$</version>
// </file>

namespace HtmlHelp2.Environment
{
	using System;
	using System.Drawing;
	using System.Reflection;
	using System.Resources;

	public sealed class ResourcesHelper
	{
		static ResourcesHelper instance = new ResourcesHelper();

		public static Bitmap GetBitmap(string resourceName)
		{
			Assembly assembly = typeof(ResourcesHelper).Assembly;
			string fullName = string.Format(null, "HtmlHelp2.Resources.{0}", resourceName);
			return new Bitmap(assembly.GetManifestResourceStream(fullName));
		}

		public static Image GetImage(string imageName)
		{
			return instance.ImageResourceHelper.GetObject(imageName) as Image;
		}

		ResourceManager ImageResourceHelper;

		ResourcesHelper()
		{
			ImageResourceHelper = new ResourceManager("HtmlHelp2.Resources", GetType().Assembly);
		}
	}
}
