
ls.refresh(true);
var references = ls.getReferencesAtPosition(host.fileName, host.position);
host.updateReferencesAtPosition(references);