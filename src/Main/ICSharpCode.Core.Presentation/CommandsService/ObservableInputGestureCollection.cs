/*
 * Created by SharpDevelop.
 * User: Sergej Andrejev
 * Date: 7/17/2009
 * Time: 8:56 AM
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
    /// <summary>
    /// Description of ObservableInputBindingCollection.
    /// </summary>
    public class ObservableInputGestureCollection : IList<InputGesture>, IEnumerable<InputGesture>, ICollection<InputGesture>
    {
		private ObservableCollection<InputGesture> observedInputGestures;
    	
		public ObservableInputGestureCollection()
		{
			observedInputGestures = new ObservableCollection<InputGesture>();
			observedInputGestures.CollectionChanged += observedInputGestures_CollectionChanged;
		}
		
		public void Add(InputGesture item)
		{
			observedInputGestures.Add(item);
		}
		
		public void Insert(int index, InputGesture item)
		{
			observedInputGestures.Insert(index, item);
		}
		
		public void RemoveAt(int index)
		{
			observedInputGestures.RemoveAt(index);
		}
		
		public InputGesture this[int index] 
		{
			get {
				return observedInputGestures[index];
			}
			set {
				observedInputGestures[index] = value;
			}
		}
		
		public bool IsReadOnly
		{
			get {
				return false;
			}
		}
		
		public int Count
		{
			get {
				return observedInputGestures.Count;
			}
		}
		
		public bool Remove(InputGesture item)
		{
			return observedInputGestures.Remove(item);
		}
		
		public void Clear()
		{
			observedInputGestures.Clear();
		}
		
		public bool Contains(InputGesture item)
		{
			return observedInputGestures.Contains(item);
		}
		
		public InputGestureCollection GetInputGestureCollection()
		{
			return new InputGestureCollection(observedInputGestures);
		}
		
		public void AddRange(InputGestureCollection items)
		{
			foreach(InputGesture item in items) {
				Add(item);
			}
		}
		
		public void AddRange(IEnumerable<InputGesture> items)
		{
			foreach(InputGesture item in items) {
				Add(item);
			}
		}
		
		public void CopyTo(InputGesture[] array, int arrayIndex)
		{
			observedInputGestures.CopyTo(array, arrayIndex);
		}
		
		public IEnumerator<InputGesture> GetEnumerator()
		{
			return observedInputGestures.GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return observedInputGestures.GetEnumerator();
		}

		public int IndexOf(InputGesture value)
		{
			return observedInputGestures.IndexOf(value);
		}
		
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		
		private void observedInputGestures_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if(CollectionChanged != null) {
				CollectionChanged.Invoke(sender, e);
			}
		}
    }
}
