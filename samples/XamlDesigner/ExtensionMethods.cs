using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;

namespace ICSharpCode.XamlDesigner
{
	static class ExtensionMethods
	{
		public static IEnumerable<string> Paths(this IDataObject data)
		{
			string[] paths = (string[])data.GetData(DataFormats.FileDrop);
			if (paths != null) {
				foreach (var path in paths) {
					yield return path;
				}
			}
		}

		public static T GetObject<T>(this IDataObject data)
		{
			return (T)data.GetData(typeof(T).FullName);
		}

		public static Stream ToStream(this string s)
		{
			return new MemoryStream(Encoding.UTF8.GetBytes(s));
		}

		public static void AddRange<T>(this ObservableCollection<T> col, IEnumerable<T> items)
		{
			foreach (var item in items) {
				col.Add(item);
			}
		}

		public static void KeepSyncronizedWith<S>(this IList target, ObservableCollection<S> source, Func<S, object> convert)
		{
			target.Clear();
			foreach (var item in source) {
				target.Add(convert(item));
			}

			source.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e) {
				switch (e.Action) {
					case NotifyCollectionChangedAction.Add:
						target.Add(convert((S)e.NewItems[0]));
						break;

					case NotifyCollectionChangedAction.Remove:
						target.RemoveAt(e.OldStartingIndex);
						break;

					case NotifyCollectionChangedAction.Move:
						target.RemoveAt(e.OldStartingIndex);
						target.Insert(e.NewStartingIndex, e.NewItems[0]);
						break;

					case NotifyCollectionChangedAction.Replace:
						target[e.NewStartingIndex] = convert((S)e.NewItems[0]);
						break;

					case NotifyCollectionChangedAction.Reset:
						target.Clear();
						break;
				}
			};
		}

		public static object GetDataContext(this RoutedEventArgs e)
		{
			var f = e.OriginalSource as FrameworkElement;
			if (f != null) return f.DataContext;
			return null;
		}
	}
}
