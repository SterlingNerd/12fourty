using System;
using System.Collections.Generic;
using System.Linq;

namespace RefactorMoveFolderValidation;

public static class EnumerableExtensions
{
	public static bool None<T>(this IEnumerable<T> enumerable)
	{
		return !enumerable.Any();
	}	
	
	public static bool None<T>(this ICollection<T> collection)
	{
		return collection.Count == 0;
	}
}
