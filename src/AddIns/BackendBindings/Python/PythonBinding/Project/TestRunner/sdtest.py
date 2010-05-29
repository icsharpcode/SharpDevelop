
import sys
import System.IO

class SharpDevelopTestProgram:
    
    def __init__(self):
        self._sysPaths = []
        self._testNames = []

    def run(self):
        if self._validateCommandLineArgs():
            self._runTests()
            return
        
        print 'Usage: sdunittest.py test-names-file sys-paths-file test-results-file'
        print 'Usage: sdunittest.py @response-file'
        print ''
        print 'Example response file content: '
        print  '/p:"sys/path/1"'
        print  '/p:"sys/path/2"'
        print  '/r:"path/to/results-file"'
        print  'test-name1'
        print  'test-name2'
        print  'test-name3'
    
    def _validateCommandLineArgs(self):
        if len(sys.argv) == 4:
            self._testNamesFile = sys.argv[1]
            self._sysPathFile = sys.argv[2]
            self._testResultsFile = sys.argv[3]
            self._responseFile = ''
            return True
        if len(sys.argv) == 2:
            return self._getResponseFileName(sys.argv[1])
        return False
    
    def _getResponseFileName(self, fileName):
        if len(fileName) > 0:
            if fileName[0] == '@':
                self._responseFile = fileName[1:]
                return True
        return False
    
    def _runTests(self):        
        if len(self._responseFile) > 0:
            self._readResponseFile()
        else:
            self._readSysPathsFromFile()
            self._readTestNames()
        
        self._addSysPaths()

        import unittest
        import sdtestrunner
        
        suite = unittest.TestLoader().loadTestsFromNames(self._testNames)
        sdtestrunner.SharpDevelopTestRunner(resultsFileName=self._testResultsFile, verbosity=2).run(suite)       

    def _readResponseFile(self):
        for line in self._readLinesFromFile(self._responseFile):
            self._readResponseFileArgument(line)
    
    def _readResponseFileArgument(self, line):
        if line.startswith('/r:'):
            line = self._removeQuotes(line[3:])
            self._testResultsFile = line            
        elif line.startswith('/p:'):
            line = self._removeQuotes(line[3:])
            self._sysPaths.append(line)
        else:
            self._testNames.append(line)

    def _removeQuotes(self, line):
        return line.strip('\"')
        
    def _readLinesFromFile(self, fileName):
        #f = codecs.open(fileName, 'rb', 'utf-8')
        #return f.readall().splitlines()
        return System.IO.File.ReadAllLines(fileName)

    def _readTestNames(self):
        self._testNames = self._readLinesFromFile(self._testNamesFile)

    def _readSysPathsFromFile(self):
        self._sysPaths = self._readLinesFromFile(self._sysPathFile)
    
    def _addSysPaths(self):
        for path in self._sysPaths:
            sys.path.append(path)


if __name__ == '__main__':	
    program = SharpDevelopTestProgram()
    program.run()
