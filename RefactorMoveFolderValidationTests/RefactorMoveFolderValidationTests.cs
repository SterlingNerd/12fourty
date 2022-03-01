using System;

using FluentAssertions;

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
		moveFolderValidation = new MoveFolderValidation(new User {Id = 1, RoleId = RoleId.Student}, mockICustomReportFolderService.Object);
	}

	#endregion

	private Mock<ICustomReportFolderService> mockICustomReportFolderService;
	private MoveFolderValidation moveFolderValidation;

	// Any user may move their own folder item, but if moving to a non-shared folder, the user must own that folder
	[TestMethod]
	public void ValidateMoveFolderItem_OwnerMovingToOwnPrivateFolder_Success()
	{
		FakeFolderItem folderItem = new() {OwnerId = 1};

		mockICustomReportFolderService.Setup(x => x.Get(It.IsAny<long>())).Returns(new FakeFolderItem {OwnerId = 1, IsShared = false});

		moveFolderValidation.ValidateMoveFolderItem(folderItem, 10)
			.IsSuccessful
			.Should()
			.Be(true);
	}

	// * Shared item may only move into a shared folder
	[TestMethod]
	public void ValidateMoveFolderItem_OwnerMovingPrivateFolder_Invalid()
	{
		FakeFolderItem folderItem = new() {OwnerId = 1, IsShared = true};

		mockICustomReportFolderService.Setup(x => x.Get(It.IsAny<long>())).Returns(new FakeFolderItem {OwnerId = 23, IsShared = false});

		moveFolderValidation.ValidateMoveFolderItem(folderItem, 10)
			.IsSuccessful
			.Should()
			.Be(false);
	}

	// * Only an admin can move another user's folder item, but only if both the folder item and the target folder are shared
	[DataTestMethod]
	[DataRow(false, false, false, false)]
	[DataRow(false, false, true, false)]
	[DataRow(false, true, false, false)]
	[DataRow(false, true, true, false)]
	[DataRow(true, false, false, false)]
	[DataRow(true, false, true, false)]
	[DataRow(true, true, false, false)]
	[DataRow(true, true, true, true)]
	public void ValidateMoveFolderItem_AdminMovingPrivateItem_Invalid(bool isAdmin, bool sourceShared, bool destinationShared, bool expected)
	{
		FakeFolderItem folderItem = new() {OwnerId = 1, IsShared = sourceShared};
		moveFolderValidation = new MoveFolderValidation(new User {Id = 42, RoleId = isAdmin ? RoleId.Admin : RoleId.Student}, mockICustomReportFolderService.Object);

		mockICustomReportFolderService.Setup(x => x.Get(It.IsAny<long>())).Returns(new FakeFolderItem {OwnerId = 23, IsShared = destinationShared});

		moveFolderValidation.ValidateMoveFolderItem(folderItem, 10)
			.IsSuccessful
			.Should()
			.Be(expected);
	}

	// * Root folder (i.e no targetFolderId) is always considered to be shared
	[TestMethod]
	public void ValidateMoveFolderItem_MoveToRootFolder_Success()
	{
		FakeFolderItem folderItem = new() {OwnerId = 1, IsShared = true};

		//mockICustomReportFolderService.Setup(x => x.Get(0)).Returns(new FakeFolderItem {OwnerId = 23, IsShared = false});

		moveFolderValidation.ValidateMoveFolderItem(folderItem, null)
			.IsSuccessful
			.Should()
			.Be(true);
	}

	// Any user may move their own folder item
	[TestMethod]
	public void ValidateMoveFolderItem_OwnerMovingToOwnSharedFolder_Success()
	{
		FakeFolderItem folderItem = new() {OwnerId = 1, IsShared = false};

		mockICustomReportFolderService.Setup(x => x.Get(It.IsAny<long>())).Returns(new FakeFolderItem {OwnerId = 1, IsShared = false});

		moveFolderValidation.ValidateMoveFolderItem(folderItem, 10)
			.IsSuccessful
			.Should()
			.Be(true);
	}
}
