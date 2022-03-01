using System;

namespace RefactorMoveFolderValidation;

public class Result
{
	public bool IsSuccessful { get; set; } = true;

	public void AddErrorByCode(string code)
	{
		IsSuccessful = false;
	}
}
