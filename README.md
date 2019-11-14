> Debugging is like being the detective in a crime movie where you are also the murderer.

# Troubleshooting Tools (Windows, .NET & native, no container)
Legend:
- 📊 monitoring and data collection (dumps)
- ⚠️ inspects logs & events
- 📝 inspects memory usage (RAM)
- 📉 inspects CPU usage
- 🔀 inspects threads
- ➿ inspects dependencies, static & dynamic reverse eng
- 📁 inspects IO access (disk, shares)
- 🔌 inspects network usage
- 📜 inspects database

You do not need to be an expert in any of the tools listed below. Simply learn that 20% that will let you solve 80% of the cases (aka *pareto principle* or *80/20 rule*).

## Event Viewer (`eventvwr`) ⚠️
[article](https://www.codeproject.com/articles/597856/using-windows-event-viewer-to-debug-crashes) \
*When:* One of the first things to check when app or service crash. When app log shows an error and we need more info. When we need to view windows logs remotely. \
*Usage:*
1. run problematic app and `eventvwr`
1. *(optional)* rclick *Event Viewer* > *Connect to Another Computer...*
1. select *Windows Logs* > *Application* > look for *Error* or *Warning*
1. note *Faulting application path*, *Faulting module name and path*, *Exception code* and *Faulting offset*
1. debug the app and look at the offset, it points to exact crash location in the faulting module
1. use *Disassembly* and jump to *starting memory address of the loaded faulting module* + *faulting offset*

*Keywords:* crash, windows events, windows logs, remote event viewer \
*Similar to:* n/a \
*Synergy with:* n/a

## Debug View (`dbgview`) ⚠️
[page](https://docs.microsoft.com/en-us/sysinternals/downloads/debugview) \
*When:* You want to quickly see debug output from an (remote) app that does `Trace.Write*()` or `Debug.Write*()`. \
*Keywords:* debug console debugger \
*Similar to:* n/a \
*Synergy with:* n/a

## Performance Monitor (`perfmon`) 📊
[article](http://www.codeproject.com/Articles/42721/Best-Practices-No-Detecting-NET-application-memo) \
*When:* We need to monitor consumption of *private bytes* to detect memory leaks. \
*Usage:*
1. start your application which has memory leaks and keep it running
1. run `perfmon` and delete all the current performance counters
1. *(optional)* rclick *Performance* > *Connect to Another Computer...*
1. rclick > *Add counters* > *Process* from the performance object and select *Private bytes* from the counter list
1. rclick > *Add counters* > *.NET CLR memory* from the performance object and select *Bytes in all heaps* from the counter list
1. select app from the instance list.
   - *Private bytes* is the total memory consumed by the application
   - *Bytes in all heaps* is the memory consumed by the managed code
   - You need to substract those two to find how much of unmanaged memory is consumed: `Unmanaged memory + Bytes in all helps = Private bytes`

*Keywords:* memory leak, performance counters \
*Similar to:* Task Manager, Resource Monitor \
*Synergy with:* [debugdiag](http://www.microsoft.com/en-us/download/details.aspx?id=40336 )

## ProcDump (`procdump`) 📊
[page](https://docs.microsoft.com/en-us/sysinternals/downloads/procdump) \
*When:* We need to generate full memory (crash) dumps of a process or monitoring for: CPU spikes, hung window, unhandled exception, specific values of system performance counters.
- *full dump:* `procdump -ma <pid>`
- *dump memo when NullReferenceException happens:* `procdump -ma -e 1 -n 3 -f "*Null*" <pid>`

*Keywords:* memory leak, crash dump, full memory dump \
*Similar to:* process explorer > dump, `dotnet-dump` (multiplatform) \
*Synergy with:* most of the tools that can load `.dmp` file format, i.e: WinDbg+SOS, PerfView, 

## Assembly Binding Log Viewer (Fusion, Fuslogvw) 📊
[page](https://docs.microsoft.com/en-us/dotnet/framework/tools/fuslogvw-exe-assembly-binding-log-viewer?redirectedfrom=MSDN)
| [so](https://stackoverflow.com/a/1527249/2871223) \
*When:* We want to diagnose why the .NET Framework cannot locate an assembly at run time, diagnose failed assembly bind \
*Usage:*
1. Add reg key values
   ```
   reg add HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Fusion /v ForceLog /t REG_DWORD /d 1
   reg add HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Fusion /v LogFailures /t REG_DWORD /d 1
   reg add HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Fusion /v LogResourceBinds /t REG_DWORD /d 1
   ```
   - *optional* `reg add HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Fusion /v LogPath /t REG_SZ /d C:\FusionLog\`
1. Run the app that has problems loading assembly or one of its dependencies
1. Navigate to `C:\FusionLog\` or run `dcmdp> fuslogvw` and view logs

*Keywords:* crash, assembly binding, dependencies, iis \
*Similar to:* dependency walker, ILSpy \
*Synergy with:* EventViewer (tells us we failed loading)

## Windows Error Reporting (WER) auto-collect dump 📊
[page](https://docs.microsoft.com/en-us/windows/win32/wer/collecting-user-mode-dumps) 
| [article](https://blogs.msdn.microsoft.com/chaun/2013/11/12/steps-to-catch-a-simple-crash-dump-of-a-crashing-process/) \
*When:* We want to collect full user-mode dumps after an app crashes withou 3rd party tools. When DebugDiag cannot be installed cause of IT. \
*Usage:*
1. Add reg key values (replace `<yourapp>`)
   ```
   reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\Windows Error Reporting\LocalDumps\<yourapp>.exe" /v DumpFolder /t REG_SZ /d "c:\dumps"
   reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\Windows Error Reporting\LocalDumps\<yourapp>.exe" /v DumpCount /t REG_DWORD /d 2
   reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\Windows Error Reporting\LocalDumps\<yourapp>.exe" /v DumpType /t REG_DWORD /d 2
   ```
1. Run the app that may crash or freeze
1. This will make a total of 2 full user dumps (not mini-dumps but full dumps) and write the dumps to `C:\dumps`.

*Keywords:* crash dump, happy IT \
*Similar to:* DebugDiag (also automatically trigger a crash dump, but DD is better + can analyze) \
*Synergy with:* most of the tools that can load `.dmp` file format

## Debug Diagnostics Tool (`debugdiag`) 📊
[page](https://support.microsoft.com/en-us/help/919792/how-to-use-the-debug-diagnostics-tool-to-troubleshoot-a-process-that-h) 
| [download](https://www.microsoft.com/en-us/download/details.aspx?id=58210)
| [article](https://blogs.msdn.microsoft.com/debugdiag/2013/03/15/debug-diag-blog-post-series-from-my-french-iis-colleagues/)
| [article: native memo leaks](https://blogs.msdn.microsoft.com/tess/2010/01/14/debugging-native-memory-leaks-with-debug-diag-1-1/)
| [article: performance](https://support.microsoft.com/en-us/help/919791/how-to-use-the-debug-diagnostics-tool-to-troubleshoot-high-cpu-usage-b) \
*When:* We need to catch and analyze memory dump for: hangs, slow performance, memory leaks or memory fragmentation, and crashes. When we need to track allocations and deallocations in native code to find leaks. When we want to diagnose memo leaks without touching the code (via dll injection). \
*Usage:*
1. run `debugdiag` as admin
1. create a leak rule against the process in question
1. *Tools* > *Options & Setings* > *Preferences* > check *Record call stacks immediatelly...*
1. create leak rule  that outputs full memory dump when certain threshold of *priv byte* usage is met

*Keywords:* crash dump, full memory dump, memory leak, trigger, performance analysis \
*Similar to:* procdump, WER auto-collect, most of the tools that can load `.dmp` file format \
*Synergy with:* most of the tools that can load `.dmp` file format

## Dependency Walker (`depends`) ➿
[page](http://www.dependencywalker.com/) \
*When:* We need to diagnose which native dependencies are missing. When diagnosing if arch/version/exports of loaded module match expectations.  \
*Usage:*
- dlls built with `/MD` flag (*Multi-threaded DLL*) will load compiler/toolset specific module, like: `MSVCP14D.DLL`
- `PI`/`E` views show what is imported/exported from each module
  - *View* > *Undecorate C++ Functions* to unmangle exported function names

*Keywords:* crash, native, c++, platform toolset, /MD /MDd /MT /MTd run-time library, statically linked to MSVCRT.lib, static version of the run-time library
*Similar to:* **[Dependencies](https://github.com/lucasg/Dependencies)** (modern) \
*Synergy with:* n/a

## Process Monitor (`procmon`) ➿📁🔌
[page](https://docs.microsoft.com/en-us/sysinternals/downloads/procmon) 
| [article](https://www.codeproject.com/Articles/560816/Troubleshooting-dependency-resolution-problems-usi)  \
*When:* We need to diagnose which dependencies are missing and why (user not authorized). When we need to inspect I/O read/write pattern. When we want to see if windows cache manager fails at predictions. When we want to find which process accessed/locked file, when and why (thread stack trace). When we need I/O access summary for performance metrics. \
*Usage:*
- *Process Tree* will show captured processes that closed
- you can highlight specific events with respect to type/value
- *File Summary...* provides good I/O metrics
- *Stack Summary...* may show some of the bottlenecks in the app
- *Duration* column will help evaluating how long did the event take
- TODO: diagnosing failed dll lookups, cache manager etc

*Keywords:* crash, performance, IO read write, registry, network shares, cache manager \
*Similar to:* n/a \
*Synergy with:* n/a

## PerfView (`perfview`) 📊📝📉➿
[page](https://github.com/microsoft/perfview)
| [doc](http://htmlpreview.github.io/?https://github.com/Microsoft/perfview/blob/master/src/PerfView/SupportFiles/UsersGuide.htm)
| [download](https://github.com/microsoft/perfview/releases)
| [video tutorial](https://channel9.msdn.com/Series/PerfView-Tutorial) \
*When:* profiling, analyze stack traces, memory allocations, CPU (cache miss, branch misprediction), thread time, dump diffs.  \
*Usage:*
- analyze memory issues like memory leak: 
  1. *Memory* > *Take Heap Snapshot from Dump*, enter *1000000* in the *Max Dump Obj K Objs* > *Dump GC Heap* > `baseline.gcdump`
  1. do the same .gcdump file generation process but with the `.dmp` that contains unexpected memory
  1. *Stacks* window > *Diff* > *With Baseline: baseline.gcdump*
  1. empty *Fold%:* and *FoldPats:* fields, filter by *IncPats:*`Memory.`
  1. compare snapshots via *Diff Stacks*, sort by *Exc Ct* column and find obj types that should not be in cheap
  1. dclick > *Refered-From* and find references root, that is the CLR object that still holds ref (may need to clear *GroupPats:* field for better view)
- analyze cpu perf:
  1. *Collect* > *Run* > *Command* to set timeframe where we collect ETW events
  1. *CPU Stacks* > *Metric/msec* should be close to 1.0 for CPU investigations
  1. *By Name* tab shows methods where we did spent most of the time (*bottleneck*, *hotpath*)
  1. *Call Tree* tab shows stack trace, just expand calls that take most % time on each level
     - rclick > *Goto* > *Callers* to filter out callers only
     - rclick > *Goto Source* to investigate
- u can narrow CPU peak via histogram: select range like `__A1_2__` > rclick >  *Set Range Filter*
- u can view source via rclick > *Go To Source*
- investigate loaded modules (dlls)
  1. *Collect* > *Run*
  1. *Filter:* `proc|image`, *Text filter:* `clr`, *Process filter* <target>
  1. select *Time Msec* cell and rclick > *Open Any Stacks* and ivestigate
- investigate threads (deadlocks):
  1. *Collect* > *Run* and check *Thread Time*
  1. suspicious threads are the ones that have low CPU times (discard *.NET Finalizer Thread* from investigation), notice *CPU_TIME* and *BLOCKED_TIME*
  1. select a range of *First* and *Last* > rclick > *Set Time Range* to narrow the focus
  1. select thread of interest via rclick on *Thread* > *Include Item*
  1. *By Name* tab shows cpu/block/fail/disk times on average (block time includes disk time, hard fault time is for pagefiles being fetch into memory)
  1. select anyting that has CPU time via *By Name* tab > rclick *CPU_TIME*
  1. threads that spawn other threads (`Task.Run`+`Task.Wait()`) are charged for the time consumed by spawned threads (aka rolling up costs, visible via *Task Scheduled* pseudo entry, causes problem of double-counting)
  1. 

*Keywords:* [TraceEvent library](https://github.com/microsoft/perfview/blob/master/documentation/TraceEvent/TraceEventLibrary.md), ETW, crash, memory dump, memory leaks, managed memory .NET GC heap, unmanaged memory allocations, exceptions, cache-miss branch mis-prediction, cache usage L1 L2 L3 \
*Similar to:* [`dotnet-trace`](https://github.com/dotnet/diagnostics/blob/master/documentation/dotnet-trace-instructions.md) (cross-platform alternative to ETW, based on Event Pipes), VS Profiler (much better) \
*Synergy with:* [`dotnet-trace`](https://github.com/dotnet/diagnostics/blob/master/documentation/dotnet-trace-instructions.md)

## WinDebug + SOS/SOSex/gSOS (`windbg`) 📊⚠️📝🔀➿
[page](http://windbg.org/) 
| [download](https://docs.microsoft.com/en-us/windows-hardware/drivers/debugger/debugger-download-tools)
| [sosex_64](http://www.stevestechspot.com/downloads/sosex_64.zip) \
*When:* We need to do detailed (hardcore) debugging to inspect CLR data structures, stack traces, memory allocations, deadlocks and more. \
*Usage:*
- memo leak: `.loadby sos clr`, `!dumpheap -stat`, (click) `!DumpHeap /d -mt <addr>`, `.load <path to the sosex.dll>\sosex`, `!mroot <address>`
- find deadlock: `.loadby sos clr`, `.load <path>\sosex`, `!dlk`, `!do <addr referenced by locked type>`, `!threads` to find Idx of  thread locking/acquiring lock, switch stack via `~<idx>s`, `!clrstack` command to display the callstack, `!refs` and follow reverse references chain
- full memory dump: `.dump /mf <path>`

*Keywords:* crash, exceptions, deadlocks, memory leaks, debugger, remote \
*Similar to:* VS Debugger & Profiler \
*Synergy with:* procdump

## Process Explorer (`procexp`) 📊📝📉📁🔌
[page](https://technet.microsoft.com/en-us/sysinternals/processexplorer.aspx) \
*When:* We want to monitor process for anomalies. Observe general behaviour. See memory consumption. See nr of threads. \
*Usage:*
- monitor *Performance Graph* > *Private Bytes* for memory consumpion (potential memo leaks)
  - priv bytes = commited by native + commited by managed code 
    - commit size FTW, working set sux
  - priv bytes (*native*) + priv bytes history (*native*) + heap bytes (*.NET*)
- monitor *Threads* and their call stacks
  - *fun fact*: `notepad.exe` has 6 threads when started

*Keywords:* memory dump, monitoring, memory usage, memory leak \
*Similar to:* Task Manager, Resource Monitor, PerfMon \
*Synergy with:* n/a

## VMMap 📝
[page](https://docs.microsoft.com/en-us/sysinternals/downloads/vmmap) \
*When:* Monitor committed virtual memory, managed (GC) heap, GC generations (memory segments). When we want to determine whether it be a managed or unmanaged memory leak. \
*Usage:*
- monitor *Managed Heap* for memo leaks in .NET apps
- monitor *Heap* for memo leaks in native apps
- Note: when looking at memory segments, note that *nr of memorysegments* = *nr of gen* x *nr of CPU cores*

*Keywords:* virtual memory, managed memory, managed heap stack, gc, gen0 gen1 gen2, memory leak \
*Similar to:* RAMMap (for physical memory) \
*Synergy with:* n/a

## RAMMap 📝
[page](https://docs.microsoft.com/en-us/sysinternals/downloads/rammap)
| [article](https://devblogs.microsoft.com/oldnewthing/?p=8493) \
*When:* We need to watch physical memory allocation, how much file data is cached in RAM. When working on I/O read/write improvement that collides with [file buffering](https://docs.microsoft.com/en-us/windows/win32/fileio/file-buffering) done by [windows cache manager](https://docs.microsoft.com/en-us/windows/win32/fileio/file-caching), especially over the network via *smb* protocol. When experiencing perf issues in apps and services caused by system file cache consuming most of the physical RAM.  \
*Usage:*
1. Run an app that does sequential/random/unbuffered IO reads
1. Monitor *Mapped File* column
1. Empty "Stand By" memory for more accurate performance results

*Keywords:* buffering, physical memory, cache manager, `FILE_FLAG_SEQUENTIAL_SCAN`, `FILE_FLAG_RANDOM_ACCESS`, `FILE_FLAG_NO_BUFFERING`,  \
*Similar to:* VMMap (for virtual memory) \
*Synergy with:* n/a

## DUMPBIN (`dumpbin`) ➿
[page](https://docs.microsoft.com/en-us/cpp/build/reference/dumpbin-reference?view=vs-2019) 
| [article](https://embeddedguruji.blogspot.com/2015/10/dumpbin-utility-tutorial.html) \
*When:* Check whether a dll is build for  x86 or x64. When we need all the methods exported by the dll. \
*Usage:*
```
> dumpbin /headers foo.dll
   8664 machine (x64)
> dumpbin /exports foo.dll
   4    3 000139E9 ?CreateXmlDecoder@@YAHPEAPEAVIXmlDecoder@@@Z
> undname ?CreateXmlDecoder@@YAHPEAPEAVIXmlDecoder@@@Z
   Undecoration of :- "?CreateXmlDecoder@@YAHPEAPEAVIXmlDecoder@@@Z"
   is :- "int __cdecl CreateXmlDecoder(class IXmlDecoder * __ptr64 * __ptr64)"
```
*Keywords:* native binary info, P/Invoke, architecture x86 x64 \
*Similar to:* n/a \
*Synergy with:* `undname`, [demangler.com](https://demangler.com/)

## ILSpy ➿
[page](https://github.com/icsharpcode/ILSpy)
| [download](https://github.com/icsharpcode/ILSpy/releases) \
*When:* We want to decompile managed code that is not obfuscated. When we wan to track loaded dependencies with respect to GAC/locality to troubleshoot assembly binding problems. When we want to learn how things work without looking at the source code. \
*Usage:*
- load dll/exe and look at the References and their appropriate locations
- load dll/exe and look what does the compiler produces for some of the language constructs,i.e: `async` state machine (uncheck *Decompile async methods*), switch/case, LINQ expressions
- view GAC via *File* > *Open from GAC..*

*Keywords:* decompile, decompiler, managed code, open source, static reverse engineering  \
*Similar to:* dotPeek (lil bit better: can generate SLN, has comments), JustDecompileEngine (produces SLN), ildasm \
*Synergy with:* n/a

## dotPeek ➿
[page](https://www.jetbrains.com/decompiler/) \
*When:* We need to do static reverse engineering. When we want to generate *.sln* from a managed binary. When we need to troubleshoot assembly binding problems. \
*Keywords:* JetBrains, decompile, decompiler, static reverse engineering \
*Similar to:* ILSpy (better at showing compiler gen code), JustDecompileEngine (more options for SLN gen) \
*Synergy with:* Visual Studio

## JustDecompile Engine ➿
[page](https://github.com/telerik/JustDecompileEngine) \
*When:* We want to decompile managed binaries in order to generate *.sln*.
*Usage:*
- `ConsoleRunner.exe /target:"./some-app.exe" /out:"./reveng"`

*Keywords:* Telerik, decompile, decompiler, static reverse engineering \
*Similar to:* ILSpy, dotPeek \
*Synergy with:* Visual Studio

## Ghidra ➿
[page](https://ghidra-sre.org/) \
*When:* We need to do static reverse engineering of unmanaged (native) code. \
*Keywords:* NSA, decompile, decompiler, unmanaged code, open source, static reverse engineering \
*Similar to:* IDA Pro (expensive) \
*Synergy with:* n/a

## Visual Studio Remote Debugger (`msvsmon`) ⚠️📝📉🔀➿
[managed](https://docs.microsoft.com/en-us/visualstudio/debugger/remote-debugging-csharp?view=vs-2019) 
| [native](https://docs.microsoft.com/en-us/visualstudio/debugger/remote-debugging-cpp?view=vs-2019) \
*When:* We need to debug flow of the app remotely. When we can install/copy & run `msvsmon`. When we are in the same domain. When we are in different domain but the client cooperates. \
*Usage:*
- build the problematic app in debug mode
- copy the binaries (+pdb) to `remote` box 
- on `remote` box
  1. allow traffic on TCP/IP ports that `msvsmon` is listening on: `4022-4025`
  1. install [Remote Tools for Visual Studio](https://my.visualstudio.com/Downloads?q=remote%20tools%20visual%20studio%202017) or cp folder with `msvsmon`
     - has to be in the same version as VS used for debug on `local` box
  1. run `msvsmon` and note down name & port
  1. *(optional)* for remote symbols, you need to config `msvsmon` to look for symbols wherever they are
- on `local` box
  1. run VS > *Debug* > *Attach to Process* 
  1. change *Connection target:* to what we got from debugger console running on `remote` box
  1. *(optional)* *Select...* > *Debug those code types:* and select *Managed (xyz)* and *Native* if needed
  1. select the process and *Attach*

*Keywords:* debugger, attach, managed native, *Allow unsafe code*, *Debugger Type to Mixed*  \
*Similar to:* windbg (does not require installation, but is harder to use) \
*Synergy with:* n/a

## Visual Studio Debugger and C Run-time Library (CRT) 📝
[page](https://docs.microsoft.com/en-us/visualstudio/debugger/finding-memory-leaks-using-the-crt-library?view=vs-2019&redirectedfrom=MSDN) \
*When:* We want to find source of memo leaks in unmanaged (native) code via built-in tools. \
*Usage:*
1. add:
   ```
   #define _CRTDBG_MAP_ALLOC
   #include <cstdlib>
   #include <crtdbg.h>
   #define new new ( _NORMAL_BLOCK , __FILE__ , __LINE__ )
   ```
1. add at the beginning of `main()`: `_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);`
1. run debug > let it finish > note down alloc number that leaked: `{<alloc_nr>} normal block at 0x007E1578, 8 bytes long`
1. set the variable `_crtBreakAlloc = <alloc_nr>;` and rerun debug or...
1. rerun the app > pause at the beginning > in *Watch 1* view for variable name `{,,ucrtbased.dll}_crtBreakAlloc` set value as `<alloc_nr>`

*Keywords:* meory leaks, false positives, C++ unmanaged native, `_CrtDumpMemoryLeaks`, `_CrtMemCheckpoint`, `_CrtMemDifference`, `_CrtMemDumpStatistics` \
*Similar to:* valgrind/memcheck (3rd party, much better), vld \
*Synergy with:* n/a

## Visual Leak Detector 📝
[page](https://github.com/KindDragon/vld) \
*When:* We want to find source of memo leaks in unmanaged (native) including COM-based leaks. \
*Usage:*
1. `#include <vld.h>` and add `vld.lib` dep in any module you want to scan
1. build the debug version of your app
1. check debugger's output window for report

*Keywords:* meory leaks, C++ unmanaged native \
*Similar to:* crt (built in, no COM-based leak detection) \
*Synergy with:* n/a

## TCPView 🔌
[page](https://docs.microsoft.com/en-us/sysinternals/downloads/tcpview) \
*When:* To diagnose state of TCP/UDP connection. To diagnose socket/port exhaustion. When we need forcibly close connections. \
*Usage:*
- run `tcpview` and watch amout of conns in `TIME_WAIT` status with same remote address
- check if antivirus is interfering with conn (proxy) as par of a scan

*Keywords:* socket port exhaustion, McAfee antivirus,   \
*Similar to:* `netstat /b /a` \
*Synergy with:* n/a

## Fiddler ➿🔌
[page](https://www.telerik.com/fiddler)
| [docs](https://docs.telerik.com/fiddler/Configure-Fiddler/Tasks/ConfigureFiddler) \
*When:* Monitor app upstream over HTTP/HTTPS proto. When we need dynamic rev eng of 3rd party library accessing its web api. When we need to do perf tests like: total page weight, HTTP caching and compression. \
*Usage:*
1. *Tools* > *Options* > *HTTPS* > check *Decrypt HTTPS traffic*
1. allow the tool to run
1. check *Inspectors* for data being send and received from the server
1. use *Composer* to fake some of the requests

*Keywords:* debugging proxy server, http https traffic recording, REST web api \
*Similar to:* wireshark \
*Synergy with:* .net (scripting, proxy in app config)

## Wireshark ➿🔌
[page](https://www.wireshark.org/) \
*When:* Monitor network activity on the machine. When we need to monitor app activity over any protocol. \
*Keywords:* RPC streams, traffic recording, network protocol analyzer, offline analysis, multi-platform \
*Similar to:* fiddler \
*Synergy with:* n/a

## MS SQL Execution Plan 📜
[page](https://docs.microsoft.com/en-us/sql/relational-databases/performance/display-an-actual-execution-plan?view=sql-server-2017) | [download](https://aka.ms/ssmsfullsetup) \
*When:* We need to quickly troubleshoot performance issues. When we want to analyze what indexes are missing. \
*Usage:*
1. do `UPDATE STATISTICS <table_or_indexed_view>`
1. click *Include Actual Execution Plan* or add `SET STATISTICS XML ON` to your TSQL
1. run TSQL query/stored procedure that needs to be inspected
1. look at *Execution plan* tab, pay attention to cost of each (sub) query
   - *non-clustered index scan* reads all rows from the non-clustered index, seek doesn't > ur missing an index
   - *table scan* reads all rows > ur missing an index
   - *hash match* adds additional calculations > maybe join/subselect operation uses no index > add it
   - when *Estimated Number of Executions* exceeds actual row count then try limiting nr of results from other tables, i.e via `WHERE xxx IS NOT NULL`
   - is it gave us a hint to create a new index which would greatly improve the performance of this query?

*Keywords:* performance, database, execution query plan, missing indexes, table scan vs index seek, hash table, hash match \
*Similar to:* `EXPLAIN QUERY PLAN` in SQLite \
*Synergy with:* n/a

## MS SQL Server Profiler (deprecated) ➿📜
[page](https://docs.microsoft.com/en-us/sql/tools/sql-server-profiler/sql-server-profiler?view=sql-server-2017)
| [download](https://aka.ms/ssmsfullsetup) \
*When:* We want to find and capture series of TSQL statements that lead to a problem. \
*Usage:*
1. run SQL Server Profiler
1. select new *Trace Template Properties* > base id on `TSQL_SPs`
1. under *Event Selection* tab choose *Show all events* and *Show all columns*
1. select events like: *RPC:Sarting*/*Completed*, *SP:Sarting*/*Completed*, *SQL:BatchStarting*/*Completed*
1. select columns like: *Duration*, *ApplicationName*, *HostName*, *NTDomainName*, *LoginName*,

*Keywords:* deprecated, replay, mssql performance monitoring \
*Similar to:* SQL Server [Extended Events](https://docs.microsoft.com/en-us/sql/relational-databases/extended-events/quick-start-extended-events-in-sql-server?view=sql-server-2017), **[SSMS XEvent Profiler](https://docs.microsoft.com/en-us/sql/relational-databases/extended-events/use-the-ssms-xe-profiler?view=sql-server-2017)** (more lightweight) \
*Synergy with:* n/a

# Cross Platform Troubleshooting Tools
## dotnet-trace 📊
[page](https://github.com/dotnet/diagnostics/blob/master/documentation/dotnet-trace-instructions.md) \
*When:* To collect .NET Core traces of a running process without any native profiler involved. \
*Usage:*
```
dotnet tool install dotnet-trace -g
dotnet-trace collect --process-id <pid>
dotnet trace convert -format speedscope <logs>
```
*Keywords:* cross platform, memory dump, .NET Core, crash, profiling, traces, ETW, Event Pipes \
*Similar to:* perfview (older, windows only) \
*Synergy with:* perfview (can load *.nettrace* file format), [speedscope.app](https://www.speedscope.app) (visualizer, i.e: CPU Flame Graphs)

## dotnet-counters 📊
[page](https://github.com/dotnet/diagnostics/blob/master/documentation/dotnet-counters-instructions.md) \
*When:* To monitor health or investigate performance. CPU usage or the rate of exceptions being thrown. Cross platform. \
*Usage:*
```
dotnet tool install dotnet-counters -g
dotnet-counters monitor --process-id <pid> System.Runtime[cpu-usage,gc-heap-size,exception-count]
```
*Keywords:* cross platform, .NET Core, monitoring, performance, exceptions \
*Similar to:* perfmon (older, windows only) \
*Synergy with:* n/a

## dotnet-dump 📊📝🔀➿
[page](https://github.com/dotnet/diagnostics/blob/master/documentation/dotnet-dump-instructions.md) \
*When:* To collect and analyze memory dumps without any native debugger involved. We want SOS commands to analyze crashes and the stack/heap. \
*Usage:*
```
> dotnet tool install dotnet-dump -g
> dotnet-dump collect --process-id <pid>
> dotnet-dump analyze ./<log>
> clrstack
- pe -lines
```

*Keywords:* cross platform, .NET Core, memory dump, crash dump, analysis \
*Similar to:* procdump (older, windows only), windbg (more sophisticated but not cross platform) \
*Synergy with:* n/a

# Troubleshooting Tools (Linux)
## Perf (`perf`) 📊📉
[article](http://www.brendangregg.com/perf.html) \
*When:* We need performance counters like cache-miss or branch miss-predictions. \
*Usage:*
```
perf top
perf stat -B <command>
```
*Keywords:* cache-miss, cache misses, branch instruction, branch-misses, mispreciction \
*Similar to:* perfview \
*Synergy with:* n/a

## Valgrind/Memcheck (`valgrind`) 📝📉🔀
[page](http://valgrind.org/docs/manual/quick-start.html) \
*When:* We want to find source of memo leaks in unmanaged (native) code. When we cannot touch the code. \
*Usage:*
1. prepare app with debug symbols `-g`
1. run `valgrind --leak-check=yes myprog arg1 arg2` and check the report

*Keywords:* memory leak, segmentation fault, out-of-range reads or writes, false positives, multiplatform, Cachegrind, Callgrind, Massif, Helgrind \
*Similar to:* VS CRT, `debugdiag` (also records leaks without touching the code, via dll injection) \
*Synergy with:* n/a

---

# Tool Template
## TODO (todo) 📊⚠️📝📉🔀➿📁🔌📜
[page](#todo) | [article](#todo) | [usage](#todo) \
*When:* TODO \
*Usage:*
```
```
*Keywords:* TODO \
*Similar to:* TODO \
*Synergy with:* TODO
