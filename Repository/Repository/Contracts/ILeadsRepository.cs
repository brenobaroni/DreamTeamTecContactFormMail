﻿using Repository.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository.Contracts
{
    public interface ILeadsRepository
    {
        Task<int> SaveAsync(Leads lead);
    }
}
