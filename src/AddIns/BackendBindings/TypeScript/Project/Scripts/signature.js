
ls.refresh(true);
var signature = ls.getSignatureHelpItems(host.fileName, host.position);
host.updateSignatureAtPosition(signature);