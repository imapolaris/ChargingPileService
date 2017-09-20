﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using CPS.Entities;

namespace CPS.DB
{
    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public partial class CPS_Entities : DbContext
    {
        public CPS_Entities()
            : base("name=cps_db")
        {
            //Configuration.ProxyCreationEnabled = false;
            //Configuration.AutoDetectChangesEnabled = false;
            //Configuration.ValidateOnSaveEnabled = false;
            //Configuration.LazyLoadingEnabled = false;
            Database.CreateIfNotExists();
            //此处是自动更新数据表，当模型改变的时候。  
            //修改Model后，自动更新数据表
            //Database.SetInitializer<CPS_Entities>(new DropCreateDatabaseIfModelChanges<CPS_Entities>());

            this.Database.Log = msg =>
            {
                Debug.WriteLine(msg);
            };
        }
        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            return base.ValidateEntity(entityEntry, items);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }

        #region entities

        public virtual DbSet<Station> Sys_PM_Permissions { get; set; }

        #endregion
    }
}