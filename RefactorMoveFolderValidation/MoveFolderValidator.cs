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
		bool isOwner = item.IsOwner(currentUser);
		bool isAdminMovingSharedItem = IsAdminMovingSharedItem(currentUser, item);

		if (!isOwner && !isAdminMovingSharedItem)
		{
			result.AddErrorByCode(FolderItemErrorCode.MoveItemOrFolderCreatedByAnotherUser);
		}

		bool isAdminMovingToSharedFolder;
		if (targetFolderId.HasValue)
		{
			IFolderItem targetFolder = customReportFolderService.Get(targetFolderId.Value);

			isOwner = targetFolder.IsOwner(currentUser);
			isAdminMovingToSharedFolder = IsAdminMovingSharedItem(currentUser, targetFolder);

			if (!isOwner && !isAdminMovingToSharedFolder)
			{
				result.AddErrorByCode(FolderItemErrorCode.MoveItemOrFolderToFolderCreatedByAnotherUser);
			}
		}
		else
		{
			// root folder
			isAdminMovingToSharedFolder = !isOwner && currentUser.RoleId == RoleId.Admin;
		}

		if (isAdminMovingSharedItem && !isAdminMovingToSharedFolder)
		{
			result.AddErrorByCode(FolderItemErrorCode.SharedItemMayOnlyMoveToSharedFolder);
		}

		return result;
	}

	private static bool IsAdminMovingSharedItem(User currentUser, IFolderItem item)
	{
		bool isAdminMovingSharedItem =  currentUser.RoleId == RoleId.Admin && item.IsShared;
		return isAdminMovingSharedItem;
	}
}
