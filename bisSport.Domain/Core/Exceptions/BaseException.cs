using System;

namespace bisSport.Domain.Core.Exceptions
{
  public abstract class BaseException : Exception
  {
    protected BaseException(string message) : base(message)
    {
      
    }
  }
}