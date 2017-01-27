using bisSport.Server.Repository;

namespace bisSport.Tests
{
  public class TestBase
  {
    public UnitOfWork Repository;

    public TestBase()
    {
      Repository = new UnitOfWork();
    }
  }
}