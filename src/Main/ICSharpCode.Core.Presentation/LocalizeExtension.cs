// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	public sealed class LocalizeExtension : LanguageDependentExtension
	{
		public LocalizeExtension(string key)
		{
			this.key = key;
			this.UsesAccessors = true;
			this.UpdateOnLanguageChange = true;
		}
		
		public LocalizeExtension()
		{
			this.UsesAccessors = true;
			this.UpdateOnLanguageChange = true;
		}
		
		string key;
		
		public string Key {
			get { return key; }
			set { key = value; }
		}
		
		/// <summary>
		/// Set whether the text uses accessors.
		/// If set to true (default), accessors will be converted to WPF syntax.
		/// </summary>
		public bool UsesAccessors { get; set; }
		
		public override string Value {
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
	}
	
	public abstract class LanguageDependentExtension : MarkupExtension, INotifyPropertyChanged, IWeakEventListener
	{
		protected LanguageDependentExtension()
		{
			this.UpdateOnLanguageChange = true;
		}
		
		public abstract string Value { get; }
		
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
				Binding binding = new Binding("Value") { Source = this, Mode = BindingMode.OneWay };
				return binding.ProvideValue(serviceProvider);
			} else {
				return this.Value;
			}
		}
		
		[Obsolete("Use ExtensionMethods.SetValueToExtension instead of directly fetching the binding from this extension")]
		public Binding CreateBinding()
		{
			return new Binding("Value") { Source = this, Mode = BindingMode.OneWay };
		}
		
		event System.ComponentModel.PropertyChangedEventHandler ChangedEvent;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged {
			add {
				if (!isRegisteredForLanguageChange) {
					isRegisteredForLanguageChange = true;
					LanguageChangeWeakEventManager.AddListener(this);
				}
				ChangedEvent += value;
			}
			remove { ChangedEvent -= value; }
		}
		
		static readonly System.ComponentModel.PropertyChangedEventArgs
			valueChangedEventArgs = new System.ComponentModel.PropertyChangedEventArgs("Value");
		
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			var handler = ChangedEvent;
			if (handler != null)
				handler(this, valueChangedEventArgs);
			return true;
		}
	}
}
