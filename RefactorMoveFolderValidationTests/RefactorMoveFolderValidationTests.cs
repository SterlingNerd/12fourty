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
		_moveFolderValidator = new MoveFolderValidator(mockICustomReportFolderService.Object);
	}

	#endregion

	private Mock<ICustomReportFolderService> mockICustomReportFolderService;
	private MoveFolderValidator _moveFolderValidator;

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
		User user = new() {Id = 42, RoleId = isAdmin ? RoleId.Admin : RoleId.Student};

		_moveFolderValidator = new MoveFolderValidator(mockICustomReportFolderService.Object);

		mockICustomReportFolderService.Setup(x => x.Get(It.IsAny<long>())).Returns(new FakeFolderItem {OwnerId = 23, IsShared = destinationShared});

		_moveFolderValidator.ValidateMoveFolderItem(user, folderItem, 10)
			.IsSuccessful
			.Should()
			.Be(expected);
	}

	// * Root folder (i.e no targetFolderId) is always considered to be shared
	[TestMethod]
	public void ValidateMoveFolderItem_MoveToRootFolder_Success()
	{
		FakeFolderItem folderItem = new() {OwnerId = 1, IsShared = true};
		User user = new() {Id = 1, RoleId = RoleId.Student};

		_moveFolderValidator.ValidateMoveFolderItem(user, folderItem, null)
			.IsSuccessful
			.Should()
			.Be(true);
	}

	// * Shared item may only move into a shared folder
	[TestMethod]
	public void ValidateMoveFolderItem_OwnerMovingPrivateFolder_Invalid()
	{
		FakeFolderItem folderItem = new() {OwnerId = 1, IsShared = true};
		User user = new() {Id = 1, RoleId = RoleId.Student};

		mockICustomReportFolderService.Setup(x => x.Get(It.IsAny<long>())).Returns(new FakeFolderItem {OwnerId = 23, IsShared = false});

		_moveFolderValidator.ValidateMoveFolderItem(user, folderItem, 10)
			.IsSuccessful
			.Should()
			.Be(false);
	}

	// Any user may move their own folder item, but if moving to a non-shared folder, the user must own that folder
	[TestMethod]
	public void ValidateMoveFolderItem_OwnerMovingToOwnPrivateFolder_Success()
	{
		FakeFolderItem folderItem = new() {OwnerId = 1};
		User user = new() {Id = 1, RoleId = RoleId.Student};
		mockICustomReportFolderService.Setup(x => x.Get(It.IsAny<long>())).Returns(new FakeFolderItem {OwnerId = 1, IsShared = false});

		_moveFolderValidator.ValidateMoveFolderItem(user, folderItem, 10)
			.IsSuccessful
			.Should()
			.Be(true);
	}

	// Any user may move their own folder item
	[TestMethod]
	public void ValidateMoveFolderItem_OwnerMovingToOwnSharedFolder_Success()
	{
		FakeFolderItem folderItem = new() {OwnerId = 1, IsShared = false};
		User user = new() {Id = 1, RoleId = RoleId.Student};

		mockICustomReportFolderService.Setup(x => x.Get(It.IsAny<long>())).Returns(new FakeFolderItem {OwnerId = 1, IsShared = false});

		_moveFolderValidator.ValidateMoveFolderItem(user, folderItem, 10)
			.IsSuccessful
			.Should()
			.Be(true);
	}
}
