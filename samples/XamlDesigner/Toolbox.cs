using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;
using ICSharpCode.XamlDesigner.Configuration;
using System.Windows;
using System.Collections.Specialized;
using ICSharpCode.WpfDesign;

namespace ICSharpCode.XamlDesigner
{
	public class Toolbox
	{
		public Toolbox()
		{
			AssemblyNodes = new ObservableCollection<AssemblyNode>();
			LoadSettings();
		}

		public static Toolbox Instance = new Toolbox();
		public ObservableCollection<AssemblyNode> AssemblyNodes { get; private set; }

		public void AddAssembly(string path)
		{
			AddAssembly(path, true);
		}

		void AddAssembly(string path, bool updateSettings)
		{
			var assembly = Assembly.LoadFile(path);
			
			MyTypeFinder.Instance.RegisterAssembly(assembly);
			
			var node = new AssemblyNode();
			node.Assembly = assembly;
			node.Path = path;
			foreach (var t in assembly.GetExportedTypes()) {
				if (IsControl(t) /* && Metadata.IsPopularControl(t) */) {
					node.Controls.Add(new ControlNode() { Type = t });
				}
			}

			node.Controls.Sort(delegate(ControlNode c1, ControlNode c2)  {
			                   	return c1.Name.CompareTo(c2.Name);
			                   });

			AssemblyNodes.Add(node);

			if (updateSettings) {
				if (Settings.Default.AssemblyList == null) {
					Settings.Default.AssemblyList = new StringCollection();
				}
				Settings.Default.AssemblyList.Add(path);
			}
		}

		public void Remove(AssemblyNode node)
		{
			AssemblyNodes.Remove(node);
			Settings.Default.AssemblyList.Remove(node.Path);
		}

		public void LoadSettings()
		{
			if (Settings.Default.AssemblyList != null) {
				foreach (var path in Settings.Default.AssemblyList) {
					try
					{
						AddAssembly(Environment.ExpandEnvironmentVariables(path), false);
					}
					catch (Exception)
					{ }
				}
			}
		}

		static bool IsControl(Type t)
		{
			return !t.IsAbstract && !t.IsGenericTypeDefinition && t.IsSubclassOf(typeof(UIElement)) && t.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null) != null;
		}
	}

	public class AssemblyNode
	{
		public AssemblyNode()
		{
			Controls = new List<ControlNode>();
		}

		public Assembly Assembly { get; set; }
		public List<ControlNode> Controls { get; private set; }
		public string Path { get; set; }

		public string Name {
			get { return Assembly.GetName().Name; }
		}
	}

	public class ControlNode
	{
		public Type Type { get; set; }

		public string Name {
			get { return Type.Name; }
		}
	}
}
