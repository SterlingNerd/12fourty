namespace RefactorMoveFolderValidation {
    public interface IFolderItem {
        bool IsShared { get; set; }
        bool IsOwner(User user);
    }

    public class User {
        public long Id { get; set; }
        public RoleId RoleId { get; set; }
    }

    public interface ICustomReportFolderService {
        IFolderItem Get(long id);
    }

    public class FolderItemErrorCode {
        public const string MoveItemOrFolderCreatedByAnotherUser = "A";
        public const string MoveItemOrFolderToFolderCreatedByAnotherUser = "B";
        public const string SharedItemMayOnlyMoveToSharedFolder = "C";
    }

    public class Result {
        public bool IsSuccessful { get; set; } = true;

        public void AddErrorByCode(string code) {
            this.IsSuccessful = false;
        }
    }

    public enum RoleId {
        Admin = 1,
        Student = 2,
    }
}
