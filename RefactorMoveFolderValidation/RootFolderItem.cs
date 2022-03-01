using System;

namespace RefactorMoveFolderValidation;

public class RootFolderItem : IFolderItem
{
	private static readonly Lazy<RootFolderItem> _lazy = new(new RootFolderItem());
	public static RootFolderItem Instance => _lazy.Value;
	public bool IsShared => true;

	private RootFolderItem()
	{
	}

	public bool IsOwner(User user)
	{
		return false;
	}
}
