using System;
using System.Diagnostics;
 
// Log writter for working with Azure Functions 
// for the Xrm.Tooling.Connector
public class Toolinglistener : TraceListener
{
  private TraceWriter _log;
 
  public Toolinglistener(string name, 
            TraceWriter logger) : base(name)
  {
    _log = logger;
  }
 
  public override void Write(string message)
  {
    _log?.Info(message);
  }
 
  public override void WriteLine(
            string message)
  {
    _log?.Info(message);
  }
}