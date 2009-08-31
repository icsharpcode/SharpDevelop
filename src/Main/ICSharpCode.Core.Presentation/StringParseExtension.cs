// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at" />
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Markup extension that works like StringParser.Parse
	/// </summary>
	[MarkupExtensionReturnType(typeof(string))]
	public sealed class StringParseExtension : MarkupExtension, INotifyPropertyChanged, IWeakEventListener
	{
		string text;
		
		public StringParseExtension(string text)
		{
			this.text = text;
			this.UsesAccessors = true;
			this.UpdateOnLanguageChange = true;
		}
		
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
				string result = StringParser.Parse(text);
				if (UsesAccessors)
					result = MenuService.ConvertLabel(result);
				return result;
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
