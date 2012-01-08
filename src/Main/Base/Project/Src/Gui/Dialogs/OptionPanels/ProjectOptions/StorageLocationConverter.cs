/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 14.11.2011
 * Time: 19:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class StorageLocationConverter:System.Windows.Data.IMultiValueConverter
	{
		
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
	//			Console.WriteLine ("Convert");
	//			foreach (var element in values) {
	//				Console.WriteLine(element.ToString());
	//			}
			return values[0];
		}
		
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			var s =   Array.ConvertAll<Type, Object>(targetTypes, t => value);
			return s;
		}
	}
}
