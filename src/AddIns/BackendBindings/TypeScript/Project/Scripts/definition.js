
ls.refresh(true);
var definition = ls.getDefinitionAtPosition(host.fileName, host.position);
host.updateDefinitionAtPosition(definition);