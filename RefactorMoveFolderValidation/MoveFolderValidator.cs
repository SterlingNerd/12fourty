using System;

namespace RefactorMoveFolderValidation;

public class MoveFolderValidator
{
	private readonly ICustomReportFolderService customReportFolderService;

	public MoveFolderValidator(ICustomReportFolderService customReportFolderService)
	{
		this.customReportFolderService = customReportFolderService;
	}

	/// <summary>
	/// Validates whether the user is permitted to move the item into the target folder.
	/// </summary>
	public Result ValidateMoveFolderItem(User currentUser, IFolderItem item, long? targetFolderId)
	{
		Result result = new();
		bool isSourceOwner = item.IsOwner(currentUser);
		bool isSourceShared = item.IsShared;

		if (!isSourceOwner && !isSourceShared)
		{
			result.AddErrorByCode(FolderItemErrorCode.MoveItemOrFolderCreatedByAnotherUser);
		}

		bool isAdmin = currentUser.RoleId == RoleId.Admin;
		if (!isSourceOwner && !isAdmin)
		{
			result.AddErrorByCode(FolderItemErrorCode.MoveItemOrFolderCreatedByAnotherUser);
		}

		bool isDestinationShared;
		if (targetFolderId.HasValue)
		{
			IFolderItem targetFolder = customReportFolderService.Get(targetFolderId.Value);

			bool isDestinationOwner = targetFolder.IsOwner(currentUser);
			isDestinationShared = targetFolder.IsShared;

			if (!isDestinationOwner && !isDestinationShared)
			{
				result.AddErrorByCode(FolderItemErrorCode.MoveItemOrFolderToFolderCreatedByAnotherUser);
			}
		}
		else
		{
			// root folder is by definition, shared.
			isDestinationShared = true;
		}

		if (isAdmin && !isDestinationShared)
		{
			result.AddErrorByCode(FolderItemErrorCode.SharedItemMayOnlyMoveToSharedFolder);
		}

		return result;
	}
}
