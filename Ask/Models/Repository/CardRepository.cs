using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using WorkV3.Global;

namespace WorkV3.Models.Repository
{
    public class CardRepository : RepositoryBase<CardsModels>, Interface.ICardRepository<CardsModels>
    {
        public CardRepository()
        {
            SetTableName("Cards");
        }
    }
}