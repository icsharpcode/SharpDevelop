ls.refresh(true);
var emitResult = ls.getEmitOutput(host.fileName);
host.updateCompilerResult(emitResult);