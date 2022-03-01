using System;

namespace RefactorMoveFolderValidation;

public interface IFolderItem
{
	bool IsShared { get; set; }
	bool IsOwner(User user);
}
