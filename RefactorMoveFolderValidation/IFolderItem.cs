using System;

namespace RefactorMoveFolderValidation;

public interface IFolderItem
{
	bool IsShared { get; }
	bool IsOwner(User user);
}
