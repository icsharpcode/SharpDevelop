ls.refresh(true);
var diagnostics = ls.getSemanticDiagnostics(host.fileName);
host.updateSemanticDiagnosticsResult(diagnostics);
diagnostics = ls.getSyntacticDiagnostics(host.fileName);
host.updateSyntacticDiagnosticsResult(diagnostics);