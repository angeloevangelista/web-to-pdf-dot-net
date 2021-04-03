using System;

namespace WebToPdf.Errors
{
  public class AppError : Exception
  {
    public AppError(string message) : base(message)
    {
    }
  }
}