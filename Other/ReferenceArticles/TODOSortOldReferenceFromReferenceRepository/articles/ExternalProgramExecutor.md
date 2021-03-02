# ExternalProgramExecutor

Sometimes you want to call a program. `ExternalProgramExecutor` is a wrapper to do exactly this. You can simply call `new System.Diagnostics.Process(...)` but there are some "features" missing. For example: Sometimes you want to execute a program and log its output, need a timeout, expect the exitcode to be 0 etc.
To code this you probably do not need many lines of code. But it is always the same and it is an annoying work. Why can't you simply have an alternative to `System.Diagnostics.Process` which does have all of this features out of the box? `ExternalProgramExecutor` does this have.
It is very easy to call: Basically it it `ExternalProgramExecutor.Create("MyProgram").StartConsoleApplicationInCurrentConsoleWindow()`. Optionally you can pass more arguments like the arguments for the executed program, the working-directory, a timeout, a logfile for the output and so on. You can also specify that an exception should be thrown if the exitcode of the executed program is not 0. When the execution is finished you can query the `exitcode`, `StdOut` , `StdErr` and `ExecutionDuration`. You also do not have to specify the full-path of Path-Variables. For example: You can do `ExternalProgramExecutor.Create("git", "<argument>")` when `git` is in your path-environement-variable. Something like `ExternalProgramExecutor.Create(@"C:\Program Files\Git\bin\git.exe", "<argument>")` is not required. When you call non-executables like `ExternalProgramExecutor.Create(@"C:\temp\test.txt")` then the file will be opened with the default-program of the operating-system for this file-extension (here: The default-program to open `.txt`-files).