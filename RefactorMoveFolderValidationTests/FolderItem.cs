using System;

using RefactorMoveFolderValidation;

namespace RefactorMoveFolderValidationTests;

public class FolderItem : IFolderItem
{
	public bool IsShared { get; set; }
	public long OwnerId { get; set; }

	public bool IsOwner(User user)
	{
		return user.Id == OwnerId;
	}
}
