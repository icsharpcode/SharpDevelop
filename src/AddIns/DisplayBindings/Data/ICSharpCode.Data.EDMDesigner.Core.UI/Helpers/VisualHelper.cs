// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#region Usings

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Input;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Helpers
{
	/// <summary>
	/// Helperklasse, um das Navigieren durch Visual-Trees, suchen von Resourcen in den Visuals und etc... zu erleichtern
	/// </summary>
	public class VisualHelper
	{		
		/// <summary>
		/// Rekursiv durch den Baum des DependencyObjects durchgehen
		/// </summary>
		/// <param name="visual"></param>
		/// <returns></returns>
		public static IEnumerable<DependencyObject> GetAllVisualChildren(DependencyObject visual)
		{
			int childrenCount = VisualTreeHelper.GetChildrenCount(visual);
			for (int i = 0; i < childrenCount; i++)
			{
				// das Child retournieren 
				DependencyObject child = VisualTreeHelper.GetChild(visual, i);
				yield return child;

				// Rekursiv hinunter
				if (VisualTreeHelper.GetChildrenCount(child) > 0)
					foreach (DependencyObject grandChild in GetAllVisualChildren(child))
						yield return grandChild;
			}
		}

		/// <summary>
		/// liefert das FrameworkElement mit angegebenen Namen
		/// </summary>
		/// <param name="visualParent"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static FrameworkElement GetChild(DependencyObject visualParent, string name)
		{

			// Animation zum Anzeigen der neuen Grid starten
			foreach (DependencyObject child in VisualHelper.GetAllVisualChildren(visualParent))
			{
				FrameworkElement childElement = child as FrameworkElement;

				if (childElement != null && childElement.Name != string.Empty)
					System.Diagnostics.Debug.WriteLine(string.Format("VisualHelper.GetChild: such nach [{0}] child = {1}", name, childElement.Name));

				if (childElement != null && childElement.Name == name)
				{
					return childElement;
				}
			}
			return null;
		}

		/// <summary>
		/// sucht ein SubElement eines 'templated' FrameworkElements
		/// </summary>
		/// <param name="subElementName"></param>
		/// <param name="control"></param>
		/// <returns></returns>
		public static ElementType TryFindSubElement<ElementType>(string subElementName, Control control) where ElementType : class
		{
			if (control == null || control.Template == null)
				return null;

			// sucht das Element
			object subObject = control.Template.FindName(subElementName, control);
			if (subObject == null)
				return null;

			ElementType subElement = subObject as ElementType;
			if (subElement == null)
				return null;

			return subElement;
		}

		/// <summary>
		/// sucht ein SubElement eines 'templated' FrameworkElements
		/// </summary>
		/// <param name="subElementName"></param>
		/// <param name="control"></param>
		/// <returns></returns>
		public static ElementType FindSubElement<ElementType>(string subElementName, Control control) where ElementType : class
		{
			if (control == null || control.Template == null)
			{
				throw new Exception("Element hat kein Template!");
			}

			// sucht das Element
			object subObject = control.Template.FindName(subElementName, control);
			if (subObject == null)
			{
				throw new Exception("Template-Subelement nicht gefunden!");
			}

			ElementType subElement = subObject as ElementType;
			if (subElement == null)
			{
				throw new Exception("Template-Subelement nicht gefunden!");
			}

			return subElement;
		}

		/// <summary>
		/// Sucht die Resource in dem FrameworkElement (dem 'hierarchischen Baum') aufwärts
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="element"></param>
		/// <returns></returns>
		public static T FindResource<T>(string key, FrameworkElement element)
		{
			try
			{
				T value = (T)element.FindResource(key);
				return value;
			}
			catch (ResourceReferenceKeyNotFoundException)
			{
				throw new Exception("Control hat kein Template!");
			}
		}

		/// <summary>
		/// Sucht die Resource in dem FrameworkElement (dem 'hierarchischen Baum') aufwärts, 
		/// falls nicht gefunden, wird der Defaultwerte zurückgegeben
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="element"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static T FindResource<T>(string key, FrameworkElement element, T defaultValue)
		{
			try
			{
				return (T)element.FindResource(key);
			}
			catch (ResourceReferenceKeyNotFoundException)
			{
				return defaultValue;
			}
			catch (Exception)
			{
				throw new Exception("Control hat kein Template!");
			}
		}

		/// <summary>
		/// Sucht das Element unter der Mouse mit dem Namen name
		/// </summary>
		/// <param name="reference"></param>
		/// <param name="mousePosition"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static FrameworkElement FindElementUnderPointer(Visual reference, Point mousePosition, string name)
		{
			// Hit-test to find out the ItemsControl under the mouse-pointer.
			_element = null;
			_hittedElements = null;
			_name = name;
			VisualTreeHelper.HitTest(reference,
				new HitTestFilterCallback(VisualHelper.FilterHitTestKeyCallback),
				new HitTestResultCallback(VisualHelper.HitTestResultCallback),
				new PointHitTestParameters(mousePosition));

			try
			{
				return _element;
			}
			finally
			{
				_element = null;
				_hittedElements = null;
				_name = null;
				_type = null;
			}
		}

		/// <summary>
		/// Sucht das Element unter der Mouse des Typs type
		/// </summary>
		/// <param name="reference"></param>
		/// <param name="mousePosition"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static FrameworkElement FindElementUnderPointer(Visual reference, Point mousePosition, Type type)
		{
			// Hit-test to find out the ItemsControl under the mouse-pointer.
			_element = null;
			_hittedElements = null;
			_type = type;
			VisualTreeHelper.HitTest(reference,
				new HitTestFilterCallback(VisualHelper.FilterHitTestTypeCallback),
				new HitTestResultCallback(VisualHelper.HitTestResultCallback),
				new PointHitTestParameters(mousePosition));

			try
			{
				return _element;
			}
			finally
			{
				_element = null;
				_name = null;
				_hittedElements = null;
				_type = null;
			}
		}

		public static IEnumerable<FrameworkElement> GetElementsUnderPointer(Visual reference, Point mousePosition)
		{
			// Hit-test to find out the ItemsControl under the mouse-pointer.
			_element = null;
			_hittedElements = new List<FrameworkElement>();
			_type = null;

			VisualTreeHelper.HitTest(reference,
				new HitTestFilterCallback(VisualHelper.NoFilterHitTestCallback),
				new HitTestResultCallback(VisualHelper.HitTestResultCallback),
				new PointHitTestParameters(mousePosition));

			try
			{
				return _hittedElements;
			}
			finally
			{
				_element = null;
				_hittedElements = null;
				_name = null;
				_type = null;
			}
		}

		private static FrameworkElement _element = null;
		private static List<FrameworkElement> _hittedElements = null;
		private static string _name = null;
		private static Type _type = null;

		private static HitTestFilterBehavior FilterHitTestKeyCallback(DependencyObject target)
		{
			FrameworkElement element = target as FrameworkElement;
			if (element != null && element.Name == _name)
			{
				if (_hittedElements != null)
					_hittedElements.Add(element);
				_element = element;
				return HitTestFilterBehavior.Stop;
			}
			else
			{
				if (_hittedElements != null)
					_hittedElements.Add(element);
				return HitTestFilterBehavior.Continue;
			}
		}

		private static HitTestFilterBehavior FilterHitTestTypeCallback(DependencyObject target)
		{
			if (target.GetType().IsAssignableFrom(_type))
			{
				_element = target as FrameworkElement;
				if (_hittedElements != null)
					_hittedElements.Add(_element);
				return HitTestFilterBehavior.Stop;
			}
			else
			{
				if (_hittedElements != null)
					_hittedElements.Add(target as FrameworkElement);
				return HitTestFilterBehavior.Continue;
			}
		}

		private static HitTestFilterBehavior NoFilterHitTestCallback(DependencyObject target)
		{
			if (_hittedElements != null && target is FrameworkElement && (target as FrameworkElement).IsVisible)
				_hittedElements.Add(target as FrameworkElement);
			return HitTestFilterBehavior.Continue;
		}

		private static HitTestResultBehavior HitTestResultCallback(HitTestResult result)
		{
			return HitTestResultBehavior.Continue;
		}

		/// <summary>
		/// Gibt ein visuelles Child eines bestimmten Typs eines DependencyObjects zurück.
		/// </summary>
		/// <typeparam name="TChild"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static TChild GetVisualChild<TChild>(DependencyObject obj) where TChild : DependencyObject
		{
			if (obj == null)
				return null;

			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				// TODO: Was passiert hier, wenn ich kein Visual, sondern z.B. ein Run will?
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child != null && child is TChild)
					return (TChild)child;
				else
				{
					TChild childOfChild = GetVisualChild<TChild>(child);
					if (childOfChild != null)
						return childOfChild;
				}
			}
			return null;
		}

		/// <summary>
		/// Gibt ein visuelles Child mit einem bestimmten Namen eines DependencyObjects zurück.
		/// </summary>
		/// <typeparam name="TChild"></typeparam>
		/// <param name="obj"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static TChild GetVisualChild<TChild>(DependencyObject obj, string name) where TChild : FrameworkElement
		{
			if (obj == null)
				return null;

			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				// TODO: Was passiert hier, wenn ich kein Visual, sondern z.B. ein Run will?
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child != null && child is TChild && ((TChild)child).Name == name)
					return (TChild)child;
				else
				{
					TChild childOfChild = GetVisualChild<TChild>(child, name);
					if (childOfChild != null)
						return childOfChild;
				}
			}
			return null;
		}

		/// <summary>
		/// Gibt ein visuelles Child mit einem bestimmten Namen eines DependencyObjects zurück.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <remarks>Ich habe diese Methode hier aus Rückwärtskompatibilitätsgründen zurückgelassen.</remarks>
		public static FrameworkElement GetVisualChild(DependencyObject obj, string name)
		{
			return GetVisualChild<FrameworkElement>(obj, name);
		}

		/// <summary>
		/// Gibt ein visuelles Child, welches eine übergebene Bedingung erfüllt zurück
		/// </summary>
		/// <typeparam name="TChild"></typeparam>
		/// <param name="obj"></param>
		/// <param name="comparePredicate"></param>
		/// <returns></returns>
		public static TChild GetVisualChild<TChild>(DependencyObject obj, Func<DependencyObject, bool> comparePredicate) where TChild : FrameworkElement
		{
			if (obj == null)
				return null;

			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				// TODO: Was passiert hier, wenn ich kein Visual, sondern z.B. ein Run will?
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child != null && child is TChild && child is DependencyObject && comparePredicate(child as DependencyObject))
					return (TChild)child;
				else
				{
					TChild childOfChild = GetVisualChild<TChild>(child, comparePredicate);
					if (childOfChild != null)
						return childOfChild;
				}
			}
			return null;
		}

		/// <summary>
		/// Gibt ein visuelles Child, welches eine übergebene Bedingung erfüllt zurück
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <remarks>Ich habe diese Methode hier aus Rückwärtskompatibilitätsgründen zurückgelassen.</remarks>
		public static FrameworkElement GetVisualChild(DependencyObject obj, Func<DependencyObject, bool> comparePredicate)
		{
			return GetVisualChild<FrameworkElement>(obj, comparePredicate);
		}

		/// <summary>
		/// Gibt einen visuellen Parent eines bestimmten Typs eines DependencyObjects zurück.
		/// </summary>
		/// <typeparam name="TParent"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static TParent GetVisualParent<TParent>(DependencyObject obj) where TParent : DependencyObject
		{
			return GetVisualParent<TParent>(obj, true);
		}

		/// <summary>
		/// Gibt einen visuellen Parent mit einem speziellen Namen eines DependencyObjects zurück.
		/// </summary>
		/// <typeparam name="TParent"></typeparam>
		/// <param name="obj"></param>
		/// <param name="parentName"></param>
		/// <returns></returns>
		public static FrameworkElement GetVisualParent(DependencyObject obj, string parentName)
		{
			return GetVisualParent<FrameworkElement>(obj, true, parent => ((FrameworkElement)parent).Name == parentName);
		}

		/// <summary>
		/// Gibt einen visuellen Parent eines bestimmten Typs eines DependencyObjects zurück.
		/// </summary>
		/// <typeparam name="TParent"></typeparam>
		/// <param name="obj"></param>
		/// <param name="walkThroughPopupRoot">Gibt an, ob auch über einen PopupRoot weitergemacht werden soll.</param>
		/// <returns></returns>
		public static TParent GetVisualParent<TParent>(DependencyObject obj, bool walkThroughPopupRoot) where TParent : DependencyObject
		{
			if (obj == null)
				return null;

			return GetVisualParent<TParent>(obj, walkThroughPopupRoot,
				parent => parent != null && parent is TParent);
		}

		/// <summary>
		/// Gibt einen visuellen Parent eines bestimmten Typs eines DependencyObjects zurück.
		/// </summary>
		/// <typeparam name="TParent"></typeparam>
		/// <param name="obj"></param>
		/// <param name="walkThroughPopupRoot">Gibt an, ob auch über einen PopupRoot weitergemacht werden soll.</param>
		/// <param name="comparePredicate">Func mit dem Parent-Vergleich</param>
		/// <returns></returns>
		public static TParent GetVisualParent<TParent>(DependencyObject obj, bool walkThroughPopupRoot,
			Func<DependencyObject, bool> comparePredicate)
				where TParent : DependencyObject
		{
			if (obj == null)
				return null;

			// Wenn es kein Visual (z.B. Run) oder ein PopupRoot ist, dann komme ich nur im LogicalTree weiter voran...
			DependencyObject parent = (!(obj is Visual) || (walkThroughPopupRoot && obj.GetType().Name == "PopupRoot")) ?
				LogicalTreeHelper.GetParent(obj) : VisualTreeHelper.GetParent(obj);
			if (comparePredicate(parent))
				return (TParent)parent;
			else
				return GetVisualParent<TParent>(parent, walkThroughPopupRoot, comparePredicate);
		}

		public static object GetVisualParentObject(DependencyObject obj, bool walkThroughPopupRoot,
			Func<DependencyObject, bool> comparePredicate)
		{
			if (obj == null)
				return null;

			// Wenn es kein Visual (z.B. Run) oder ein PopupRoot ist, dann komme ich nur im LogicalTree weiter voran...
			DependencyObject parent = (!(obj is Visual) || (walkThroughPopupRoot && obj.GetType().Name == "PopupRoot")) ?
				LogicalTreeHelper.GetParent(obj) : VisualTreeHelper.GetParent(obj);
			if (comparePredicate(parent))
				return parent;
			else
				return GetVisualParentObject(parent, walkThroughPopupRoot, comparePredicate);
		}
	}
}
