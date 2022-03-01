using System;

namespace RefactorMoveFolderValidation;

public class FolderItemErrorCode
{
	public const string MoveItemOrFolderCreatedByAnotherUser = "A";
	public const string MoveItemOrFolderToFolderCreatedByAnotherUser = "B";
	public const string SharedItemMayOnlyMoveToSharedFolder = "C";
}
