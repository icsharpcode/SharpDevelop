//
// Comment.cs
//
// Author:
//       Matej Miklečić <matej.miklecic@gmail.com>
//
// Copyright (c) 2013 Matej Miklečić (matej.miklecic@gmail.com)
//

namespace Comments
{
	/// <summary>
	///     This is a test file for all types of comments.
	/// </summary>
	class Comments
	{
		/// <summary>
		///     Comment method.
		/// </summary>
		/// <param name="id">
		///     Id.
		/// </param>
		void Comment(int id,      // id
		             string text) // text
		{
			int i; // i
			
			for (i = 0; i < 42 /* 42; */; i++) /*
			* Multi-line 
			*/
			{
				// break
				break;
			} // for
			
			/////////*
			
			while (true)
				// comments don't affect continuation
				;
			
			/********/ if (false) lock (this)
				// /*
				;
			
			{ /*/*/ }
		}
		/*/ still in comment } */
	}
}
