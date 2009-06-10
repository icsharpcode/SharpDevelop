using System;
using System.Windows;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
		/// <summary>
		/// Stores details about input binding
		/// </summary>
		public class InputBindingInfo
		{
			private UIElement contextInstance;
			
			public InputBindingInfo() {
				
			}
			
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="contextName">Context full name</param>
			/// <param name="routedCommandName">Name of routed UI command which is triggered by this binding</param>
			/// <param name="gesture">Gesture which triggers this binding</param>
			public InputBindingInfo(string contextName, string routedCommandName, InputGestureCollection gestures) {
				ContextName = contextName;
			    RoutedCommandName = routedCommandName; 
			    Gestures = gestures;
			}
			
			public InputBindingInfo(string contextName, UIElement contextInstance, string routedCommandName, InputGestureCollection gestures) {
				ContextName = contextName;
			    RoutedCommandName = routedCommandName; 
			    Gestures = gestures;
			    this.contextInstance = contextInstance;
			}
			
			
			/// <summary>
			/// Context class full name
			/// 
			/// Described binding will be valid in this context
			/// </summary>
			public string ContextName {
				get; set; 
			}
			
			/// <summary>
			/// Context class instance
			/// 
			/// Described binding will be valid in this context
			/// </summary>
			public UIElement Context { 
				get {
					if(contextInstance != null) {
						return contextInstance;
					} else {
						UIElement context;
						CommandsRegistry.contexts.TryGetValue(ContextName, out context);
						
						return context;
					}
				}
			}
			
			
			/// <summary>
			/// Routed command text
			/// 
			/// Override routed command text when displaying to user
			/// </summary>
			/// <seealso cref="RoutedCommand"></seealso>
			public string RoutedCommandText { 
				get; set;
			}
			
			
			/// <summary>
			/// Add-in to which registered this input binding
			/// </summary>
			public AddIn AddIn {
				get; set;
			}
			
			/// <summary>
			/// Routed command name
			/// 
			/// Described binding triggers this routed command
			/// </summary>
			/// <seealso cref="RoutedCommand"></seealso>
			public string RoutedCommandName { 
				get; set;
			}
			
			/// <summary>
			/// Routed command instance
			/// 
			/// Described binding triggers this routed command
			/// </summary>
			/// <seealso cref="RoutedCommandName"></seealso>
			public RoutedUICommand RoutedCommand { 
				get {
					return CommandsRegistry.GetRoutedUICommand(RoutedCommandName);
				}
			}
			
			/// <summary>
			/// Gestures which triggers this binding
			/// </summary>
			public InputGestureCollection Gestures { 
				get; set; 
			}
			
			public string CategoryName {
				get; set;
			}
		}
}
