using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows;

namespace SharpDevelop.XamlDesigner.Dom
{
	public class DesignProject
	{
		public DesignContext CreateContext(ITextHolder textHolder)
		{
			return new DesignContext(this, textHolder);
		}

		public List<Assembly> Assemblies = new List<Assembly>();

		static Assembly MscorlibAssembly = typeof(object).Assembly;
		static Assembly WindowsBaseAssembly = typeof(DependencyObject).Assembly;
		static Assembly PresentationCoreAssembly = typeof(UIElement).Assembly;
		static Assembly PresentationFrameworkAssembly = typeof(FrameworkElement).Assembly;

		static Assembly[] DefaultAssemblies = new Assembly[] {
			MscorlibAssembly,
			WindowsBaseAssembly,
			PresentationCoreAssembly,
			PresentationFrameworkAssembly
		};

		public IEnumerable<Type> GetAvailableTypes()
		{
			foreach (var assembly in Assemblies.Concat(DefaultAssemblies).Distinct()) {
				foreach (var type in assembly.GetExportedTypes()) {
					if (type.IsAbstract) continue;
					if (type.GetConstructor(Type.EmptyTypes) != null) {
						yield return type;
					}
				}
			}
		}
	}
}
