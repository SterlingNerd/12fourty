using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using RefactorMoveFolderValidation;

namespace RefactorMoveFolderValidationTests;

[TestClass]
public class RefactorMoveFolderValidationTests
{
	#region Setup/Teardown

	[TestInitialize]
	public void Init()
	{
		mockICustomReportFolderService = new Mock<ICustomReportFolderService>();
		moveFolderValidation = new MoveFolderValidation(new User {Id = 1}, mockICustomReportFolderService.Object);
	}

	#endregion

	private Mock<ICustomReportFolderService> mockICustomReportFolderService;
	private MoveFolderValidation moveFolderValidation;

	[TestMethod]
	public void ValidateMoveFolderItem_OwnerMovingToOwnPrivateFolder_Success()
	{
		FolderItem folderItem = new FolderItem {OwnerId = 1};

		mockICustomReportFolderService.Setup(x => x.Get(It.IsAny<long>())).Returns(new FolderItem {OwnerId = 1});

		Assert.IsTrue(moveFolderValidation.ValidateMoveFolderItem(folderItem, 10).IsSuccessful);
	}
}
