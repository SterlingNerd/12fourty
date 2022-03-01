using System;

namespace RefactorMoveFolderValidation;

public class MoveFolderValidation
{
	private readonly User currentUser;
	private readonly ICustomReportFolderService customReportFolderService;

	public MoveFolderValidation(
		User currentUser,
		ICustomReportFolderService customReportFolderService)
	{
		this.currentUser = currentUser;
		this.customReportFolderService = customReportFolderService;
	}

	/// <summary>
	/// Validates whether the user is permitted to move the item into the target folder.
	/// </summary>
	public Result ValidateMoveFolderItem(IFolderItem item, long? targetFolderId)
	{
		Result result = new();
		bool isOwner = item.IsOwner(currentUser);
		bool isAdminMovingSharedItem = IsAdminMovingSharedItem(item);

		if (!isOwner && !isAdminMovingSharedItem)
		{
			result.AddErrorByCode(FolderItemErrorCode.MoveItemOrFolderCreatedByAnotherUser);
		}

		bool isAdminMovingToSharedFolder;
		if (targetFolderId.HasValue)
		{
			IFolderItem targetFolder = customReportFolderService.Get(targetFolderId.Value);

			isOwner = targetFolder.IsOwner(currentUser);
			isAdminMovingToSharedFolder = IsAdminMovingSharedItem(targetFolder);

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

	private bool IsAdminMovingSharedItem(IFolderItem item)
	{
		bool isOwner = item.IsOwner(currentUser);
		bool isAdminMovingSharedItem = !isOwner && currentUser.RoleId == RoleId.Admin && item.IsShared;
		return isAdminMovingSharedItem;
	}
}
