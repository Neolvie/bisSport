namespace bisSport.Server.Repository
{
  public interface IUnitOfWork
  {
    void BeginTransaction();
    void Commit();
    void Rollback();
  }
}