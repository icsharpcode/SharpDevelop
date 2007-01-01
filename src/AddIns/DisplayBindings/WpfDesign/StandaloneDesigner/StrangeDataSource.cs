// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.WpfDesign.PropertyEditor;
using System.Windows;
using System.Windows.Threading;

namespace StandaloneDesigner
{
	public class StrangeDataSource : IPropertyEditorDataSource
	{
		string name;
		
		public string Name {
			get { return name; }
			set {
				name = value;
				
				if (NameChanged != null) {
					NameChanged(this, EventArgs.Empty);
				}
			}
		}
		
		public StrangeDataSource()
		{
			// set the name of this data source to the current time
			DispatcherTimer t = new DispatcherTimer();
			t.Interval = TimeSpan.FromSeconds(1);
			t.Tick += delegate {
				this.Name = DateTime.Now.ToString();
			};
			t.Start();
		}
		
		public event EventHandler NameChanged;
		
		public string Type {
			get {
				return "Strange";
			}
		}
		
		public System.Windows.Media.ImageSource Icon {
			get {
				return null;
			}
		}
		
		public System.Collections.Generic.ICollection<IPropertyEditorDataProperty> Properties {
			get {
				return new IPropertyEditorDataProperty[0];
			}
		}
		
		public bool CanAddAttachedProperties {
			get {
				return false;
			}
		}
	}
}
