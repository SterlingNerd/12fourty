using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace RefactorMoveFolderValidation;

public class Result
{
	public ReadOnlyCollection<string> Errors { get; private set; } = new(Array.Empty<string>());
	public bool IsSuccessful => Errors.None();

	public void AddErrorByCode(string code)
	{
		// This is likely way overkill, but it does prevent any external modifications of the errors collection.
		Errors = new ReadOnlyCollection<string>(Errors.Union(new[] {code}).ToList());
	}
}
