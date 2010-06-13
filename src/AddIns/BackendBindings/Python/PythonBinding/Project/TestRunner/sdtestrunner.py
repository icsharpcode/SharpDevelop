
import codecs
import sys
import time
from unittest import TestResult
from unittest import TextTestRunner
from unittest import _TextTestResult
from unittest import _WritelnDecorator

class _SharpDevelopTestResultWriter:
    def __init__(self, resultsFileName):
        self.stream = codecs.open(resultsFileName, "w+", "utf-8-sig")
 
    def _writeln(self, arg):
        self.stream.write(arg)
        self.stream.write('\r\n')
        
    def _writeTestName(self, test):
        self._writeln("Name: " + test.id())
        
    def _writeTestResult(self, result):
        self._writeln("Result: " + result)
        
    def _writeTestSuccess(self):
        self._writeTestResult("Success")
    
    def _writeTestFailure(self, test, err, testResult):
        self._writeTestResult("Failure")
        
        exctype, value, tb = err
        if value != None:
            message = self._prefixLinesWithSpaceChar(str(value))
            self._writeln("Message: " + message)
            
        excInfoString = testResult._exc_info_to_string(err, test)
        excInfoString = self._prefixLinesWithSpaceChar(excInfoString)
        self._writeln("StackTrace: " + excInfoString)
    
    def _prefixLinesWithSpaceChar(self, text):
        lines = []
        originalLines = text.splitlines()
        if len(originalLines) == 0:
            return text
        
        lines.append(originalLines[0] + '\r\n')
        
        for line in originalLines[1:]:
            lines.append(' ' + line + '\r\n')       
        return ''.join(lines).rstrip()

    def addSuccess(self, test):
        self._writeTestName(test)
        self._writeTestSuccess()

    def addError(self, test, err, testResult):
        self._writeTestName(test)
        self._writeTestFailure(test, err, testResult)
        
    def addFailure(self, test, err, testResult):
        self._writeTestName(test)
        self._writeTestFailure(test, err, testResult)
        
class _SharpDevelopNullTestResultWriter:
    def __init__(self):
        pass
        
    def addSuccess(self, test):
        pass 

    def addError(self, test, err, testResult):
        pass
        
    def addFailure(self, test, err, testResult):
        pass       


class _SharpDevelopTestResult(_TextTestResult):
    def __init__(self, stream, descriptions, verbosity, resultWriter):
        _TextTestResult.__init__(self, stream, descriptions, verbosity)
        self.resultWriter = resultWriter

    def addSuccess(self, test):
        self.resultWriter.addSuccess(test)
        _TextTestResult.addSuccess(self, test)

    def addError(self, test, err):
        self.resultWriter.addError(test, err, self)
        _TextTestResult.addError(self, test, err)

    def addFailure(self, test, err):
        self.resultWriter.addFailure(test, err, self)
        _TextTestResult.addFailure(self, test, err)


class SharpDevelopTestRunner(TextTestRunner):
    def __init__(self, stream=sys.stderr, resultsFileName=None, descriptions=1, verbosity=1):
        self.stream = _WritelnDecorator(stream)
        self.descriptions = descriptions
        self.verbosity = verbosity
        if resultsFileName is None:
            self.resultWriter = _SharpDevelopNullTestResultWriter()
        else:
            self.resultWriter = _SharpDevelopTestResultWriter(resultsFileName)
        
    def _makeResult(self):
        return _SharpDevelopTestResult(self.stream, self.descriptions, self.verbosity, self.resultWriter)
