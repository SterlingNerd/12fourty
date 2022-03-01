# Instructions
* The `MoveFolderValidation` class is part of an app that provides folder structure functionality similar to windows explorer or google drive. 
* A bug was identified that prevents an admin from moving another user's shared item into a shared folder. 
* Please not only identify and address the bug, but also refactor the code so that it is easier to read. 
* Feel free to completely rewrite!

# Business Rules
* Shared item may only move into a shared folder
* Only an admin can move another user's folder item, but only if both the folder item and the target folder are shared
* Any user may move their own folder item, but if moving to a non-shared folder, the user must own that folder
* Root folder (i.e no targetFolderId) is always considered to be shared