using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ApplicationCore.Connections.Interfaces
{
    public interface IDatabaseConnection: IDbConnection
    {
        IDbTransaction Transaction { get; set; }
    }
}
