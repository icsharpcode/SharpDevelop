#if !DEBUG
#define DEBUG
#endif

#if TRACE
#undef TRACE
#endif

namespace PreProcessorDirectives
{
	class PreProcessorDirectives
	{
		void IfDebug()
		{
			#region If/Elif Directives
			
#if DEBUG
			{
				// This block should be correctly indented
			}
#elif true
			{
			// This comment is not indented since the #if was true
			}
#endif
			
#if TRACE
			{
			// Not indented
			}
#elif debug
			{
			// Also not indented
			}
#else
			{
				// This should be indented
			}
#endif
			
			#endregion 
		}
		
		#region One-line directives
		
		void OneLiners
		{
			//
#pragma warning disable 649
			//
#warning 649
			//
#line 649
			//
#error 649
			//
		}
		
		#endregion 
	}
}
