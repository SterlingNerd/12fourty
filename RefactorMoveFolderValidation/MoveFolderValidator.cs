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

		IFolderItem destination = GetDestination(targetFolderId);
		bool isDestinationOwner = destination.IsOwner(currentUser);
		bool isDestinationShared = destination.IsShared;

		if (!isDestinationOwner && !isDestinationShared)
		{
			result.AddErrorByCode(FolderItemErrorCode.MoveItemOrFolderToFolderCreatedByAnotherUser);
		}

		if (isAdmin && !isDestinationShared)
		{
			result.AddErrorByCode(FolderItemErrorCode.SharedItemMayOnlyMoveToSharedFolder);
		}

		return result;
	}

	private IFolderItem GetDestination(long? targetFolderId)
	{
		// I would move this logic into the folder service. Essentially making the folder service always return an IFolderItem
		if (targetFolderId.HasValue)
		{
			return customReportFolderService.Get(targetFolderId.Value);
		}
		else
		{
			return RootFolderItem.Instance;
		}
	}
}
