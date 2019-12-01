using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ApplicationCore.Connections.Interfaces;

namespace Infrastructure.Connections
{
    public class DatabaseConnection : IDatabaseConnection
    {
        public IDbConnection Connection { get; } = null;

        public IDbTransaction Transaction { get; set; } = null;

        public DatabaseConnection(IDbConnection connection)
        {
            this.Connection = connection;
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            Transaction = Connection.BeginTransaction(il);
            return Transaction;
        }

        public IDbTransaction BeginTransaction()
        {
            Transaction = Connection.BeginTransaction();
            return Transaction;
        }

        public void ChangeDatabase(string databaseName)
        {
            Connection.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            Connection.Close();
        }

        public string ConnectionString
        {
            get
            {
                return Connection.ConnectionString;
            }
            set
            {
                Connection.ConnectionString = value;
            }
        }

        public int ConnectionTimeout
        {
            get { return Connection.ConnectionTimeout; }
        }

        public IDbCommand CreateCommand()
        {
            return Connection.CreateCommand();
        }

        public string Database
        {
            get { return Connection.Database; }
        }

        public void Open()
        {
            Connection.Open();
        }

        public ConnectionState State
        {
            get { return Connection.State; }
        }

        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}
