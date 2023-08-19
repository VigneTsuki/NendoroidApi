using System;

namespace NendoroidApi.Data.Base
{
    public interface IUnitOfWork
    {
        public interface IUnitOfWork : IDisposable
        {
            void BeginTransaction();
            void Commit();
            void Rollback();
        }
    }
}
