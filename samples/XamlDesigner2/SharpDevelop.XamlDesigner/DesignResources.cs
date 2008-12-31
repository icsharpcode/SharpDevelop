using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.IO;
using System.Reflection;
using System.Resources;

namespace SharpDevelop.XamlDesigner
{
	public static class DesignResources
	{
		static FrameworkElement dummy = new FrameworkElement();
		
		static ResourceManager resourceManager = new ResourceManager(
			typeof(DesignResources).Assembly.GetName().Name + ".g", typeof(DesignResources).Assembly);

		public static Stream GetStream(string path)
		{
			return resourceManager.GetStream(path.ToLower());
		}

		public static string GetString(string path)
		{
			var stream = GetStream(path);
			if (stream != null) {
				return new StreamReader(stream).ReadToEnd();
			}
			return null;
		}

		static ResourceKey CreateKey(object id)
		{
			return new ComponentResourceKey(typeof(DesignResources), id);
		}		

		static object FindResource(ResourceKey key)
		{
			return dummy.FindResource(key);
		}

		static DesignResources()
		{
			SnaplineStyleKey = CreateKey("SnaplineStyleKey");
		}

		public static ResourceKey SnaplineStyleKey { get; private set; }
		public static Style SnaplineStyle { get { return FindResource(SnaplineStyleKey) as Style; } }
	}
}
