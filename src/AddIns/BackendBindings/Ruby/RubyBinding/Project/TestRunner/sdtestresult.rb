
module Test
  module Unit
    module UI
      module SharpDevelopConsole

        class SharpDevelopTestResult
        
          def initialize(name)
            @name = parse_name(name)
            @result = 'Success'
            @message = ''
            @stacktrace = ''
          end
          
          def parse_name(name)
            @name = name
            open_bracket_index = name.index('(')
            if open_bracket_index > 0 then
              close_bracket_index = name.index(')', open_bracket_index)
              if close_bracket_index > 0 then
                length = close_bracket_index - open_bracket_index - 1
                method_name = name[0, open_bracket_index]
                class_name = name[open_bracket_index + 1, length]
                @name = class_name + '.' + method_name
              end
            end
          end
          
          def name
             return @name
          end
          
          def result
             return @result
          end
          
          def message
            return @message
          end
          
          def stacktrace
            return @stacktrace
          end
          
          def update_fault(fault)
            @result = 'Failure'
            @message = format_text(fault.message)
            @stacktrace = format_text(fault.long_display)
          end
          
          def format_text(text)
            formatted_text = ''
            text.each_line do |line|
              formatted_text += " " + line
            end
            return formatted_text[1..-1]
          end
        end
      
      end
    end
  end
end
