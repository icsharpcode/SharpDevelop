using System;
using System.Diagnostics;

namespace NUnit.Framework
{
	/// <summary>
	/// Experimental class with static settings that affect tests.
	/// A setings may be saved and restored.  Currently only
	/// one setting - Tracing - is available.
	/// </summary>
	public class TestContext
	{
		/// <summary>
		/// The current context, head of the list of saved contexts.
		/// </summary>
		private static ContextHolder current = new ContextHolder();

		public static bool Tracing
		{
			get { return current.Tracing; }
			set { current.Tracing = value; }
		}
		
		/// <summary>
		/// Saves the old context and makes a fresh one 
		/// current without changing any settings.
		/// </summary>
		public static void Save()
		{
			TestContext.current = new ContextHolder( current );
		}

		/// <summary>
		/// Restores the last saved context and puts
		/// any saved settings back into effect.
		/// </summary>
		public static void Restore()
		{
			current.ReverseChanges();
			current = current.prior;
		}

		private TestContext() { }

		private class ContextHolder
		{
			/// <summary>
			/// Indicates whether trace is enabled
			/// </summary>
			private bool tracing;

			/// <summary>
			/// Link to a prior saved context
			/// </summary>
			public ContextHolder prior;

			public ContextHolder()
			{
				this.prior = null;
				this.tracing = false;
			}

			public ContextHolder( ContextHolder other )
			{
				this.prior = other;
				this.tracing = other.tracing;
			}

			/// <summary>
			/// Used to restore settings to their prior
			/// values before reverting to a prior context.
			/// </summary>
			public void ReverseChanges()
			{ 
				if ( prior == null )
				throw new InvalidOperationException( "TestContext: too many Restores" );

				this.Tracing = prior.Tracing;
			}

			/// <summary>
			/// Controls whether trace and debug output are written
			/// to the standard output.
			/// </summary>
			public bool Tracing
			{
				get { return tracing; }
				set 
				{ 
					if ( tracing != value )
					{
						tracing = value;
						if ( tracing )
							Trace.Listeners.Add( new TextWriterTraceListener( Console.Out, "NUnit" ) );
						else
							Trace.Listeners.Remove( "NUnit" );
					}
				}
			}

		}
	}
}
