using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using ICSharpCode.WpfDesign.XamlDom;

namespace ICSharpCode.XamlDesigner
{
	public class MyTypeFinder : XamlTypeFinder
	{
		public override Assembly LoadAssembly(string name)
		{
			foreach (var assemblyNode in Toolbox.Instance.AssemblyNodes)
			{
				if (assemblyNode.Name == name)
					return assemblyNode.Assembly;
			}

			return null;
		}

		public override XamlTypeFinder Clone()
		{
			return _instance;
		}

		private static object lockObj = new object();

		private static MyTypeFinder _instance;
		public static MyTypeFinder Instance
		{
			get
			{
				lock (lockObj)
				{
					if (_instance == null)
					{
						_instance = new MyTypeFinder();
						_instance.ImportFrom(CreateWpfTypeFinder());
					}
				}

				return _instance;
			}
		}
	}
}
