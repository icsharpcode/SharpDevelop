using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
	public delegate void NotifyGesturesChangedEventHandler(object sender, NotifyGesturesChangedEventArgs args);
	
    public class NotifyGesturesChangedEventArgs : EventArgs
    {
    	List<GesturesModificationDescription> _modificationDescriptions = new List<GesturesModificationDescription>();
		
		public NotifyGesturesChangedEventArgs()
		{
		}
		
		public NotifyGesturesChangedEventArgs(IEnumerable<GesturesModificationDescription> descriptions)
		{
			if(descriptions == null) {
				throw new ArgumentNullException("descriptions");
			}
			
			_modificationDescriptions.AddRange(descriptions);
		}
		
		public NotifyGesturesChangedEventArgs(GesturesModificationDescription description)
		{
			if(description == null) {
				throw new ArgumentNullException("description");
			}
			
			_modificationDescriptions.Add(description);
		}
    	
		public ICollection<GesturesModificationDescription> ModificationDescriptions
		{
			get
			{
				return _modificationDescriptions.AsReadOnly();
			}
		}
    }
    
    public class GesturesModificationDescription
    {
    	public GesturesModificationDescription(BindingInfoTemplate inputBindingInfoTemplate, InputGestureCollection oldGestures, InputGestureCollection newGestures)
    	{
    		InputBindingIdentifier = inputBindingInfoTemplate;
    		OldGestures = oldGestures;
    		NewGestures = newGestures;
    	}
    	
    	public BindingInfoTemplate InputBindingIdentifier
    	{
    		get; private set;
    	}
    	
    	public InputGestureCollection OldGestures
    	{
    		get; private set;
    	}
    	
    	public InputGestureCollection NewGestures
    	{
    		get; private set;
    	}
    }
}
