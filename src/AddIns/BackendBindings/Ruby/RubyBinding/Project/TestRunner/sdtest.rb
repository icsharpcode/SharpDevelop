
require 'test/unit'
require 'test/unit/autorunner'
require 'sdselectedtestsfile'

Test::Unit.run = false

Test::Unit::AutoRunner::RUNNERS[:console] = 
	proc do |r|
		require 'sdtestrunner'
		Test::Unit::UI::SharpDevelopConsole::TestRunner
	end

standalone = true
runner = Test::Unit::AutoRunner.new(standalone)
runner.process_args(ARGV)

# Add files to run tests on.
selected_tests_filename = ARGV.last
print "selected_tests_filename: " + selected_tests_filename
selected_tests_file = Test::Unit::UI::SharpDevelopConsole::SharpDevelopSelectedTestsFile.new(selected_tests_filename)
selected_tests_file.tests.each {|filename|
  runner.to_run.push(filename)
}

runner.run

