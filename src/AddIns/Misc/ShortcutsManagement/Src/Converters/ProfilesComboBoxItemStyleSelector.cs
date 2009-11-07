using System.Windows;
using System.Windows.Controls;
using ICSharpCode.ShortcutsManagement.Data;

namespace ICSharpCode.ShortcutsManagement.Converters
{
	/// <summary>
	/// Selects profilesComboBox item style
	/// </summary>
	public class ProfilesComboBoxItemStyleSelector : StyleSelector
	{
		/// <summary>
		/// Selects profilesComboBox item style
		/// </summary>
		/// <param name="item">Item contained in ComboBoxItem</param>
		/// <param name="container">ComboBoxItem container</param>
		/// <returns>ComboBoxItem style</returns>
		public override Style SelectStyle(object item, DependencyObject container)
		{
			if(item is SeparatorData) {
				return (Style)((FrameworkElement)container).FindResource("SeparatorStyle");
			}
			
			if(item is UserGestureProfileAction) {
				return (Style)((FrameworkElement)container).FindResource("ProfileActionStyle");
			}
			
			return null;
		}
	}
}
