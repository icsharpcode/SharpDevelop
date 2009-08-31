// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Markup extension that retrieves localized resource strings.
	/// </summary>
	[MarkupExtensionReturnType(typeof(string))]
	public sealed class LocalizeExtension : MarkupExtension, INotifyPropertyChanged, IWeakEventListener
	{
		public LocalizeExtension(string key)
		{
			this.key = key;
			this.UsesAccessors = true;
			this.UpdateOnLanguageChange = true;
		}
		
		string key;
		
		/// <summary>
		/// Set whether the text uses accessors.
		/// If set to true (default), accessors will be converted to WPF syntax.
		/// </summary>
		public bool UsesAccessors { get; set; }
		
		/// <summary>
		/// Set whether the LocalizeExtension should use a binding to automatically
		/// change the text on language changes.
		/// The default value is true.
		/// </summary>
		public bool UpdateOnLanguageChange { get; set; }
		
		bool isRegisteredForLanguageChange;
		
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (UpdateOnLanguageChange) {
				if (!isRegisteredForLanguageChange) {
					isRegisteredForLanguageChange = true;
					LanguageChangeWeakEventManager.AddListener(this);
				}
				return (new Binding("Value") { Source = this }).ProvideValue(serviceProvider);
			} else {
				return this.Value;
			}
		}
		
		public string Value {
			get {
				try {
					string result = ResourceService.GetString(key);
					if (UsesAccessors)
						result = MenuService.ConvertLabel(result);
					return result;
				} catch (ResourceNotFoundException) {
					return "{Localize:" + key + "}";
				}
			}
		}
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		static readonly System.ComponentModel.PropertyChangedEventArgs
			valueChangedEventArgs = new System.ComponentModel.PropertyChangedEventArgs("Value");
		
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, valueChangedEventArgs);
			return true;
		}
	}
}
