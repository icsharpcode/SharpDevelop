
module Test
  module Unit
    module UI
      module SharpDevelopConsole
      
        class SharpDevelopSelectedTestsFile
          
          def initialize(filename)
            @tests = []
            read_tests(filename)
          end
          
          def read_tests(filename)
            File.open(filename).each {|line|
              line = line.strip
              if line.length > 0 then
                @tests.push(line)
              end
            }
          end
          
          def tests
            return @tests
          end
          
        end
        
      end
    end
  end
end
