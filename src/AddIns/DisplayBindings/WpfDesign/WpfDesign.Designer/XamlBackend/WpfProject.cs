using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Xaml;

namespace ICSharpCode.WpfDesign.Designer.XamlBackend
{
	public class WpfProject : XamlProject
	{
		public WpfProject()
		{
			Metadata.RegisterAssembly(typeof(WpfProject).Assembly);
		}
	}

	public class DefaultWpfProject : WpfProject
	{
		public DefaultWpfProject()
		{
			AddReference(XamlConstants.MscorlibAssembly);
			AddReference(XamlConstants.WindowsBaseAssembly);
			AddReference(XamlConstants.PresentationCoreAssembly);
			AddReference(XamlConstants.PresentationFrameworkAssembly);

			foreach (var item in AllAssemblies) {
				TypeFinder.RegisterAssembly(item);
			}
		}
	}
}
