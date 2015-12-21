
ls.refresh(true);
var items = ls.getCompletionEntryDetails(host.fileName, host.position, host.completionEntry);
host.updateCompletionEntryDetailsAtCurrentPosition(items);