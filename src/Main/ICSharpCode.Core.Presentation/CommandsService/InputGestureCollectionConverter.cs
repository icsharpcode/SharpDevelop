using System;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Converts input gesture from text representation to object and vice versa
	/// 
	/// Will be able to convert mouse and key gestures and handle gestures collections
	/// </summary>
	public class InputGestureCollectionConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (sourceType == typeof(string)) {
				return true;
			}
			
			return base.CanConvertFrom(context, sourceType);
		}
		
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(string)) {
				return true;
			}
			
			return base.CanConvertTo(context, destinationType);
		}

		
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if (value is string) {
				var gestures = new InputGestureCollection();
				var serializedGestures = Regex.Split((string)value, " +| +");
				
				foreach(var serializedGesture in serializedGestures) {
					if(string.IsNullOrEmpty(serializedGesture)) continue;
					
					gestures.Add((InputGesture)new KeyGestureConverter().ConvertFromString(context, culture, serializedGesture));
				}
				
				return gestures;
			}
			
			return base.ConvertFrom(context, culture, value);
		}
		
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {  
			if(destinationType == typeof(string)) {
				var sb = new StringBuilder();
				var gesturesCollection = (InputGestureCollection)value;
				foreach(var gesture in gesturesCollection) {
					string serializedGesture;
					if (gesture is KeyGesture) {
						serializedGesture = (string)new KeyGestureConverter().ConvertTo(context, culture, gesture, typeof(string));
					} else if (gesture is MouseGesture) {
						serializedGesture = (string)new KeyGestureConverter().ConvertTo(context, culture, gesture, typeof(string));
					} else {
						continue;
					}
					
					sb.AppendFormat("{0} | ", serializedGesture);
				}
				
				return sb.Length >= 3 ? sb.ToString(0, sb.Length - 3) : sb.ToString();
			}
			
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}