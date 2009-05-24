using System;
using System.Windows;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
		/// <summary>
		/// Stores details about command binding
		/// </summary>
		public class CommandBindingInfo 
		{
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="contextName">Context full name</param>
			/// <param name="routedCommandName">Name of routed UI command which triggers this binding</param>
			/// <param name="className">Command full name</param>
			/// <param name="addIn">Add-in where command is registered</param>
			/// <param name="isLazy">Lazy load command</param>
			public CommandBindingInfo(string contextName, string routedCommandName, string className, AddIn addIn, bool isLazy) {
				RoutedCommandName = routedCommandName;
				ContextName = contextName;
				ClassName = className;
				IsLazy = isLazy;
				AddIn = addIn;
			}
			
			/// <summary>
			/// Routed command name
			/// 
			/// Described binding is triggered by this routed command
			/// </summary>
			/// <seealso cref="RoutedCommand"></seealso>
			public string RoutedCommandName { 
				get; private set;
			}
			
			/// <summary>
			/// Routed command instance
			/// 
			/// Described binding is triggered by this routed command
			/// </summary>
			/// <seealso cref="RoutedCommandName"></seealso>
			public RoutedUICommand RoutedCommand { 
				get {
					return CommandsRegistry.GetRoutedUICommand(RoutedCommandName);
				}
			}
			
			/// <summary>
			/// Add-in to which binded command belongs
			/// </summary>
			public AddIn AddIn {
				get; private set;
			}
			
			/// <summary>
			/// Binded command full name
			/// 
			/// This command is invoke when this binding is triggered
			/// </summary>
			/// <seealso cref="Class"></seealso>
			public string ClassName {
				get; private set;
			}
			
			/// <summary>
			/// Binded command instance
			/// 
			/// This command is invoke when this binding is triggered. If this value is equal 
			/// to null then add-in is not loaded yet, see <see cref="IsLazy">IsLazy</see> attribute
			/// for details
			/// </summary>
			/// <seealso cref="ClassName"></seealso>
			public System.Windows.Input.ICommand Class { 
				get {
					System.Windows.Input.ICommand command;
					CommandsRegistry.commands.TryGetValue(ClassName, out command);
					
					return command;
				}
			}
			
			/// <summary>
			/// Context class full name
			/// 
			/// Described binding will be valid in this context
			/// </summary>
			public string ContextName{
				get; private set;
			}
						
			/// <summary>
			/// Context class instance
			/// 
			/// Described binding will be valid in this context
			/// </summary>
			public UIElement Context { 
				get {
					UIElement context;
					CommandsRegistry.contexts.TryGetValue(ContextName, out context);
					
					return context;
				}
			}
			
			/// <summary>
			/// Lazy load
			/// 
			/// If lazy load is enabled then all add-in references are loaded when this 
			/// command is invoked. Otherwice if add-in is not loaded and IsLazy is set
			/// to false then this binding can't be triggered until add-in is loaded.
			/// </summary>
			public bool IsLazy{
				get; private set;
			}
		}
}
