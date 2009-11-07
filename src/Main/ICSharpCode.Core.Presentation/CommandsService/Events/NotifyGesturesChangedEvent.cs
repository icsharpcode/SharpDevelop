using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Represent a method that will handle <see cref="ICSharpCode.Core.Presentation.CommandManager.GesturesChanged" /> 
	/// or <see cref="ICSharpCode.Core.Presentation.UserGestureProfile.GesturesChanged" /> event
	/// </summary>
	public delegate void NotifyGesturesChangedEventHandler(object sender, NotifyGesturesChangedEventArgs args);
	
	/// <summary>
	/// Provides data for <see cref="ICSharpCode.Core.Presentation.CommandManager.GesturesChanged" /> 
	/// or <see cref="ICSharpCode.Core.Presentation.UserGestureProfile.GesturesChanged" /> event
	/// </summary>
    public class NotifyGesturesChangedEventArgs : EventArgs
    {
    	private bool _enforceUpdates;
    	private List<GesturesModificationDescription> _modificationDescriptions = new List<GesturesModificationDescription>();
    	
    	/// <summary>
    	/// When this property is set to <code>true</code> all handlers code should be executed
    	/// </summary>
    	public bool EnforceUpdates
    	{
    		get {
    			return _enforceUpdates;
    		}
    	}
    	
    	/// <summary>
    	/// Gets list of modification descriptions
    	/// </summary>
		public ICollection<GesturesModificationDescription> ModificationDescriptions
		{
			get
			{
				return _modificationDescriptions.AsReadOnly();
			}
		}
		
		/// <summary>
		/// Initializes new instance of <see cref="NotifyGesturesChangedEventArgs" /> that describes
		/// <see cref="NotifyGesturesChangedEventArgs.EnforceUpdates" /> event
		/// </summary>
		public NotifyGesturesChangedEventArgs()
		{
			_enforceUpdates = true;
		}
		
		/// <summary>
		/// Initializes new instance of <see cref="NotifyGesturesChangedEventArgs" /> containing a collection modification descriptions
		/// </summary>
		/// <param name="descriptions">Collection of modification descriptions</param>
		public NotifyGesturesChangedEventArgs(IEnumerable<GesturesModificationDescription> descriptions)
		{
			if(descriptions == null) {
				throw new ArgumentNullException("descriptions");
			}
			
			_modificationDescriptions.AddRange(descriptions);
		}
		
		/// <summary>
		/// Initializes new instance of <see cref="NotifyGesturesChangedEventArgs" /> containing one modification description
		/// </summary>
		/// <param name="descriptions">Modification description</param>
		public NotifyGesturesChangedEventArgs(GesturesModificationDescription description)
		{
			if(description == null) {
				throw new ArgumentNullException("description");
			}
			
			_modificationDescriptions.Add(description);
		}
    }
    
    /// <summary>
    /// Class describing single gestures modification in <see cref="InputBindingInfo" /> instance 
    /// </summary>
    public class GesturesModificationDescription
    {
    	private BindingInfoTemplate _inputBindingIdentifier;
    	private InputGestureCollection _oldGestures;
    	private InputGestureCollection _newGestures;
    	
    	/// <summary>
    	/// Creates new instance of <see cref="GesturesModificationDescription" />
    	/// </summary>
    	/// <param name="inputBindingInfoTemplate"><see cref="BindingInfoTemplate" /> which identify modified <see cref="InputBindingInfo" /></param>
    	/// <param name="oldGestures">Active <see cref="InputGestureCollection" /> before modification</param>
    	/// <param name="newGestures">Active <see cref="InputGestureCollection" /> after modification</param>
    	public GesturesModificationDescription(BindingInfoTemplate inputBindingInfoTemplate, InputGestureCollection oldGestures, InputGestureCollection newGestures)
    	{
    		_inputBindingIdentifier = inputBindingInfoTemplate;
    		_oldGestures = oldGestures;
    		_newGestures = newGestures;
    	}
    	
    	/// <summary>
    	/// <see cref="BindingInfoTemplate" /> which identifies modified <see cref="InputBindingInfo" />
    	/// </summary>
    	public BindingInfoTemplate InputBindingIdentifier
    	{
    		get {
    			return _inputBindingIdentifier;
    		}
    	}
    	
    	/// <summary>
    	/// Active <see cref="InputGestureCollection" /> before modification
    	/// </summary>
    	public InputGestureCollection OldGestures
    	{
    		get {
    			return _oldGestures;
    		}
    	}
    	
    	/// <summary>
    	/// Active <see cref="InputGestureCollection" /> after modification
    	/// </summary>
    	public InputGestureCollection NewGestures
    	{
    		get {
    			return _newGestures;
    		}
    	}
    }
}
