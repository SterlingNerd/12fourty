using System;

namespace RefactorMoveFolderValidation;

public interface ICustomReportFolderService
{
	IFolderItem Get(long id);
}
