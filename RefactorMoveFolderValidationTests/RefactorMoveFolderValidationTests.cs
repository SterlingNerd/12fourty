using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RefactorMoveFolderValidation;

namespace RefactorMoveFolderValidationTests {
    [TestClass]
    public class RefactorMoveFolderValidationTests {
        private Mock<ICustomReportFolderService> mockICustomReportFolderService;
        private MoveFolderValidation moveFolderValidation;

        [TestInitialize]
        public void Init() {
            this.mockICustomReportFolderService = new Mock<ICustomReportFolderService>();
            this.moveFolderValidation = new MoveFolderValidation(new User { Id = 1 }, this.mockICustomReportFolderService.Object);
        }

        [TestMethod]
        public void ValidateMoveFolderItem_OwnerMovingToOwnPrivateFolder_Success() {
            var folderItem = new FolderItem {
                OwnerId = 1,
            };

            this.mockICustomReportFolderService.Setup(x => x.Get(It.IsAny<long>())).Returns(new FolderItem {
                OwnerId = 1,
            });

            Assert.IsTrue(this.moveFolderValidation.ValidateMoveFolderItem(folderItem, 10).IsSuccessful);
        }
    }

    public class FolderItem : IFolderItem {
        public long OwnerId { get; set; }
        public bool IsShared { get; set; }
        public bool IsOwner(User user) {
            return user.Id == this.OwnerId;
        }
    }

}
