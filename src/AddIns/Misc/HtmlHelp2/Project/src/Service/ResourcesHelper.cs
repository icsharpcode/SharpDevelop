/* ***********************************************************
 *
 * Help 2.0 Environment for SharpDevelop
 * Base Help 2.0 Services
 * Copyright (c) 2005, Mathias Simmack. All rights reserved.
 *
 * ********************************************************* */
namespace HtmlHelp2Service
{
	using System;
	using System.Drawing;
	using System.Reflection;
	using System.Resources;

	public sealed class ResourcesHelper
	{
		static ResourcesHelper instance;

		static ResourcesHelper()
		{
			instance = new ResourcesHelper();
		}

		public static Bitmap GetBitmap(string resourceName)
		{
			Assembly assembly = typeof(ResourcesHelper).Assembly;
			string fullName   = String.Format("HtmlHelp2.Resources.{0}", resourceName);
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
