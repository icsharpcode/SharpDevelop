
require 'sdtestresult'

module Test
  module Unit
    module UI
      module SharpDevelopConsole

        class SharpDevelopTestResultWriter
        
          def initialize(filename)
            @file = File.open(filename, 'w')
          end
          
          def write_test_result(test_result)
            writeline("Name: " + test_result.name)
            writeline("Result: " + test_result.result)
            writeline("Message: " + test_result.message)
            writeline("StackTrace: " + test_result.stacktrace)
          end
          
          def writeline(message)
            @file.write(message + "\r\n")
            @file.flush
          end
        end
      
      end
    end
  end
end
