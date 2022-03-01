﻿namespace RefactorMoveFolderValidation {
    public class MoveFolderValidation {
        private readonly User currentUser;
        private readonly ICustomReportFolderService customReportFolderService;

        public MoveFolderValidation(
            User currentUser,
            ICustomReportFolderService customReportFolderService) {
            this.currentUser = currentUser;
            this.customReportFolderService = customReportFolderService;
        }

        /// <summary>
        /// Validates whether the user is permitted to move the item into the target folder.
        /// </summary>
        public Result ValidateMoveFolderItem(IFolderItem item, long? targetFolderId) {
            var result = new Result();
            var isOwner = item.IsOwner(this.currentUser);
            var isAdminMovingSharedItem = this.IsAdminMovingSharedItem(item);

            if (!isOwner && !isAdminMovingSharedItem) {
                result.AddErrorByCode(FolderItemErrorCode.MoveItemOrFolderCreatedByAnotherUser);
            }

            bool isAdminMovingToSharedFolder;
            if (targetFolderId.HasValue) {
                var targetFolder = this.customReportFolderService.Get(targetFolderId.Value);

                isOwner = targetFolder.IsOwner(this.currentUser);
                isAdminMovingToSharedFolder = this.IsAdminMovingSharedItem(targetFolder);

                if (!isOwner && !isAdminMovingToSharedFolder) {
                    result.AddErrorByCode(FolderItemErrorCode.MoveItemOrFolderToFolderCreatedByAnotherUser);
                }
            } else {
                // root folder
                isAdminMovingToSharedFolder = !isOwner && this.currentUser.RoleId == RoleId.Admin;
            }

            if (isAdminMovingSharedItem && !isAdminMovingToSharedFolder) {
                result.AddErrorByCode(FolderItemErrorCode.SharedItemMayOnlyMoveToSharedFolder);
            }

            return result;
        }

        private bool IsAdminMovingSharedItem(IFolderItem item) {
            var isOwner = item.IsOwner(this.currentUser);
            var isAdminMovingSharedItem = !isOwner && this.currentUser.RoleId == RoleId.Admin && item.IsShared;
            return isAdminMovingSharedItem;
        }
    }
}