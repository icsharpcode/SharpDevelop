
ls.refresh(true);
var items = ls.getCompletionsAtPosition(host.fileName, host.position, host.isMemberCompletion);
host.updateCompletionInfoAtCurrentPosition(items);