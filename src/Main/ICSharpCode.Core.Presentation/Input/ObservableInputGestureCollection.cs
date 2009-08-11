using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
    /// <summary>
    /// Represents observable collection of <see cref="InputGesture" />
    /// </summary>
    public class ObservableInputGestureCollection : IList<InputGesture>, INotifyCollectionChanged, IEnumerable<InputGesture>, ICollection<InputGesture>
    {
		private ObservableCollection<InputGesture> observedInputGestures;
    	
		/// <summary>
		/// Creates new instance of <see cref="ObservableInputGestureCollection" />
		/// </summary>
		public ObservableInputGestureCollection()
		{
			observedInputGestures = new ObservableCollection<InputGesture>();
			observedInputGestures.CollectionChanged += observedInputGestures_CollectionChanged;
		}
		
		/// <inheritdoc /> 
		public void Add(InputGesture item)
		{
			observedInputGestures.Add(item);
		}
		
		/// <inheritdoc />
		public void Insert(int index, InputGesture item)
		{
			observedInputGestures.Insert(index, item);
		}
		
		/// <inheritdoc />
		public void RemoveAt(int index)
		{
			observedInputGestures.RemoveAt(index);
		}
		
		/// <inheritdoc />
		public InputGesture this[int index] 
		{
			get {
				return observedInputGestures[index];
			}
			set {
				observedInputGestures[index] = value;
			}
		}
		
		/// <inheritdoc />
		public bool IsReadOnly
		{
			get {
				return false;
			}
		}
		
		/// <inheritdoc />
		public int Count
		{
			get {
				return observedInputGestures.Count;
			}
		}
		
		/// <inheritdoc />
		public bool Remove(InputGesture item)
		{
			return observedInputGestures.Remove(item);
		}
		
		/// <inheritdoc />
		public void Clear()
		{
			observedInputGestures.Clear();
		}
		
		/// <inheritdoc />
		public bool Contains(InputGesture item)
		{
			return observedInputGestures.Contains(item);
		}
		
		/// <summary>
		/// Add all instances from another <see cref="InputGestureCollection" />
		/// </summary>
		/// <param name="gestures">Collection of gestures</param>
		public void AddRange(InputGestureCollection gestures)
		{
			foreach(InputGesture item in gestures) {
				Add(item);
			}
		}
		
		/// <summary>
		/// Add all <see cref="InputGesture" /> instances from enumerable collection
		/// </summary>
		/// <param name="gestures">Collection of gestures</param>
		public void AddRange(IEnumerable<InputGesture> gestures)
		{
			foreach(InputGesture item in gestures) {
				Add(item);
			}
		}
		
		/// <inheritdoc />
		public void CopyTo(InputGesture[] array, int arrayIndex)
		{
			observedInputGestures.CopyTo(array, arrayIndex);
		}
		
		/// <inheritdoc />
		public IEnumerator<InputGesture> GetEnumerator()
		{
			return observedInputGestures.GetEnumerator();
		}
		
		/// <inheritdoc />
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return observedInputGestures.GetEnumerator();
		}

		/// <inheritdoc />
		public int IndexOf(InputGesture value)
		{
			return observedInputGestures.IndexOf(value);
		}
		
		/// <inheritdoc />
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		
		/// <inheritdoc />
		private void observedInputGestures_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if(CollectionChanged != null) {
				CollectionChanged.Invoke(sender, e);
			}
		}
		
		/// <summary>
		/// Gets <see cref="InputGestureCollection" /> containing all items from this observable collection
		/// </summary>
		public InputGestureCollection InputGesturesCollection
		{
			get {
				return new InputGestureCollection(observedInputGestures);
			}
		}
    }
}
